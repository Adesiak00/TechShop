using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TechShopFinal.Api.Common.Api;
using TechShopFinal.Api.Common.Pagination; // Dodany namespace
using TechShopFinal.Api.Data;

namespace TechShopFinal.Api.Products.Endpoints;

public class GetProducts : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/products", HandleAsync)
            .WithTags("Products");
    }

    private static async Task<Ok<PagedResult<ProductResponse>>> HandleAsync(
        [AsParameters] PagedRequest pagedRequest, 
        AppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Products
            .AsNoTracking()
            .Include(p => p.Categories)
            .OrderByDescending(p => p.CreationDate)
            .Select(p => new ProductResponse(
                p.Id, p.Title, p.Description, p.Price, p.ImageUrl, 
                p.CreationDate, p.CreatorUserId, 
                p.Categories.Select(c => c.Id).ToList()));

        var pagedResult = await query.ToPagedResultAsync(
            pagedRequest.PageNumber, 
            pagedRequest.PageSize, 
            cancellationToken);

        return TypedResults.Ok(pagedResult);
    }
}