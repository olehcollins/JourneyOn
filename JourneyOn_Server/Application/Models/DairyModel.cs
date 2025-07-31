using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Application.Models;

[Table("diary_entries")]
public class DairyModel
{
    [Key]
    [Column("id")]
    public int Id { get; init; }

    [Column("user_id")]
    public int UserId { get; init; }

    [Column("title")]
    public string Title { get; init; }

    [Column("body")]
    public string Body { get; init; }

    [Column("created_at")]
    public DateTime CreatedAt { get; init; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; init; }
}