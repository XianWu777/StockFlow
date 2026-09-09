using System.ComponentModel.DataAnnotations;

namespace StockFlow.Application.Authentication;

public sealed record LoginRequest(
    [Required]
    string Username,
    [Required]
    string Password
);