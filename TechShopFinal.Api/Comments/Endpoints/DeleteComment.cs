using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TechShopFinal.Api.Common.Api;
using TechShopFinal.Api.Common.Api.Extensions;
using TechShopFinal.Api.Data;
using TechShopFinal.Api.Data.Types;

namespace TechShopFinal.Api.Comments.Endpoints;

public class DeleteComment : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/comments/{id:guid}", HandleAsync)
            .WithTags("Comments")
            .EnsureEntityExists<Comment>();
    }

    private static async Task<NoContent> HandleAsync(
        Guid id,
        AppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var comment = await dbContext.Comments.FirstAsync(c => c.Id == id, cancellationToken);
        
        comment.IsDeleted = true;
        
        await dbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.NoContent();
    }
}