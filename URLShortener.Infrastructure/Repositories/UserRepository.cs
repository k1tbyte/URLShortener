using Microsoft.EntityFrameworkCore;
using URLShortener.Domain.Entities;
using URLShortener.Infrastructure.Data.Context;
using URLShortener.Infrastructure.Repositories.Abstraction;

namespace URLShortener.Infrastructure.Repositories;

public sealed class UserRepository(AppDbContext context)
    : BaseAsyncCrudRepository<User, IUserRepository>(context, context.Users), IUserRepository
{
    public Task<bool> UserExistsAsync(string login)
    {
        return Set.AnyAsync(o => o.Username == login);
    }

    public Task<User?> GetByUsernameAsync(string username)
    {
        return Set.FirstOrDefaultAsync(o => o.Username == username);
    }

    public Task AddUserAsync(User user)
    {
        return WithAutoSave().Add(user);
    }

    public Task DeleteSessionAsync(Guid refreshToken)
    {
        return context.Sessions
            .Where(o => o.Token == refreshToken)
            .ExecuteDeleteAsync();
    }

    public async Task AddSessionAsync(Session session)
    {
        await context.Sessions.AddAsync(session);
        await context.SaveChangesAsync();
    }

    public Task EnsureSessionLimitAsync(int userId, int maxSessions = 4)
    {
        var sessions = context.Sessions
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.ExpiresIn)
            .Skip(maxSessions);
        
        return sessions?.ExecuteDeleteAsync();
    }

    public Task<Session?> GetSessionAsync(Guid refreshToken)
    {
        return context.Sessions.Include(o => o.User)
            .FirstOrDefaultAsync(o => o.Token == refreshToken);
    }
}