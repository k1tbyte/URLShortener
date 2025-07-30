using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using URLShortener.Infrastructure.Lib;
using URLShortener.Infrastructure.Services;
using URLShortener.Server.Contracts.Authentication;
using URLShortener.Server.Tools;

namespace URLShortener.Server.Controllers;

[ApiController]
[Route(Constants.DefaultApiRoutePattern)]
public sealed class AuthController(AuthService authService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Register([FromBody] AuthRequest request)
    {
        try
        {
            await authService.RegisterAsync(request.Login, request.Password);
            var tokens = await authService.LoginAsync(request.Login, request.Password);
            return Ok(new
            {
                tokens.AccessToken, tokens.RefreshToken
            });
        }
        catch (ApiException e)
        {
            return StatusCode(e.StatusCode, e.Message);
        }

        return Ok();
    }
    
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] AuthRequest request)
    {
        try
        {
            var tokens = await authService.LoginAsync(request.Login, request.Password);
            return Ok(new
            {
                tokens.AccessToken, tokens.RefreshToken
            });
        }
        catch (ApiException e)
        {
            return StatusCode(e.StatusCode, e.Message);
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> RefreshSession([FromBody] RefreshRequest request)
    {
        try
        {
            var tokens = await authService.RefreshSessionAsync(request.AccessToken, request.RefreshToken);
            return Ok(new
            {
                tokens.AccessToken, tokens.RefreshToken
            });
        }
        catch (ApiException e)
        {
            return StatusCode(e.StatusCode, e.Message);
        }
    }
    
   [HttpPost]
   [Authorize]
    public IActionResult Logout()
    {
        // Logic for user logout
        return Ok("Logout successful.");
    }
}