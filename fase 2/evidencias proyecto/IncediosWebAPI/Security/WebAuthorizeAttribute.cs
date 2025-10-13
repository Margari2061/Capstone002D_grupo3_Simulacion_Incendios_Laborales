using IncediosWebAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace IncediosWebAPI.Security;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class WebAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    private readonly AppRoles _roles;

    public WebAuthorizeAttribute(AppRoles roles)
    {
        _roles = roles;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context.HttpContext.User.Identity == null)
        {
            context.Result = new RedirectToActionResult("index", "auth", null); 
            return;
        }

        if (!context.HttpContext.User.Identity.IsAuthenticated)
        {
            context.Result = new RedirectToActionResult("index", "auth", null);
            return;
        }

        if (_roles == 0)
            return;

        ClaimsIdentity[] identities = context
            .HttpContext
            .User
            .Identities
            .ToArray();

        List<string> claims = [];

        foreach (ClaimsIdentity identity in identities) 
        {
            Claim[] chunck = identity
                .Claims
                .Where(c => c.Type == "role")
                .ToArray();

            foreach(Claim claim in chunck)
                if(!claims.Contains(claim.Value))
                    claims.Add(claim.Value);
        }

        AppRoles current = AppRoles.None;

        foreach (string claim in claims)
        {
            switch (claim)
            {
                case "Player":
                    current |= AppRoles.Player;
                    break;
                case "Admin":
                    current |= AppRoles.Admin;
                    break;
                default:
                    break;
            }
        }

        if (!current.HasFlag(_roles))
        {
            context.Result = new RedirectToActionResult("index", "auth", null);
            return;
        }

    }
}
