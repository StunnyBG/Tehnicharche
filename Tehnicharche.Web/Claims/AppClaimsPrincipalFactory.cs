using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Tehnicharche.Data.Models;

public class AppClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser>
{
    public AppClaimsPrincipalFactory(UserManager<ApplicationUser> userManager, IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, optionsAccessor) { }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);

        var roles = await UserManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            identity.AddClaim(new Claim(ClaimTypes.Role, role));
        }

        if (user.IsBanned)
        {
            identity.AddClaim(new Claim("Banned", "true"));
        }

        return identity;
    }
}