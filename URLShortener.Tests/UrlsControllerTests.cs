/*using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;
using URLShortener.Domain.Entities;
using URLShortener.Infrastructure.Repositories.Abstraction;
using URLShortener.Server.Controllers;
using URLShortener.Server.Contracts.Urls;

namespace URLShortener.Tests
{
    public class LinksControllerTests
    {
        private readonly Mock<IUrlsRepository> _mockRepository;
        private readonly LinksController _controller;

        public LinksControllerTests()
        {
            _mockRepository = new Mock<IUrlsRepository>();
            _controller = new LinksController(_mockRepository.Object);
            
            // Setup controller context
            var httpContext = new DefaultHttpContext();
            var controllerContext = new ControllerContext(
                new ActionContext(
                    httpContext,
                    new RouteData(),
                    new ControllerActionDescriptor())
            );
            _controller.ControllerContext = controllerContext;
            
            // Setup default authenticated user
            SetupAuthenticatedUser(1, "testuser", "User");
        }

        private void SetupAuthenticatedUser(int userId, string username, string role)
        {
            var claims = new[] 
            { 
                new Claim("userid", userId.ToString()),
                new Claim("username", username),
                new Claim("role", role)
            };
            var identity = new ClaimsIdentity(claims, "Test");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext.HttpContext.User = claimsPrincipal;
        }

        [Fact]
        public void GetLinks_ReturnsOkResultWithLinks()
        {
            // Arrange
            var links = new List<ShortUrl>
            {
                new ShortUrl { Id = 1, ShortCode = "abc123", OriginalUrl = "https://example.com/1", CreatedBy = 1 },
                new ShortUrl { Id = 2, ShortCode = "def456", OriginalUrl = "https://example.com/2", CreatedBy = 2 }
            };

            var queryableLinks = links.AsQueryable();
            var mockSet = new Mock<Microsoft.EntityFrameworkCore.DbSet<ShortUrl>>();
            mockSet.As<IQueryable<ShortUrl>>().Setup(m => m.Provider).Returns(queryableLinks.Provider);
            mockSet.As<IQueryable<ShortUrl>>().Setup(m => m.Expression).Returns(queryableLinks.Expression);
            mockSet.As<IQueryable<ShortUrl>>().Setup(m => m.ElementType).Returns(queryableLinks.ElementType);
            mockSet.As<IQueryable<ShortUrl>>().Setup(m => m.GetEnumerator()).Returns(queryableLinks.GetEnumerator());

            _mockRepository.Setup(r => r.Set).Returns(mockSet.Object);

            // Act
            var result = _controller.GetLinks();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedLinks = Assert.IsAssignableFrom<List<object>>(okResult.Value);
            Assert.Equal(2, returnedLinks.Count);
        }

        [Fact]
        public void GetLinkById_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var linkId = 1;
            var user = new User { Id = 1, Username = "testuser", Password = "123" };
            var link = new ShortUrl 
            { 
                Id = linkId, 
                ShortCode = "abc123", 
                OriginalUrl = "https://example.com/original",
                CreatedBy = 1,
                CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                CreatedByUser = user,
                UrlClicks = new List<UrlClick>()
            };

            var queryableLinks = new List<ShortUrl> { link }.AsQueryable();
            var mockSet = new Mock<Microsoft.EntityFrameworkCore.DbSet<ShortUrl>>();
            mockSet.As<IQueryable<ShortUrl>>().Setup(m => m.Provider).Returns(queryableLinks.Provider);
            mockSet.As<IQueryable<ShortUrl>>().Setup(m => m.Expression).Returns(queryableLinks.Expression);
            mockSet.As<IQueryable<ShortUrl>>().Setup(m => m.ElementType).Returns(queryableLinks.ElementType);
            mockSet.As<IQueryable<ShortUrl>>().Setup(m => m.GetEnumerator()).Returns(queryableLinks.GetEnumerator());

            _mockRepository.Setup(r => r.Set).Returns(mockSet.Object);

            // Act
            var result = _controller.GetLinkById(linkId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic returnedLink = okResult.Value;
            Assert.Equal(linkId, returnedLink.Id);
            Assert.Equal(link.ShortCode, returnedLink.ShortCode);
            Assert.Equal(link.OriginalUrl, returnedLink.OriginalUrl);
            Assert.True(returnedLink.Owned);
        }

        [Fact]
        public void GetLinkById_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var linkId = 999;
            var queryableLinks = new List<ShortUrl>().AsQueryable();
            var mockSet = new Mock<Microsoft.EntityFrameworkCore.DbSet<ShortUrl>>();
            mockSet.As<IQueryable<ShortUrl>>().Setup(m => m.Provider).Returns(queryableLinks.Provider);
            mockSet.As<IQueryable<ShortUrl>>().Setup(m => m.Expression).Returns(queryableLinks.Expression);
            mockSet.As<IQueryable<ShortUrl>>().Setup(m => m.ElementType).Returns(queryableLinks.ElementType);
            mockSet.As<IQueryable<ShortUrl>>().Setup(m => m.GetEnumerator()).Returns(queryableLinks.GetEnumerator());

            _mockRepository.Setup(r => r.Set).Returns(mockSet.Object);

            // Act
            var result = _controller.GetLinkById(linkId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Link_WithValidCode_ReturnsRedirectResult()
        {
            // Arrange
            var shortCode = "abc123";
            var originalUrl = "https://example.com/original";
            var link = new ShortUrl { Id = 1, ShortCode = shortCode, OriginalUrl = originalUrl };

            var queryableLinks = new List<ShortUrl> { link }.AsQueryable();
            var mockSet = new Mock<Microsoft.EntityFrameworkCore.DbSet<ShortUrl>>();
            mockSet.As<IQueryable<ShortUrl>>().Setup(m => m.Provider).Returns(queryableLinks.Provider);
            mockSet.As<IQueryable<ShortUrl>>().Setup(m => m.Expression).Returns(queryableLinks.Expression);
            mockSet.As<IQueryable<ShortUrl>>().Setup(m => m.ElementType).Returns(queryableLinks.ElementType);
            mockSet.As<IQueryable<ShortUrl>>().Setup(m => m.GetEnumerator()).Returns(queryableLinks.GetEnumerator());

            _mockRepository.Setup(r => r.Set).Returns(mockSet.Object);

            // Act
            var result = _controller.Link(shortCode);

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal(originalUrl, redirectResult.Url);
        }

        [Fact]
        public void Link_WithInvalidCode_RedirectsToHome()
        {

            var shortCode = "nonexistent";
            var queryableLinks = new List<ShortUrl>().AsQueryable();
            var mockSet = new Mock<Microsoft.EntityFrameworkCore.DbSet<ShortUrl>>();
            mockSet.As<IQueryable<ShortUrl>>().Setup(m => m.Provider).Returns(queryableLinks.Provider);
            mockSet.As<IQueryable<ShortUrl>>().Setup(m => m.Expression).Returns(queryableLinks.Expression);
            mockSet.As<IQueryable<ShortUrl>>().Setup(m => m.ElementType).Returns(queryableLinks.ElementType);
            mockSet.As<IQueryable<ShortUrl>>().Setup(m => m.GetEnumerator()).Returns(queryableLinks.GetEnumerator());

            _mockRepository.Setup(r => r.Set).Returns(mockSet.Object);

            var result = _controller.Link(shortCode);

            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/", redirectResult.Url);
        }
        
        [Fact]
        public async Task CreateLink_WithExistingUrl_ReturnsConflict()
        {
            var request = new CreateLinkRequest { Url = "https://example.com/long-url" };
            var existingLink = new ShortUrl { Id = 1, ShortCode = "abc123", OriginalUrl = request.Url };


            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(request.Url));
            var hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            var shortCode = hash.Substring(0, 8);

            // Setup the repository to return the existing link
            var queryableLinks = new List<ShortUrl> { existingLink }.AsQueryable();
            var mockSet = new Mock<Microsoft.EntityFrameworkCore.DbSet<ShortUrl>>();
            mockSet.As<IQueryable<ShortUrl>>().Setup(m => m.Provider).Returns(queryableLinks.Provider);
            mockSet.As<IQueryable<ShortUrl>>().Setup(m => m.Expression).Returns(queryableLinks.Expression);
            mockSet.As<IQueryable<ShortUrl>>().Setup(m => m.ElementType).Returns(queryableLinks.ElementType);
            mockSet.As<IQueryable<ShortUrl>>().Setup(m => m.GetEnumerator()).Returns(queryableLinks.GetEnumerator());

            _mockRepository.Setup(r => r.Set).Returns(mockSet.Object);

            var result = await _controller.CreateLink(request);

            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal("Link already exists", conflictResult.Value);
        }
        
        [Fact]
        public async Task DeleteLink_NonExistentLink_ReturnsNotFound()
        {
            var linkId = 999;
            _mockRepository.Setup(r => r.Get(linkId)).ReturnsAsync((ShortUrl)null);

            var result = await _controller.DeleteLink(linkId);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}*/