using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TechShopFinal.Api.Common.Api;
using TechShopFinal.Api.Common.Api.Extensions;
using TechShopFinal.Api.Data;
using TechShopFinal.Api.Data.Types;

namespace TechShopFinal.Api.Comments.Endpoints;

public class GetCommentById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/comments/{id:guid}", HandleAsync)
            .WithTags("Comments")
            .EnsureEntityExists<Comment>();
    }

    private static async Task<Ok<CommentResponse>> HandleAsync(
        Guid id,
        AppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var comment = await dbContext.Comments
            .AsNoTracking()
            .FirstAsync(c => c.Id == id, cancellationToken);

        return TypedResults.Ok(new CommentResponse(comment.Id, comment.ProductId, comment.Description, comment.CreationDate, comment.CreatorUserId));
    }
}