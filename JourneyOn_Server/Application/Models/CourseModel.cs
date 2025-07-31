using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Application.Models;

[Table("courses")]
public record CourseModel
{
    [Key]
    [Column("course_id")]
    public int Id { get; init; }

    [Column("course_name")]
    public string CourseName { get; init; }
};