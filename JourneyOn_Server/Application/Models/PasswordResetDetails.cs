namespace Application.Models;

public sealed record PasswordResetDetails(string Email, string Token, string Password);