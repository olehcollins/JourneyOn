namespace Application.Models;

public sealed class UpdateUserModel
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required int CourseId { get; init; }
    public required decimal ProgressScore { get; init; }
    public required string Email { get; init; }
    public required string Role { get; init; }
}