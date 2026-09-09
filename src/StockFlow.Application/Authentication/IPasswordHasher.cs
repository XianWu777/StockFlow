namespace StockFlow.Application.Authentication;

public interface IPasswordHasher
{
    string HasPassword(string password);
    bool Verify(string password, string hash);
}