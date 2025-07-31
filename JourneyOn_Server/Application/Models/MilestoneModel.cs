using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Application.Models;

[Table("milestones")]
public sealed class MilestoneModel
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("title")]
    public string Title { get; set; }

    [Column("description")]
    public string Description { get; set; }

    [Column("course_id")]
    public int CourseId { get; set; }        // ← was string

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }   // ← was string
}