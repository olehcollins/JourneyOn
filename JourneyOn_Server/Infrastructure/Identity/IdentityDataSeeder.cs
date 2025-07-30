using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Infrastructure.Identity
{
    public class IdentityDataSeeder
    {
        private static readonly string[] Roles = new[] { "Admin", "User" };
        private const string AdminEmail = "oleh@gmail.com";
        private const string AdminPassword = "P@ssw0rd!"; // Ideally read from secret store

        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var logger = serviceProvider
                .GetRequiredService<ILogger<IdentityDataSeeder>>();

            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // 1) Ensure roles
            foreach (var roleName in Roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var role = new ApplicationRole { Name = roleName };
                    var result = await roleManager.CreateAsync(role);
                    if (result.Succeeded)
                        logger.LogInformation("Created role '{Role}'", roleName);
                    else
                        logger.LogError("Error creating role '{Role}': {Errors}",
                            roleName, string.Join(", ", result.Errors));
                }
            }

            // 2) Ensure default Admin user
            var admin = await userManager.FindByEmailAsync(AdminEmail);
            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = AdminEmail,
                    Email = AdminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, AdminPassword);
                if (result.Succeeded)
                {
                    logger.LogInformation("Created default admin user '{User}'", AdminEmail);
                    await userManager.AddToRoleAsync(admin, "Admin");
                    logger.LogInformation("Assigned '{User}' to 'Admin' role", AdminEmail);
                }
                else
                {
                    logger.LogError("Error creating admin user '{User}': {Errors}",
                        AdminEmail, string.Join(", ", result.Errors));
                }
            }
        }
    }
}