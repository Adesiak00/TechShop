using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TechShopFinal.Api.Common.Api;
using TechShopFinal.Api.Common.Api.Extensions;
using TechShopFinal.Api.Common.Pagination;
using TechShopFinal.Api.Data;
using TechShopFinal.Api.Data.Types;

namespace TechShopFinal.Api.Comments.Endpoints;

public class GetProductComments : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        // 1. ZMIANA: {productId:guid} -> {id:guid}
        app.MapGet("/api/products/{id:guid}/comments", HandleAsync)
            .WithTags("Comments")
            .EnsureEntityExists<Product>();
    }

    private static async Task<Ok<PagedResult<CommentResponse>>> HandleAsync(
        Guid id, // 2. ZMIANA: argument zmieniony z productId na id
        [AsParameters] PagedRequest pagedRequest,
        AppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Comments
            .AsNoTracking()
            .Where(c => c.ProductId == id) // 3. ZMIANA: filtrowanie po zmiennej id
            .OrderByDescending(c => c.CreationDate)
            .Select(c => new CommentResponse(c.Id, c.ProductId, c.Description, c.CreationDate, c.CreatorUserId));

        var pagedResult = await query.ToPagedResultAsync(
            pagedRequest.PageNumber, 
            pagedRequest.PageSize, 
            cancellationToken);

        return TypedResults.Ok(pagedResult);
    }
}