using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using TechShopFinal.Api.Common.Api;
using TechShopFinal.Api.Common.Api.Extensions;
using TechShopFinal.Api.Data;
using TechShopFinal.Api.Data.Types;

namespace TechShopFinal.Api.Categories.Endpoints;

public record CreateCategoryRequest(string Name);
public record CategoryResponse(Guid Id, string Name);

public class CreateCategoryValidator : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}

public class CreateCategory : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/categories", HandleAsync)
            .WithTags("Categories")
            .WithRequestValidation<CreateCategoryRequest>();
    }

    private static async Task<Created<CategoryResponse>> HandleAsync(
        CreateCategoryRequest request,
        AppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = request.Name
        };

        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync(cancellationToken);

        var response = new CategoryResponse(category.Id, category.Name);
        return TypedResults.Created($"/api/categories/{category.Id}", response);
    }
}