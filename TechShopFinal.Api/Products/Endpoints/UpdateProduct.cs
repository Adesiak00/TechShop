using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TechShopFinal.Api.Common.Api;
using TechShopFinal.Api.Common.Api.Extensions;
using TechShopFinal.Api.Data;
using TechShopFinal.Api.Data.Types;

namespace TechShopFinal.Api.Products.Endpoints;

public record UpdateProductRequest(string Title, string? Description, decimal Price, string? ImageUrl, List<Guid> CategoryIds);

public class UpdateProductValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.CategoryIds).NotNull();
    }
}

public class UpdateProduct : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/products/{id:guid}", HandleAsync)
            .WithTags("Products")
            .WithRequestValidation<UpdateProductRequest>()
            .EnsureEntityExists<Product>();
    }

    private static async Task<NoContent> HandleAsync(
        Guid id,
        UpdateProductRequest request,
        AppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var product = await dbContext.Products
            .Include(p => p.Categories) // Śledzimy relacje!
            .FirstAsync(p => p.Id == id, cancellationToken);

        // Aktualizacja prostych właściwości
        product.Title = request.Title;
        product.Description = request.Description;
        product.Price = request.Price;
        product.ImageUrl = request.ImageUrl;

        // Aktualizacja kategorii (Many-to-Many)
        var newCategories = await dbContext.Categories
            .Where(c => request.CategoryIds.Contains(c.Id))
            .ToListAsync(cancellationToken);

        product.Categories.Clear(); // Usuwa obecne powiązania
        foreach (var category in newCategories)
        {
            product.Categories.Add(category); // Dodaje nowe
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.NoContent();
    }
}