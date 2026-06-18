using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using TechShopFinal.Api.Common.Api;

namespace TechShopFinal.Api.Authentication.Endpoints;

public record RoleResponse(string Role);

public class GetRole : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/auth/role", HandleAsync)
            .WithTags("Auth")
            .RequireAuthorization();
    }

    private static Ok<RoleResponse> HandleAsync(ClaimsPrincipal user)
    {
        var role = user.FindFirst(ClaimTypes.Role)?.Value ?? "User";
        
        return TypedResults.Ok(new RoleResponse(role));
    }
}