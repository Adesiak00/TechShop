using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TechShopFinal.Api.Common.Api;
using TechShopFinal.Api.Common.Api.Extensions;
using TechShopFinal.Api.Data;
using TechShopFinal.Api.Data.Types;

namespace TechShopFinal.Api.Categories.Endpoints;

public class GetCategoryById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/categories/{id:guid}", HandleAsync)
            .WithTags("Categories")
            .EnsureEntityExists<Category>();
    }

    private static async Task<Ok<CategoryResponse>> HandleAsync(
        Guid id,
        AppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var category = await dbContext.Categories
            .AsNoTracking()
            .FirstAsync(c => c.Id == id, cancellationToken);

        return TypedResults.Ok(new CategoryResponse(category.Id, category.Name));
    }
}