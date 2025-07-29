using Application.Models;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(ApplicationDbContext db) : ControllerBase
{
    [HttpGet("get-users")]
    public async Task<IEnumerable<User>> Get() =>
        await db.UsersDev.ToListAsync();

    [HttpGet("true")]
    public string GetTest() => "true";
}