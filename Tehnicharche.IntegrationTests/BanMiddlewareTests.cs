using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Security.Claims;
using static Tehnicharche.GCommon.ApplicationConstants;

namespace Tehnicharche.Tests.Integration
{
    [TestFixture]
    public class BanMiddlewareTests
    {
        private Mock<ILogger<BanMiddleware>> logger = null!;

        [SetUp]
        public void SetUp()
        {
            logger = new Mock<ILogger<BanMiddleware>>();
        }

        // helpers
        private static DefaultHttpContext BuildContext(
            IEnumerable<Claim> claims,
            bool isAuthenticated,
            out Mock<IAuthenticationService> authService)
        {
            var identity = isAuthenticated ? new ClaimsIdentity(claims, "TestScheme") : new ClaimsIdentity();

            var principal = new ClaimsPrincipal(identity);

            authService = new Mock<IAuthenticationService>();
            authService
                .Setup(a => a.SignOutAsync(
                    It.IsAny<HttpContext>(),
                    It.IsAny<string?>(),
                    It.IsAny<AuthenticationProperties?>()))
                .Returns(Task.CompletedTask);

            var services = new Mock<IServiceProvider>();
            services
                .Setup(s => s.GetService(typeof(IAuthenticationService)))
                .Returns(authService.Object);

            var context = new DefaultHttpContext
            {
                User = principal,
                RequestServices = services.Object
            };

            context.Response.Body = new MemoryStream();

            return context;
        }

        private static async Task<string> ReadResponseBodyAsync(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            return await new StreamReader(response.Body).ReadToEndAsync();
        }


        [Test]
        public async Task BannedUser_RequestIsTerminated_NextDelegateIsNotCalled()
        {
            var nextCalled = false;
            RequestDelegate next = _ =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            };

            var middleware = new BanMiddleware(next, logger.Object);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "banned_user"),
                new Claim(BannedClaimType, BannedClaimValue)
            };
            var context = BuildContext(claims, isAuthenticated: true, out _);

            await middleware.Invoke(context);

            Assert.That(nextCalled, Is.False);
        }

        [Test]
        public async Task BannedUser_SignOutIsCalled()
        {
            RequestDelegate next = _ => Task.CompletedTask;
            var middleware = new BanMiddleware(next, logger.Object);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "banned_user"),
                new Claim(BannedClaimType, BannedClaimValue)
            };
            var context = BuildContext(claims, isAuthenticated: true, out var authService);

            await middleware.Invoke(context);

            authService.Verify(
                a => a.SignOutAsync(
                    context,
                    IdentityConstants.ApplicationScheme,
                    null),
                Times.Once);
        }

        [Test]
        public async Task BannedUser_ResponseStatusIs403()
        {
            RequestDelegate next = _ => Task.CompletedTask;
            var middleware = new BanMiddleware(next, logger.Object);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "banned_user"),
                new Claim(BannedClaimType, BannedClaimValue)
            };
            var context = BuildContext(claims, isAuthenticated: true, out _);

            await middleware.Invoke(context);

            Assert.That(context.Response.StatusCode, Is.EqualTo(StatusCodes.Status403Forbidden));
        }

        [Test]
        public async Task BannedUser_ResponseBodyContainsBannedMessage()
        {
            RequestDelegate next = _ => Task.CompletedTask;
            var middleware = new BanMiddleware(next, logger.Object);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "banned_user"),
                new Claim(BannedClaimType, BannedClaimValue)
            };
            var context = BuildContext(claims, isAuthenticated: true, out _);

            await middleware.Invoke(context);

            var body = await ReadResponseBodyAsync(context.Response);
            Assert.That(body, Does.Contain("Banned").Or.Contain("banned"));
        }

        [Test]
        public async Task BannedUser_ContentTypeIsHtml()
        {
            RequestDelegate next = _ => Task.CompletedTask;
            var middleware = new BanMiddleware(next, logger.Object);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "banned_user"),
                new Claim(BannedClaimType, BannedClaimValue)
            };
            var context = BuildContext(claims, isAuthenticated: true, out _);

            await middleware.Invoke(context);

            Assert.That(context.Response.ContentType, Does.Contain("text/html"));
        }


        [Test]
        public async Task AuthenticatedNonBannedUser_NextDelegateIsCalled()
        {
            var nextCalled = false;
            RequestDelegate next = _ =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            };
            var middleware = new BanMiddleware(next, logger.Object);

            var claims = new[] { new Claim(ClaimTypes.Name, "normal_user") };
            var context = BuildContext(claims, isAuthenticated: true, out var authService);

            await middleware.Invoke(context);

            Assert.That(nextCalled, Is.True);
            authService.Verify(
                a => a.SignOutAsync(It.IsAny<HttpContext>(), It.IsAny<string?>(), It.IsAny<AuthenticationProperties?>()),
                Times.Never);
        }

        [Test]
        public async Task AuthenticatedNonBannedUser_ResponseStatusRemainsDefault()
        {
            RequestDelegate next = ctx =>
            {
                ctx.Response.StatusCode = 200;
                return Task.CompletedTask;
            };
            var middleware = new BanMiddleware(next, logger.Object);

            var claims = new[] { new Claim(ClaimTypes.Name, "normal_user") };
            var context = BuildContext(claims, isAuthenticated: true, out _);

            await middleware.Invoke(context);

            Assert.That(context.Response.StatusCode, Is.EqualTo(200));
        }


        [Test]
        public async Task AnonymousUser_NextDelegateIsCalled()
        {
            var nextCalled = false;
            RequestDelegate next = _ =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            };
            var middleware = new BanMiddleware(next, logger.Object);

            var context = BuildContext(Array.Empty<Claim>(), isAuthenticated: false, out var authService);

            await middleware.Invoke(context);

            Assert.That(nextCalled, Is.True);
            authService.Verify(
                a => a.SignOutAsync(It.IsAny<HttpContext>(), It.IsAny<string?>(), It.IsAny<AuthenticationProperties?>()),
                Times.Never);
        }


        [Test]
        public async Task UnauthenticatedContextWithBannedClaim_NextIsCalled()
        {
            var nextCalled = false;
            RequestDelegate next = _ =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            };
            var middleware = new BanMiddleware(next, logger.Object);

            var claims = new[] { new Claim(BannedClaimType, BannedClaimValue) };
            var context = BuildContext(claims, isAuthenticated: false, out _);

            await middleware.Invoke(context);

            Assert.That(nextCalled, Is.True);
        }
    }
}