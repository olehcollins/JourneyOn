using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Application.Models;

[Table("progress")]
public sealed class ProgressModel
{
    [Key]
    [Column("user_id")]
    public int UserId { get; set; }

    [Key]
    [Column("milestone_id")]
    public int MilestoneId {get; set;}

    [Column("status")]
    public string Status { get; set; }

    [Column("completed_at")]
    public DateTime CompletedAt { get; set; }
};