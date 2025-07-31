using Microsoft.AspNetCore.Http;
using URLShortener.Domain.Entities;
using URLShortener.Infrastructure.Lib;
using URLShortener.Infrastructure.Repositories.Abstraction;

namespace URLShortener.Infrastructure.Services;

public record AuthResult(string RefreshToken, string AccessToken);
public class AuthService(IUserRepository userRepo, JwtService tokenService, IHttpContextAccessor accessor)
{
    private const int RefreshTokenLifetime = 7; //days
    /*#if DEBUG
private const int AccessTokenLifetime = 200; //minutes
#else*/
    private const int AccessTokenLifetime = 10; //minutes
    /*#endif*/
    private const int MaxSessionsAmount = 5;
    
    public async Task RegisterAsync(string login, string password)
    {
        if (await userRepo.UserExistsAsync(login))
        {
            throw new ApiException("User already exists", 409);
        }

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User { Username = login, Password = hashedPassword };

        await userRepo.AddUserAsync(user);
    }
    
    public async Task<AuthResult> LoginAsync(string login, string password)
    {
        var user = await userRepo.GetByUsernameAsync(login);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
        {
            throw new ApiException("Invalid username or password", 401);
        }

        var access = tokenService.GenerateAccessToken([
            new("userid", user.Id.ToString()),
            new("username", user.Username),
            new("role", user.Role.ToString())
        ], AccessTokenLifetime);

        var refresh = await _createSession(access, user);

        return new AuthResult(
            refresh.ToString(),
            access
        );
    }

    public async Task<AuthResult> RefreshSessionAsync(string accessToken, Guid refreshToken)
    {
        var payload = tokenService.GetPayload(accessToken);
        var userId = payload.Claims.FirstOrDefault(c => c.Type == "userid")?.Value;
        if (userId == null)
        {
            throw new ApiException("Invalid token payload", 400);
        }

        var session = await userRepo.GetSessionAsync(refreshToken);
        if (session == null || session.UserId != int.Parse(userId))
        {
            throw new ApiException("Invalid session or user ID mismatch", 401);
        }
        
        await userRepo.DeleteSessionAsync(refreshToken);
        
        if(session.ExpiresIn <= DateTimeOffset.UtcNow.ToUnixTimeSeconds())
        {
            throw new ApiException("Session expired", 401);
        }
        
        var access = tokenService.GenerateAccessToken([
            new("userid", userId),
            new("username", payload.Claims.FirstOrDefault(c => c.Type == "username")?.Value ?? ""),
            new("role", payload.Claims.FirstOrDefault(c => c.Type == "role")?.Value ?? "")
        ], AccessTokenLifetime);
        
        var refresh = await _createSession(access, session.User);
        return new AuthResult(
            refresh.ToString(),
            access
        );
    }

    
    private async Task<Guid> _createSession(string accessToken, User user)
    {
        var refreshToken = Guid.NewGuid();
        var expires = DateTimeOffset.UtcNow.AddDays(RefreshTokenLifetime);

        await userRepo.EnsureSessionLimitAsync(user.Id, MaxSessionsAmount - 1);
        
        await userRepo.AddSessionAsync(new Session()
        {
            ExpiresIn = expires.ToUnixTimeSeconds(),
            Token     = refreshToken,
            UserId    = user.Id,
        });
        
        return refreshToken;
    }
}