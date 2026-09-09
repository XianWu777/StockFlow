using System.ComponentModel.DataAnnotations;

namespace StockFlow.Application.Authentication;

public sealed record RegisterRequest(
    [Required]
    [StringLength(100, MinimumLength = 3)]
    string Username,

    [Required]
    [StringLength(100, MinimumLength = 8)]
    string Password
);