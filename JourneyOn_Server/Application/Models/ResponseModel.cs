namespace Application.Models;

public sealed record ResponseModel<T>(T? Data, string? Message);