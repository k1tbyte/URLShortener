using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace URLShortener.Domain.Entities;

[Table("short_urls")]
public sealed class ShortUrl
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("original_url")]
    public required string OriginalUrl { get; set; }
    
    [Column("short_code")]
    public required string ShortCode { get; set; } 
    
    [Column("created_at")]
    public long CreatedAt { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    
    [ForeignKey(nameof(CreatedByUser))]
    [Column("created_by")]
    public int CreatedBy { get; set; }
    
    public User CreatedByUser { get; set; } = null!;
    public ICollection<UrlClick> UrlClicks { get; set; } = [];
}