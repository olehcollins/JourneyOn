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
    public DbSet<MilestoneModel> MilestoneTable { get; set; }
    public DbSet<MessageModel> MessageTable { get; set; }
    public DbSet<CourseModel> CourseTable { get; set; }
    public DbSet<DiaryModel>  DairyTable { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Customize the ASP.NET Identity table names or schema if desired:
        builder.Entity<ApplicationUser>().ToTable("Users", "auth");
        builder.Entity<ApplicationRole>().ToTable("Roles", "auth");

        builder.Entity<ProgressModel>()
            .HasKey(p => new { p.UserId, p.MilestoneId });

        builder.Entity<MilestoneModel>()
            .HasKey(p => p.Id);
        builder.Entity<DiaryModel>(eb =>
        {
            eb.HasKey(e => e.Id);
            eb.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            eb.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            eb.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .ValueGeneratedOnAddOrUpdate();
        });
    }
}