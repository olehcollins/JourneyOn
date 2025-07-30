using Application.Models;
using Infrastructure.Identity;
using Infrastructure.Persistence;
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


    [HttpGet("get-users")]
    public async Task<IActionResult> GetDevUsers()
    {
        var users = await userManager.Users.ToListAsync();
        return Ok(users);
    }

    [HttpGet("get-dev-users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await dbContext.UserDev.AsNoTracking().ToListAsync();

        return Ok(users);
    }
}