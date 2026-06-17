using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TechShopFinal.Api.Common.Api.Results;
using TechShopFinal.Api.Data;
using TechShopFinal.Api.Data.Types;

namespace TechShopFinal.Api.Common.Api.Filters;

public class EnsureEntityExistsFilter<TEntity>(AppDbContext dbContext) : IEndpointFilter where TEntity : class, IEntity
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (!context.HttpContext.Request.RouteValues.TryGetValue("id", out var idValue) || 
            !Guid.TryParse(idValue?.ToString(), out var id))
        {
            return TypedResults.Problem("Brak poprawnego ID w ścieżce lub ma niepoprawny format.", statusCode: StatusCodes.Status400BadRequest);
        }

        var exists = await dbContext.Set<TEntity>().AnyAsync(x => x.Id == id);
        if (!exists) 
        {
            return NotFoundProblem.Create<TEntity>(id);
        }

        return await next(context);
    }
}