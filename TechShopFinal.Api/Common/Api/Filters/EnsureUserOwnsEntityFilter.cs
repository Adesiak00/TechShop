using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TechShopFinal.Api.Data;
using TechShopFinal.Api.Data.Types;

namespace TechShopFinal.Api.Common.Api.Filters;

public class EnsureUserOwnsEntityFilter<TEntity>(AppDbContext dbContext) : IEndpointFilter where TEntity : class, IEntity
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (!context.HttpContext.Request.RouteValues.TryGetValue("id", out var idValue) || 
            !Guid.TryParse(idValue?.ToString(), out var id))
        {
             return TypedResults.Problem("Invalid ID format.", statusCode: StatusCodes.Status400BadRequest);
        }

        var userId = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return TypedResults.Problem("Unauthorized, login required.", statusCode: StatusCodes.Status401Unauthorized);
        }

        var isOwner = await dbContext.Set<TEntity>()
            .AnyAsync(x => x.Id == id && EF.Property<string>(x, "UserId") == userId);

        if (!isOwner)
        {
            return TypedResults.Problem("Unauthorized, you are not the owner of this resource.", statusCode: StatusCodes.Status403Forbidden);
        }

        return await next(context);
    }
}