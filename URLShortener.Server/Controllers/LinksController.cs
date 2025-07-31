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
        var link = repository.Set.FirstOrDefault(x => x.Id == id);
        if (link is null)
        {
            return NotFound();
        }
        
        return Ok(link);
    }

    [HttpGet("/link/{code}")]
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
        var userId = User.Claims
            .FirstOrDefault(c => c.Type == "userid")?.Value!;
        
        await repository.WithAutoSave().Add(new ShortUrl
        {
            OriginalUrl = request.Url,
            ShortCode = Guid.NewGuid().ToString("N").Substring(0, 8),
            CreatedBy = int.Parse(userId),
        });

        return Ok();
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