using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace URLShortener.Domain.Entities;

[Table("sessions")]
public sealed class Session
{
    [Key]
    [Column("refresh_token")]
    public required Guid Token { get; init; }
        
    [Column("expires_in")]
    public required long ExpiresIn { get; init; }
        
    [Column("user_id")]
    public required int UserId { get; init; } 
    
    public User User { get; set; }
}