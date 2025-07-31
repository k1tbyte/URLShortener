using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using URLShortener.Domain.Entities;
using URLShortener.Server.Contracts.Urls;
using URLShortener.Server.Tools;

namespace URLShortener.Server.Controllers;

[ApiController]
[Route(Constants.DefaultApiRoutePattern)]
public sealed class LinksController(IUrlsRepository repository) : ControllerBase
{
    [HttpGet]
    public IActionResult GetLinks()
    {
        int id = -1;
        var userId = User.Claims
            .FirstOrDefault(c => c.Type == "userid")?.Value;
        
        return Ok(repository.Set.Select(o => new
        {
            Id = o.Id,
            ShortUrl = o.ShortCode,
            Owned = o.CreatedBy == (int.TryParse(userId, out id) ? id : -1),
            OriginalUrl = o.OriginalUrl,
        }).ToList());
    }
    
    
    [HttpGet]
    [Authorize]
    public IActionResult GetLinkById(int id)
    {
        var link = repository.Set.Include(o => o.CreatedByUser).FirstOrDefault(x => x.Id == id);
        if (link is null)
        {
            return NotFound();
        }

        return Ok(new
        {
            Id = link.Id,
            ShortCode = link.ShortCode,
            OriginalUrl = link.OriginalUrl,
            CreatedByUsername = link.CreatedByUser.Username,
            CreatedAt = DateTimeOffset.FromUnixTimeSeconds(link.CreatedAt).UtcDateTime,
            Owned = link.CreatedBy == int.Parse(User.Claims
                .FirstOrDefault(c => c.Type == "userid")?.Value!),
            Clicks = link.UrlClicks
        });
    }

    [HttpGet("/short/{code}")]
    public IActionResult Link(string code)
    {
        Console.WriteLine($"Redirecting to original URL for code: {code}");
        return Redirect(repository.Set
            .Where(x => x.ShortCode == code)
            .Select(x => x.OriginalUrl)
            .FirstOrDefault() ?? "/");
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateLink([FromBody] CreateLinkRequest request)
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == "userid")?.Value!;
        

        // Simple hash using SHA256, take first 8 characters of hex
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(request.Url));
        var hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower(); // hex string
        var shortCode = hash.Substring(0, 8); // first 8 characters
        
        // Check if link already exists
        var existing = await repository.Set
            .FirstOrDefaultAsync(x => x.ShortCode == shortCode);

        if (existing != null)
        {
            return Conflict("Link already exists");
        }

        var newEntry = new ShortUrl
        {
            OriginalUrl = request.Url,
            ShortCode = shortCode,
            CreatedBy = int.Parse(userId),
        };

        await repository.WithAutoSave().Add(newEntry);

        return Ok(new { shortCode });
    }
    
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteLink(int id)
    {
        var link = await repository.Get(id);
        if (link is null)
        {
            return NotFound();
        }

        var isAdmin = User.IsInRole(EUserRole.Admin.ToString());
        if (!isAdmin && link.CreatedBy != int.Parse(User.Claims
                .FirstOrDefault(c => c.Type == "userid")?.Value!))
        {
            return Forbid();
        }

        await repository.WithAutoSave().Delete(link);
        return Ok();
    }
}