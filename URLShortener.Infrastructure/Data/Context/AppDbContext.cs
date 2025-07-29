using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using URLShortener.Domain.Entities;

namespace URLShortener.Infrastructure.Data.Context;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration)
    : DbContext(options)
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Session> Sessions { get; set; } = null!;
    public DbSet<ShortUrl> ShortUrls { get; set; } = null!;
    public DbSet<UrlClick> UrlClicks { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("en_US.UTF-8");
        modelBuilder.UseIdentityColumns();

        modelBuilder.Entity<User>()
            .HasMany(e => e.Sessions)
            .WithOne(e => e.User)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ShortUrl>()
            .HasMany(e => e.UrlClicks)
            .WithOne(e => e.ShortUrl)
            .HasForeignKey(e => e.ShortUrlId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ShortUrl>()
            .HasOne(e => e.CreatedByUser)
            .WithMany(e => e.ShortUrls)
            .HasForeignKey(e => e.CreatedBy)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasMany(e => e.ShortUrls)
            .WithOne(e => e.CreatedByUser)
            .HasForeignKey(e => e.CreatedBy)
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("Database"));
    }
}