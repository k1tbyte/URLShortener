namespace URLShortener.Infrastructure.Services;

public interface IAuthService
{
    Task RegisterAsync(string login, string password);
    Task<AuthResult> LoginAsync(string login, string password);
    Task LogoutAsync(Guid refreshToken);
    Task<AuthResult> RefreshSessionAsync(string accessToken, Guid refreshToken);
}