using System.Security.Claims;

namespace TechShopFinal.Api.Authentication;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var userIdString = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        
        return Guid.TryParse(userIdString, out var userId) ? userId : Guid.Empty;
    }
}