using URLShortener.Domain.Entities;
using URLShortener.Infrastructure.Repositories.Abstraction;

public interface IUrlsRepository : IAsyncCrudRepository<ShortUrl, IUrlsRepository>
{
    
}