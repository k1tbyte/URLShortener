using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using URLShortener.Server.Tools;

namespace URLShortener.Server.Controllers;

[ApiController]
[Route(Constants.DefaultApiRoutePattern)]
public sealed class LinksController(IHttpContextAccessor accessor) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public IActionResult GetLinks()
    {
        var s = accessor.HttpContext;
        // Logic to retrieve and return links
        return Ok(new
        {
            Links = "List of links"
        });
    }
}