using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Application.Models;

[Table("messages")]
public record MessageModel
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("message")]
    public string Message { get; set; }
}