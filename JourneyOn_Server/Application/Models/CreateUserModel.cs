namespace Application.Models;

public sealed class CreateUserModel
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required int CourseId { get; init; }
    public decimal ProgressScore { get; init; } = 0.00m;
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string Role { get; init; }
}