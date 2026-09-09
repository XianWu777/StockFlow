using System.Security.Authentication;

namespace StockFlow.Application.Authentication;

public sealed class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAccessTokenService _accessTokenService;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork,
        IAccessTokenService accessTokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
        _accessTokenService = accessTokenService;
    }

    public async Task RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var exists = await _userRepository.ExistsAsync(
            request.Username,
            cancellationToken);

        if (exists)
        {
            throw new UsernameAlreadyExistsException();
        }

        var passwordHash = _passwordHasher.HasPassword(request.Password);

        var user = new UserDraft(
            request.Username,
            passwordHash,
            "User");

        await _userRepository.AddAsync(
            user,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<LoginResponse> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByUsernameAsync(
            request.Username,
            cancellationToken);

        if (user is null)
            throw new InvalidCredentialException();

        var passwordValid =
            _passwordHasher.Verify(request.Password, user.PasswordHash);

        if (!passwordValid)
            throw new InvalidCredentialsException();

        var accessToken = _accessTokenService.CreateToken(user);

        return new LoginResponse(accessToken);
    }
}