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
    [HttpGet("get-users")]
    public async Task<IActionResult> GetDevUsers()
    {
        var users = await userManager.Users.ToListAsync();
        return Ok(users);
    }

    /*[HttpGet("get-users")]
    public async Task<IActionResult> GetDevUsers()
    {
        var users = await dbContext.Users
            .Select(u => new
            {
                u.Id,
                u.UserName,
                u.Email,
                u.EmailConfirmed
            })
            .ToListAsync();

        return Ok(users);
    }*/

    [HttpGet("get-dev-users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await dbContext.UserDev.AsNoTracking().ToListAsync();

        return Ok(users);
    }


    [HttpGet("weather-forecast")]
    public WeatherForecast[] HealthCheck()
    {
        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();

        return forecast;
    }
}