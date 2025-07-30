using Application.Models;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Identity;

public class IdentityApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
{
    public IdentityApplicationDbContext(DbContextOptions<IdentityApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserDev> UserDev { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Customize the ASP.NET Identity table names or schema if desired:
        builder.Entity<ApplicationUser>().ToTable("Users", "auth");
        builder.Entity<ApplicationRole>().ToTable("Roles", "auth");
    }
}