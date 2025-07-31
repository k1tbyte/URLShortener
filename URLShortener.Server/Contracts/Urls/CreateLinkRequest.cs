using System.ComponentModel.DataAnnotations;

namespace URLShortener.Server.Contracts.Urls;

public sealed class CreateLinkRequest
{
    [Required(ErrorMessage = "The URL is required")]
    [Url(ErrorMessage = "The URL is not valid")]
    [MaxLength(2048, ErrorMessage = "The URL must not exceed 2048 characters")]
    public string Url { get; set; } = string.Empty;
}