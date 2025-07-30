using URLShortener.Domain.Entities;

namespace URLShortener.Infrastructure.Repositories.Abstraction;

public interface IUserRepository : IAsyncCrudRepository<User, IUserRepository>
{
    Task<bool> UserExistsAsync(string login);
    Task<User?> GetByUsernameAsync(string username);
    Task AddUserAsync(User user);
    Task DeleteSessionAsync(Guid refreshToken);
    Task AddSessionAsync(Session session);
    Task EnsureSessionLimitAsync(int userId, int maxSessions = 5);
    Task<Session?> GetSessionAsync(Guid refreshToken);
}