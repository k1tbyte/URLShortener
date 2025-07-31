using URLShortener.Domain.Entities;
using URLShortener.Infrastructure.Data.Context;
using URLShortener.Infrastructure.Repositories.Abstraction;

namespace URLShortener.Infrastructure.Repositories;

public sealed class UrlsRepository(AppDbContext context)
    : BaseAsyncCrudRepository<ShortUrl, IUrlsRepository>(context, context.ShortUrls), IUrlsRepository
{
    
}