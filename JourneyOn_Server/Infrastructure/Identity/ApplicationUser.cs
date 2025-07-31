using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

[Table("Users", Schema = "auth")]
public sealed class ApplicationUser : IdentityUser<int>
{
    [Column("FirstName")]
    public string FirstName { get; set; }

    [Column("LastName")]
    public string LastName { get; set; }

    [Column("CourseId")]
    public int? CourseId { get; set; }

    [Column("ProgressScore", TypeName = "numeric(5,2)")]
    public decimal ProgressScore { get; set; } = 0.00m;

    [Column("CreatedAt")]
    public DateTime CreatedAt { get; set; }

    [Column("UpdatedAt")]
    public DateTime UpdatedAt { get; set; }

    [Column("message_id")]
    public int? MessageId { get; set; }
}