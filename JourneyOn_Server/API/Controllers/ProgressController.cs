using System.Security.Claims;
using Application.Models;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class ProgressController(IdentityApplicationDbContext dbContext, UserManager<ApplicationUser> userManager) : ControllerBase
{
    [HttpPut("update-milestone-status")]
    public async Task<ActionResult> UpdateUserProgress([FromBody] ProgressModel model)
    {
        if (!ModelState.IsValid || (model.Status != "completed" && model.Status != "in-progress"))
        {
            return BadRequest(new ResponseModel<string>(ModelState.ToString(), "Invalid input"));
        }

        var callerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        if (callerId != model.UserId)
        {
            return Forbid();
        }

        var progressRecord = await dbContext.ProgressTable.FirstOrDefaultAsync(x => x.UserId == model.UserId &&  x.MilestoneId == model.MilestoneId);
        if (progressRecord == null)
        {
            return  NotFound(new ResponseModel<string>(null, $"No Progress record found for user with id {model.UserId} and milestoneId {model.MilestoneId}."));
        }

        if (progressRecord.Status == model.Status)
        {
            return BadRequest(new ResponseModel<string>(null, "Status has not changed."));
        }

        progressRecord.Status = model.Status.ToLower();
        progressRecord.CompletedAt = DateTime.UtcNow;

        dbContext.ProgressTable.Update(progressRecord);
        if (await dbContext.SaveChangesAsync() < 0)
        {
            return BadRequest(new ResponseModel<string>(null, "Failed to update Progress"));
        }

        // find user
        var user = await userManager.FindByIdAsync(model.UserId.ToString());
        if (user == null)
        {
            return NotFound(new ResponseModel<string>(null, $"User with id {model.UserId} not found."));
        }

        if (model.Status == "completed")
        {
            user.MessageId += 1;
        }
        else
        {
            user.MessageId -= 1;
        }

        // get progress stats and update user progress score
        var milestoneCount = await dbContext.ProgressTable.CountAsync(x => x.UserId == model.UserId);
        var completedMilestones = await dbContext.ProgressTable.Where(x => x.Status == "completed").ToArrayAsync();
        var progressPercentage = Math.Floor((completedMilestones.Length / (decimal)milestoneCount) * 100);

        user.ProgressScore = progressPercentage;
        user.MessageId = completedMilestones.Length;

        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            return BadRequest(new ResponseModel<string>(null, "Failed to update user"));
        }

        // create object data to send to the frontend
        var milestoneRecords = new List<Dictionary<string, object>>();
        foreach (var milestone in completedMilestones)
        {
            var mappedMilestoneData = new Dictionary<string, object>();
            var record = await dbContext.MilestoneTable.FirstOrDefaultAsync(x => x.Id == milestone.MilestoneId);

            mappedMilestoneData.Add("milestone", record!);
            mappedMilestoneData.Add("status", milestone.Status);
            mappedMilestoneData.Add("completedAt", milestone.CompletedAt!.Value.ToLocalTime().ToString("f"));
            mappedMilestoneData.Add("progressPercentage", progressPercentage);

            milestoneRecords.Add(mappedMilestoneData);
        }

        var message = model.Status == "completed"
            ? $"Well done you just completed milestone number: {completedMilestones.Length}"
            : null;

        return updateResult.Succeeded
            ? Ok(new ResponseModel<List<Dictionary<string, object>>>(
                milestoneRecords,
                message ?? "progress updated"))
            : BadRequest(
                new ResponseModel<string[]>(updateResult.Errors.Select(e => e.Description).ToArray(),
                    $"Failed to update user {model.UserId}"));
    }
}