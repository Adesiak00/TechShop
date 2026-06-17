using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TechShopFinal.Api.Authentication;
using TechShopFinal.Api.Common.Api;
using TechShopFinal.Api.Common.Api.Extensions;
using TechShopFinal.Api.Data;
using TechShopFinal.Api.Data.Types;

namespace TechShopFinal.Api.Comments.Endpoints;

public record CommentResponse(Guid Id, Guid ProductId, string Description, DateTime CreationDate, Guid CreatorUserId);

public record CreateCommentRequest(Guid ProductId, string Description);

public class CreateCommentValidator : AbstractValidator<CreateCommentRequest>
{
    public CreateCommentValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
    }
}

public class CreateComment : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/comments", HandleAsync)
            .WithTags("Comments")
            .RequireAuthorization()
            .WithRequestValidation<CreateCommentRequest>();
    }

    private static async Task<Results<Created<CommentResponse>, NotFound>> HandleAsync(
        CreateCommentRequest request,
        ClaimsPrincipal user,
        AppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        // Sprawdzamy ręcznie czy produkt istnieje, ponieważ ID idzie w ciele żądania (DTO), a nie w URL
        var productExists = await dbContext.Products.AnyAsync(p => p.Id == request.ProductId, cancellationToken);
        if (!productExists)
        {
            return TypedResults.NotFound();
        }

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            ProductId = request.ProductId,
            Description = request.Description,
            CreationDate = DateTime.UtcNow,
            CreatorUserId = user.GetUserId()
        };

        dbContext.Comments.Add(comment);
        await dbContext.SaveChangesAsync(cancellationToken);

        var response = new CommentResponse(comment.Id, comment.ProductId, comment.Description, comment.CreationDate, comment.CreatorUserId);
        
        return TypedResults.Created($"/api/comments/{comment.Id}", response);
    }
}