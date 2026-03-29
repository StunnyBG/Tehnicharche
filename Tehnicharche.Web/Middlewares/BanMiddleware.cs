using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

public class BanMiddleware
{
    private readonly RequestDelegate next;

    public BanMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            if (context.User.HasClaim("Banned", "true"))
            {
                await context.SignOutAsync(IdentityConstants.ApplicationScheme);

                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "text/html";

                // the banned page is rendered directly in the middleware insted of redirecting because there were issues with redirect loops and 404 errors
                await context.Response.WriteAsync(@"
                    <div style=""text-align:center; margin-top:50px; font-family:Arial, sans-serif;"">
                    <h1 style=""color:#d32f2f;"">Your Account Has Been Banned</h1>
                    <p>Access to this account has been restricted due to a violation of our site policies.</p>
                    <p><strong>You have been logged out automatically.</strong></p>
                    <p>If you believe this is a mistake, please reach out to our support team for assistance.</p>
                    <p>
                        <a href=""/"" style=""margin-right:10px;"">Go to Home Page</a>
                        <a href=""/Contact"">Contact Support</a>
                    </p>
                </div>
                ");
                return;
            }
        }

        await next(context);
    }
}