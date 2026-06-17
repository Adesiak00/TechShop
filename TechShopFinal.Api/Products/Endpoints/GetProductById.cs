using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TechShopFinal.Api.Common.Api;
using TechShopFinal.Api.Common.Api.Extensions;
using TechShopFinal.Api.Data;
using TechShopFinal.Api.Data.Types;

namespace TechShopFinal.Api.Products.Endpoints;

public class GetProductById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/products/{id:guid}", HandleAsync)
            .WithTags("Products")
            .EnsureEntityExists<Product>();
    }

    private static async Task<Ok<ProductResponse>> HandleAsync(
        Guid id,
        AppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var product = await dbContext.Products
            .AsNoTracking()
            .Include(p => p.Categories)
            .FirstAsync(p => p.Id == id, cancellationToken);

        var response = new ProductResponse(
            product.Id, product.Title, product.Description, product.Price, 
            product.ImageUrl, product.CreationDate, product.CreatorUserId, 
            product.Categories.Select(c => c.Id).ToList());

        return TypedResults.Ok(response);
    }
}