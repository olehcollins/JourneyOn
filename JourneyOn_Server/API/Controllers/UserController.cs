using Application.Models;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(IdentityApplicationDbContext dbContext, UserManager<ApplicationUser> userManager) : ControllerBase
{
    [HttpPost("create-apprentice")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserModel dto)
    {
        if (!ModelState.IsValid)
        {
            return  BadRequest(new ResponseModel<ApplicationUser>(null, "Invalid input"));
        }
        // 1) Skip if already exists
        if (await userManager.FindByEmailAsync(dto.Email) != null)
        {
            return BadRequest(
                new ResponseModel<ApplicationUser>(null, "Email already exists."));
        }

        // 2) Create the user
        var user = new ApplicationUser
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            UserName = dto.Email,
            Email = dto.Email,
            CourseId = dto.CourseId
        };

        var createResult = await userManager.CreateAsync(user, dto.Password);
        if (!createResult.Succeeded)
        {
            return BadRequest(
                new ResponseModel<IEnumerable<IdentityError>>(createResult.Errors, "Failed to create user."));
        }

        var addRoleResult = await userManager.AddToRoleAsync(user, dto.Role);
        if (!addRoleResult.Succeeded)
        {
            return BadRequest(
                new ResponseModel<IEnumerable<IdentityError>>(addRoleResult.Errors, $"Failed to set user role to {dto.Role}"));
        }

        var userId = (await userManager.FindByEmailAsync(dto.Email))!.Id;
        var milestones = await dbContext.MilestoneTable
            .Where(x => x.Id == dto.CourseId)
            .Select(m => m.Id)
            .ToArrayAsync();

        var progressEntries = milestones.Select(milestoneId => new ProgressModel
        {
            UserId = userId,
            MilestoneId = milestoneId,
            Status = "uncompleted",
            CompletedAt = null
        }).ToList();

        dbContext.ProgressTable.AddRange(progressEntries);
        if (await dbContext.SaveChangesAsync() < 0)
        {
            return Ok(new ResponseModel<ApplicationUser>(user, $"new {dto.Role} added successfully but failed to create milestones for the user"));
        }


        return Ok(new ResponseModel<ApplicationUser>(user, "new Apprentice created successfully"));
    }

    [HttpPut("update-apprentice")]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserModel dto)
    {
        var user = await userManager.FindByEmailAsync(dto.Email);

        if (user == null)
        {
            return BadRequest(
                new ResponseModel<ApplicationUser>(null, $"User with email {dto.Email} doesn't exist."));
        }

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.UserName = dto.Email;
        user.Email = dto.Email;
        user.CourseId = dto.CourseId;
        user.ProgressScore = dto.ProgressScore;

        var updateResult = await userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            return BadRequest(
                new ResponseModel<IEnumerable<IdentityError>>(updateResult.Errors, "Failed to update user data"));
        }

        // Remove existing roles
        var currentRoles = await userManager.GetRolesAsync(user);
        var removeRolesResult = await userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!removeRolesResult.Succeeded)
        {
            return BadRequest(
                new ResponseModel<IEnumerable<IdentityError>>(removeRolesResult.Errors, "Failed to remove existing roles"));
        }

        // Add the new role
        var updateRoleResult = await userManager.AddToRoleAsync(user, dto.Role);
        if (!updateRoleResult.Succeeded)
        {
            return BadRequest(
                new ResponseModel<IEnumerable<IdentityError>>(updateRoleResult.Errors, "Failed to update User Role"));
        }

        return Ok(new ResponseModel<ApplicationUser>(user, $"new {dto.Role} updated successfully"));
    }

    [HttpGet("get-user-details/{id}")]
    public async Task<IActionResult> GetUserDetails(int id)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            return NotFound(new ResponseModel<string>(null, "User not found"));
        }
        // get message
        var messageRecord = await dbContext.MessageTable.FindAsync(user.MessageId);
        // get course
        var courseRecord = await dbContext.CourseTable.FindAsync(user.CourseId);
        // get dairy entries
        var dairyRecords = await dbContext.DairyTable.Where(x => x.UserId == user.Id).ToListAsync();

        var data = new Dictionary<string, object>
        {
            { "course", courseRecord.CourseName},
            {"message", messageRecord.Message},
            {"diary_entries", dairyRecords}
        };
        ;

        return  Ok(new ResponseModel<Dictionary<string, object>>(data, ""));
    }

    [HttpGet("get-milestones/{id}")]
    public async Task<IActionResult> GetMilestones(int id)
    {
        var users = await userManager.FindByIdAsync(id.ToString());
        if (users == null)
        {
            return NotFound(new ResponseModel<string>(null, "User not found"));
        }
        var milestones = await dbContext.MilestoneTable.Where(x =>  x.CourseId == users.CourseId).ToListAsync();

        return Ok(new ResponseModel<List<MilestoneModel>>(milestones, ""));
    }

    [AllowAnonymous]
    [HttpGet("all-users")]
    public async Task<IActionResult> GetDevUsers()
    {
        var users = await userManager.Users.ToListAsync();
        return Ok(users);
    }
}