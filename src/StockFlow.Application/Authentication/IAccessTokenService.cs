namespace StockFlow.Application.Authentication;

public interface IAccessTokenService
{
    string CreateToken(UserInfo user);
}