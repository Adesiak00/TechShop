using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TechShopFinal.Api.Common.Api;
using TechShopFinal.Api.Data.Types;

namespace TechShopFinal.Api.Users.Endpoints;

public record UserResponse(Guid Id, string Email, IList<string> Roles);

public class GetUsers : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/users", HandleAsync)
            .WithTags("Users")
            .RequireAuthorization(policy => policy.RequireRole("Admin")); // Zabezpieczenie na poziomie roli
    }

    private static async Task<Ok<List<UserResponse>>> HandleAsync(
        UserManager<AppUser> userManager,
        CancellationToken cancellationToken)
    {
        var users = await userManager.Users.ToListAsync(cancellationToken);
        var response = new List<UserResponse>();

        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
            response.Add(new UserResponse(user.Id, user.Email!, roles));
        }

        return TypedResults.Ok(response);
    }
}