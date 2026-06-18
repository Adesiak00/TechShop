using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TechShopFinal.Api.Common.Api;
using TechShopFinal.Api.Data;

namespace TechShopFinal.Api.Categories.Endpoints;

public class GetCategories : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/categories", HandleAsync)
            .WithTags("Categories");
    }

    private static async Task<Ok<List<CategoryResponse>>> HandleAsync(
        AppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var categories = await dbContext.Categories
            .AsNoTracking()
            .Select(c => new CategoryResponse(c.Id, c.Name))
            .ToListAsync(cancellationToken);

        return TypedResults.Ok(categories);
    }
}