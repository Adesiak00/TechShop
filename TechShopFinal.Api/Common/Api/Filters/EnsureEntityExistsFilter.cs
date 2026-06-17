using Microsoft.EntityFrameworkCore;
using TechShopFinal.Api.Data;
using TechShopFinal.Api.Data.Types;
using TechShopFinal.Api.Common.Api.Results;

namespace TechShopFinal.Api.Common.Api.Filters;

public class EnsureEntityExistsFilter<TEntity> : IEndpointFilter where TEntity : class, IEntity
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var id = context.Arguments.OfType<Guid>().FirstOrDefault();
        
        if (id != Guid.Empty)
        {
            var dbContext = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
            var exists = await dbContext.Set<TEntity>().AnyAsync(x => x.Id == id);

            if (!exists)
            {
                return NotFoundProblem.Create<TEntity>(id);
            }
        }

        return await next(context);
    }
}