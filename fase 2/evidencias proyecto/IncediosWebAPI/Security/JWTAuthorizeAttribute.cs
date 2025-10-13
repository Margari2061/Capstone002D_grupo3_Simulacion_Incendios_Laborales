using IncediosWebAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace IncediosWebAPI.Security;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class JWTAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        StringValues auth = context
            .HttpContext
            .Request
            .Headers
            .Authorization;

        if(auth.Count == 0 || auth[0] is null)
        {
            context.Result = new JsonResult(new { message = "Unauthorized" })
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };
            return;
        }

        string? token = auth[0]?
            .Split(" ")
            .Last();

        JwtSecurityTokenHandler handler = new();

        string? key = AppSettings.Instance.JWT;

        if (key is null)
        {
            context.Result = new JsonResult(new { message = "Unauthorized" })
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };
            return;
        }

        byte[] keyBuffer = Convert.FromBase64String(key);

        try
        {
            handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(keyBuffer),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken _);
        }
        catch
        {
            context.Result = new JsonResult(new { message = "Unauthorized" })
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };
            return;
        }
    }
}
