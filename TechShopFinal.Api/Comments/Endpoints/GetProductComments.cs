using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TechShopFinal.Api.Common.Api;
using TechShopFinal.Api.Common.Api.Extensions;
using TechShopFinal.Api.Data;
using TechShopFinal.Api.Data.Types;

namespace TechShopFinal.Api.Comments.Endpoints;

public class GetProductComments : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/products/{productId:guid}/comments", HandleAsync)
            .WithTags("Comments")
            .EnsureEntityExists<Product>();
    }

    private static async Task<Ok<List<CommentResponse>>> HandleAsync(
        Guid productId,
        AppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var comments = await dbContext.Comments
            .AsNoTracking()
            .Where(c => c.ProductId == productId)
            .OrderByDescending(c => c.CreationDate)
            .Select(c => new CommentResponse(c.Id, c.ProductId, c.Description, c.CreationDate, c.CreatorUserId))
            .ToListAsync(cancellationToken);

        return TypedResults.Ok(comments);
    }
}