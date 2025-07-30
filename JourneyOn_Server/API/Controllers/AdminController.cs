using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Application.Models;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
        : ControllerBase
    {
        // POST: /api/admin/create-default-admin
        [HttpPost("create-default-admin")]
        public async Task<IActionResult> CreateDefaultAdmin()
        {
            // hard-coded credentials
            const string email = "oleh@gmail.com";
            const string password = "Secret123@";

            // 1) Skip if already exists
            if (await userManager.FindByEmailAsync(email) != null)
                return BadRequest(new { error = "Admin user already exists." });

            // 2) Create the user
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };
            var createResult = await userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
                return BadRequest(createResult.Errors);

            // 3) Ensure "Admin" role exists
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                var roleResult = await roleManager.CreateAsync(new ApplicationRole { Name = "Admin" });
                if (!roleResult.Succeeded)
                    return StatusCode(500, roleResult.Errors);
            }

            // 4) Assign the Admin role
            var addRoleResult = await userManager.AddToRoleAsync(user, "Admin");
            if (!addRoleResult.Succeeded)
                return StatusCode(500, addRoleResult.Errors);

            return Ok(new { message = "Default admin created.", userId = user.Id });
        }
    }
}