using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TechShopFinal.Api.Authentication;
using TechShopFinal.Api.Common.Api;
using TechShopFinal.Api.Common.Api.Extensions;
using TechShopFinal.Api.Data;
using TechShopFinal.Api.Data.Types;

namespace TechShopFinal.Api.Products.Endpoints;

public record CategoryDto(Guid Id, string Name);
public record ProductResponse(Guid Id, string Title, string? Description, decimal Price, string? ImageUrl, DateTime CreationDate, Guid CreatorUserId, List<CategoryDto> Categories);

public record CreateProductRequest(string Title, string? Description, decimal Price, string? ImageUrl, List<Guid> CategoryIds);

public class CreateProductValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.CategoryIds).NotNull();
    }
}

public class CreateProduct : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/products", HandleAsync)
            .WithTags("Products")
            .RequireAuthorization() 
            .WithRequestValidation<CreateProductRequest>();
    }

    private static async Task<Created<ProductResponse>> HandleAsync(
        CreateProductRequest request,
        ClaimsPrincipal user, 
        AppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var categories = await dbContext.Categories
            .Where(c => request.CategoryIds.Contains(c.Id))
            .ToListAsync(cancellationToken);

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Price = request.Price,
            ImageUrl = request.ImageUrl,
            CreatorUserId = user.GetUserId(),
            CreationDate = DateTime.UtcNow,
            Categories = categories
        };

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(cancellationToken);

        var response = new ProductResponse(
            product.Id, product.Title, product.Description, product.Price, 
            product.ImageUrl, product.CreationDate, product.CreatorUserId, 
            product.Categories.Select(c => new CategoryDto(c.Id, c.Name)).ToList());

        return TypedResults.Created($"/api/products/{product.Id}", response);
    }
}