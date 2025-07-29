using Application.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    // TODO: add your DbSet<T> properties here, e.g.:
    // public DbSet<WeatherForecast> WeatherForecasts { get; set; }

    public DbSet<User> UsersDev { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Map the User entity to the correct table and column names
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("UsersDev");

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.Name)
                .HasColumnName("name");

            entity.Property(e => e.Email)
                .HasColumnName("email");
        });
    }

}