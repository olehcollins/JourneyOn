namespace Application.Models;

public sealed class SignInedUser
{
    public required int Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required int CourseId { get; init; }
    public required decimal ProgressScore { get; init; }
    public required string Email { get; init; }
    public required string Role { get; init; }
    public required Dictionary<string, string?> TokenData { get; init; }
}