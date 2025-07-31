using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;
using URLShortener.Infrastructure.Lib;
using URLShortener.Infrastructure.Services;
using URLShortener.Server.Controllers;
using URLShortener.Server.Contracts.Authentication;
using System.Text.Json;

namespace URLShortener.Tests
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _controller = new AuthController(_mockAuthService.Object);
            
            // Setup controller context
            var httpContext = new DefaultHttpContext();
            var controllerContext = new ControllerContext(
                new ActionContext(
                    httpContext,
                    new RouteData(),
                    new ControllerActionDescriptor())
            );
            _controller.ControllerContext = controllerContext;
        }

        [Fact]
        public async Task Register_WithValidCredentials_ReturnsOkResult()
        {
            var request = new AuthRequest
            {
                Login = "testuser",
                Password = "password123"
            };

            var tokens = new AuthResult("test_access_token", Guid.NewGuid().ToString());
            
            _mockAuthService
                .Setup(x => x.RegisterAsync(request.Login, request.Password))
                .Returns(Task.CompletedTask);
            
            _mockAuthService
                .Setup(x => x.LoginAsync(request.Login, request.Password))
                .ReturnsAsync(tokens);
            
            var result = await _controller.Register(request);
            
            var okResult = Assert.IsType<OkObjectResult>(result);
            
            // Properly convert the anonymous object to a concrete type
            var responseJson = JsonSerializer.Serialize(okResult.Value);
            var response = JsonSerializer.Deserialize<TokenResponse>(responseJson);
            
            Assert.Equal(tokens.AccessToken, response.AccessToken);
            Assert.Equal(tokens.RefreshToken, response.RefreshToken);
        }

        [Fact]
        public async Task Register_WithExistingUsername_ReturnsStatusCode()
        {
            var request = new AuthRequest
            {
                Login = "existinguser",
                Password = "password123"
            };

            _mockAuthService
                .Setup(x => x.RegisterAsync(request.Login, request.Password))
                .ThrowsAsync(new ApiException("Username already exists", StatusCodes.Status409Conflict));
            
            var result = await _controller.Register(request);
            
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status409Conflict, statusCodeResult.StatusCode);
            Assert.Equal("Username already exists", statusCodeResult.Value);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOkResult()
        {
            var request = new AuthRequest
            {
                Login = "testuser",
                Password = "password123"
            };

            var tokens = new AuthResult("test_access_token", Guid.NewGuid().ToString());
            
            _mockAuthService
                .Setup(x => x.LoginAsync(request.Login, request.Password))
                .ReturnsAsync(tokens);
            
            var result = await _controller.Login(request);
            
            var okResult = Assert.IsType<OkObjectResult>(result);
            
            // Properly convert the anonymous object to a concrete type
            var responseJson = JsonSerializer.Serialize(okResult.Value);
            var response = JsonSerializer.Deserialize<TokenResponse>(responseJson);
            
            Assert.Equal(tokens.AccessToken, response.AccessToken);
            Assert.Equal(tokens.RefreshToken, response.RefreshToken);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            var request = new AuthRequest
            {
                Login = "wronguser",
                Password = "wrongpassword"
            };

            _mockAuthService
                .Setup(x => x.LoginAsync(request.Login, request.Password))
                .ThrowsAsync(new ApiException("Invalid username or password", StatusCodes.Status401Unauthorized));
            
            var result = await _controller.Login(request);
            
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status401Unauthorized, statusCodeResult.StatusCode);
            Assert.Equal("Invalid username or password", statusCodeResult.Value);
        }

        [Fact]
        public async Task RefreshSession_WithValidTokens_ReturnsOkResult()
        {
            var request = new RefreshRequest
            {
                AccessToken = "old_access_token",
                RefreshToken = Guid.NewGuid()
            };

            var tokens = new AuthResult("new_access_token", Guid.NewGuid().ToString());
            
            _mockAuthService
                .Setup(x => x.RefreshSessionAsync(request.AccessToken, request.RefreshToken))
                .ReturnsAsync(tokens);
            
            var result = await _controller.RefreshSession(request);
            
            var okResult = Assert.IsType<OkObjectResult>(result);
            
            // Properly convert the anonymous object to a concrete type
            var responseJson = JsonSerializer.Serialize(okResult.Value);
            var response = JsonSerializer.Deserialize<TokenResponse>(responseJson);
            
            Assert.Equal(tokens.AccessToken, response.AccessToken);
            Assert.Equal(tokens.RefreshToken, response.RefreshToken);
        }

        [Fact]
        public async Task RefreshSession_WithInvalidTokens_ReturnsUnauthorized()
        {
            var request = new RefreshRequest
            {
                AccessToken = "invalid_access_token",
                RefreshToken = Guid.NewGuid()
            };

            _mockAuthService
                .Setup(x => x.RefreshSessionAsync(request.AccessToken, request.RefreshToken))
                .ThrowsAsync(new ApiException("Invalid or expired token", StatusCodes.Status401Unauthorized));
            
            var result = await _controller.RefreshSession(request);
            
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status401Unauthorized, statusCodeResult.StatusCode);
            Assert.Equal("Invalid or expired token", statusCodeResult.Value);
        }

        [Fact]
        public async Task Logout_WhenAuthorized_ReturnsNoContentResult()
        {
            var claims = new[] { new Claim("userid", "1") };
            var identity = new ClaimsIdentity(claims, "Test");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext.HttpContext.User = claimsPrincipal;
    
            var request = new RefreshRequest
            {
                AccessToken = "access_token",
                RefreshToken = Guid.NewGuid()
            };
            var result = await _controller.Logout(request);
    
            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(StatusCodes.Status204NoContent, noContentResult.StatusCode);
        }
        
        private class TokenResponse
        {
            public string AccessToken { get; set; }
            public string RefreshToken { get; set; }
        }
    }
}