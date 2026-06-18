using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using TechShopFinal.Api.Common.Api;
using TechShopFinal.Api.Data.Types;

namespace TechShopFinal.Api.Users.Endpoints;

public record UpdateUserRoleRequest(string RoleName);

public class UpdateUserRole : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/users/{id:guid}/role", HandleAsync)
            .WithTags("Users")
            .RequireAuthorization(policy => policy.RequireRole("Admin"));
    }

    private static async Task<Results<NoContent, NotFound, ProblemHttpResult>> HandleAsync(
        Guid id,
        UpdateUserRoleRequest request,
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null) 
        {
            return TypedResults.NotFound();
        }

        if (!await roleManager.RoleExistsAsync(request.RoleName))
        {
            return TypedResults.Problem("Podana rola nie istnieje.", statusCode: StatusCodes.Status400BadRequest);
        }

        var currentRoles = await userManager.GetRolesAsync(user);
        await userManager.RemoveFromRolesAsync(user, currentRoles);
        await userManager.AddToRoleAsync(user, request.RoleName);

        return TypedResults.NoContent();
    }
}