using System.ComponentModel.DataAnnotations;

namespace Application.Models;

public class CreateDiaryEntryDto
{
    [Required]
    public int UserId { get; init; }

    [Required, MaxLength(200)]
    public string Title { get; init; }

    [Required]
    public string Body { get; init; }
}

public record UpdateDiaryEntryDto(string Title, string Body);