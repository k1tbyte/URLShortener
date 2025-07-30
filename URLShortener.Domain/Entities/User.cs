using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace URLShortener.Domain.Entities;

public enum EUserRole 
{
    None = 0,
    User = 1,
    Admin = 1
}

[Table("users")]
public sealed class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("username")]
    [MaxLength(32)]
    public required string Username { get; set; }
    
    [Column("password")]
    [MaxLength(128)]
    public required string Password { get; set; }
    
    [Column("role")]
    public EUserRole Role { get; set; } = EUserRole.User;

    [Column("joined_at")] 
    public long JoinedAt { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    
    public ICollection<Session> Sessions { get; set; } = [];
    public ICollection<ShortUrl> ShortUrls { get; set; } = [];

}