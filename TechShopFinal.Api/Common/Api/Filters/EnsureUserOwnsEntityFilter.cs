using Microsoft.EntityFrameworkCore;
using TechShopFinal.Api.Authentication;
using TechShopFinal.Api.Data;
using TechShopFinal.Api.Data.Types;

namespace TechShopFinal.Api.Common.Api.Filters;
public interface IOwnedEntity : IEntity
{
    Guid CreatorUserId { get; }
}

public class EnsureUserOwnsEntityFilter<TEntity> : IEndpointFilter where TEntity : class, IOwnedEntity
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var id = context.Arguments.OfType<Guid>().FirstOrDefault();
        
        if (id != Guid.Empty)
        {
            var userId = context.HttpContext.User.GetUserId();
            var dbContext = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
            
            // Szukamy encji
            var entity = await dbContext.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

            if (entity is not null && entity.CreatorUserId != userId)
            {
                return TypedResults.Problem(
                    statusCode: StatusCodes.Status403Forbidden,
                    title: "Forbidden",
                    detail: "You do not have permission to modify this resource."
                );
            }
        }

        return await next(context);
    }
}