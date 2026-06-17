using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TechShopFinal.Api.Authentication; // Używamy Twojego rozszerzenia!
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

        var userId = context.HttpContext.User.GetUserId();
        if (userId == Guid.Empty)
        {
            return TypedResults.Problem("No authorization. Please log in.", statusCode: StatusCodes.Status401Unauthorized);
        }

        var isOwner = await dbContext.Set<TEntity>()
            .AnyAsync(x => x.Id == id && EF.Property<Guid>(x, "CreatorUserId") == userId);

        if (!isOwner)
        {
            return TypedResults.Problem("You are not the owner of this resource.", statusCode: StatusCodes.Status403Forbidden);
        }

        return await next(context);
    }
}