namespace StockFlow.Application.Authentication;

public sealed record UserDraft(
    string UserName,
    string PasswordHash,
    string Role
);