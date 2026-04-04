using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using static Tehnicharche.GCommon.ApplicationConstants;

public class BanMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<BanMiddleware> logger;

    public BanMiddleware(RequestDelegate next, ILogger<BanMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.User?.Identity?.IsAuthenticated == true &&
            context.User.HasClaim(BannedClaimType, BannedClaimValue))
        {
            var userName = context.User.Identity.Name ?? "unknown";
            logger.LogWarning(
                "Banned user '{UserName}' attempted to access '{Path}'. Signing out.",
                userName, context.Request.Path);

            await context.SignOutAsync(IdentityConstants.ApplicationScheme);

            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "text/html; charset=utf-8";

            // Rendered inline to avoid redirect loops with the error-handler pipeline.
            await context.Response.WriteAsync(@"
                <div style=""text-align:center; margin-top:50px; font-family:Arial, sans-serif;"">
                    <h1 style=""color:#d32f2f;"">Your Account Has Been Banned</h1>
                    <p>Access to this account has been restricted due to a violation of our site policies.</p>
                    <p><strong>You have been logged out automatically.</strong></p>
                    <p>If you believe this is a mistake, please reach out to our support team.</p>
                    <p>
                        <a href=""/"" style=""margin-right:10px;"">Go to Home Page</a>
                        <a href=""/Contact"">Contact Support</a>
                    </p>
                </div>");

            return;
        }

        await next(context);
    }
}