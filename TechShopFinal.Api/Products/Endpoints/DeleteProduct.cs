using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TechShopFinal.Api.Common.Api;
using TechShopFinal.Api.Common.Api.Extensions;
using TechShopFinal.Api.Data;
using TechShopFinal.Api.Data.Types;

namespace TechShopFinal.Api.Products.Endpoints;

public class DeleteProduct : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/products/{id:guid}", HandleAsync)
            .WithTags("Products")
            .EnsureEntityExists<Product>();
    }

    private static async Task<NoContent> HandleAsync(
        Guid id,
        AppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var product = await dbContext.Products.FirstAsync(p => p.Id == id, cancellationToken);
        
        product.IsDeleted = true;
        
        await dbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.NoContent();
    }
}