public sealed record UserInfo(
    Guid Id,
    string Username,
    string PasswordHash,
    string Role
);