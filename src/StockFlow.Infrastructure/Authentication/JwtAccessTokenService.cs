using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using StockFlow.Application.Authentication;

public sealed class JwtAccessTokenService : IAccessTokenService
{
    private readonly JwtOptions _jwtOptions;

    public JwtAccessTokenService(JwtOptions jwtOptions)
    {
        _jwtOptions = jwtOptions;
    }

    public string CreateToken(UserInfo user)
    {
        var claims = new[]
        {
            new Claim(
                JwtRegisteredClaimNames.Sub,
                user.Id.ToString()),

            new Claim(
                JwtRegisteredClaimNames.UniqueName,
                user.Username),

            new Claim(
                ClaimTypes.Role,
                user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationMinutes),
            signingCredentials: credentials);

        try
        {
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw new InvalidOperationException(ex.Message, ex);
        }
    }
}