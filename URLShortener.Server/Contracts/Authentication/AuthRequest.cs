using System.ComponentModel.DataAnnotations;

namespace URLShortener.Server.Contracts.Authentication;

public sealed class AuthRequest
{
    [MaxLength(30, ErrorMessage = "Login cannot exceed 30 characters")]
    [MinLength(3, ErrorMessage = "Login must be at least 3 characters long")]
    public required string Login { get; set; }
    
    [MaxLength(128, ErrorMessage = "Password cannot exceed 128 characters")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
    [Required(ErrorMessage = "Password is required")]
    public required string Password { get; set; }
}

public sealed class RefreshRequest
{
    public required string AccessToken { get; set; }
    public required Guid RefreshToken { get; set; }
}