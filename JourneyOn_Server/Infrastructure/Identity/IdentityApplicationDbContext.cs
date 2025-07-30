using Application.Models;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Identity;

public class IdentityApplicationDbContext(DbContextOptions<IdentityApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser, ApplicationRole, int>(options)
{
    public DbSet<UserDev> UserDev { get; set; }
    public DbSet<ProgressModel> ProgressTable { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Customize the ASP.NET Identity table names or schema if desired:
        builder.Entity<ApplicationUser>().ToTable("Users", "auth");
        builder.Entity<ApplicationRole>().ToTable("Roles", "auth");

        builder.Entity<ProgressModel>()
            .HasKey(p => new { p.UserId, p.MilestoneId });
    }
}