using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TechShopFinal.Api.Common.Api;
using TechShopFinal.Api.Common.Api.Extensions;
using TechShopFinal.Api.Common.Api.Filters;
using TechShopFinal.Api.Data;
using TechShopFinal.Api.Data.Types;

namespace TechShopFinal.Api.Comments.Endpoints;

public record UpdateCommentRequest(string Description);

public class UpdateCommentValidator : AbstractValidator<UpdateCommentRequest>
{
    public UpdateCommentValidator()
    {
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
    }
}

public class UpdateComment : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/comments/{id:guid}", HandleAsync)
            .WithTags("Comments")
            .WithRequestValidation<UpdateCommentRequest>()
            .EnsureEntityExists<Comment>()
            .AddEndpointFilter<EnsureUserOwnsEntityFilter<Comment>>();
    }

    private static async Task<NoContent> HandleAsync(
        Guid id,
        UpdateCommentRequest request,
        AppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var comment = await dbContext.Comments.FirstAsync(c => c.Id == id, cancellationToken);
        
        comment.Description = request.Description;
        
        await dbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.NoContent();
    }
}