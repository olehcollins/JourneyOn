// Models/DiaryEntry.cs

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("diary_entries")]
public class DiaryModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; private set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("title")]
    public string Title { get; set; } = null!;

    [Column("body")]
    public string Body { get;  set; } = null!;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    private DiaryModel() { }    // EF Core

    public static DiaryModel Create(int userId, string title, string body)
    {
        var now = DateTime.UtcNow;
        return new DiaryModel {
            UserId    = userId,
            Title     = title,
            Body      = body,
            CreatedAt = now,
            UpdatedAt = now
        };
    }
}