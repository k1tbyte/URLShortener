using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace URLShortener.Domain.Entities;

[Table("url_clicks")]
public sealed class UrlClick
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("clicked_at")]
    public long ClickedAt { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    [Column("ip_address")]
    public string? IpAddress { get; set; }

    [Column("user_agent")]
    public string? UserAgent { get; set; }
    
    [ForeignKey(nameof(ShortUrl))]
    [Column("short_url_id")]
    public int ShortUrlId { get; set; }

    public ShortUrl ShortUrl { get; set; } = null!;
}