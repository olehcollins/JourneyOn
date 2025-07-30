using Application.Models;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class ProgressController(IdentityApplicationDbContext dbContext, UserManager<ApplicationUser> userManager) : ControllerBase
{
    [HttpPost("update-progress")]
    public async Task<ActionResult> UpdateUserProgress([FromBody] ProgressModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ResponseModel<string>(null, "Invalid input"));
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

        progressRecord.Status = model.Status;
        var updateProgressResult = dbContext.ProgressTable.Update(progressRecord);
        if (await dbContext.SaveChangesAsync() < 0)
        {
            return  BadRequest(new ResponseModel<ProgressModel>(progressRecord, $"Failed to update Progress record"));
        }

        var milestoneCount = await dbContext.ProgressTable.CountAsync(x => x.UserId == model.UserId &&  x.MilestoneId == model.MilestoneId);
        var completedMilestoneCount = await dbContext.ProgressTable.CountAsync(x => x.Status == "completed");
        var progressPercentage = Math.Floor(completedMilestoneCount / (decimal)milestoneCount);

        // find user
        var user = await userManager.FindByIdAsync(model.UserId.ToString());
        if (user == null)
        {
            return NotFound(new ResponseModel<string>(null, $"User with id {model.UserId} not found."));
        }

        string? message = null;
        if (model.Status == "completed")
        {
            user.MessageId += 1;
            message = $"Well done you just completed milestone number: {completedMilestoneCount}";
        }
        else
        {
            user.MessageId -= 1;
        }


        user.ProgressScore = progressPercentage;
        user.MessageId = completedMilestoneCount;

        var updateResult = await userManager.UpdateAsync(user);

        return updateResult.Succeeded
            ? Ok(new ResponseModel<decimal>(progressPercentage, message ?? "progress updated"))
            : BadRequest(
                new ResponseModel<string[]>(updateResult.Errors.Select(e => e.Description).ToArray(),
                    $"Failed to update user {model.UserId}"));
    }
}