using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TechShopFinal.Api.Common.Api;
using TechShopFinal.Api.Common.Api.Extensions;
using TechShopFinal.Api.Data;
using TechShopFinal.Api.Data.Types;

namespace TechShopFinal.Api.Categories.Endpoints;

public record UpdateCategoryRequest(string Name);

public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryRequest>
{
    public UpdateCategoryValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}

public class UpdateCategory : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/categories/{id:guid}", HandleAsync)
            .WithTags("Categories")
            .WithRequestValidation<UpdateCategoryRequest>()
            .EnsureEntityExists<Category>();
    }

    private static async Task<NoContent> HandleAsync(
        Guid id,
        UpdateCategoryRequest request,
        AppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var category = await dbContext.Categories.FirstAsync(c => c.Id == id, cancellationToken);
        
        category.Name = request.Name;
        
        await dbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.NoContent();
    }
}