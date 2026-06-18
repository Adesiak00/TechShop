using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TechShopFinal.Api.Common.Api;
using TechShopFinal.Api.Common.Api.Extensions;
using TechShopFinal.Api.Common.Pagination;
using TechShopFinal.Api.Data;

namespace TechShopFinal.Api.Products.Endpoints;

// 1. ZMIANA: Nowy dedykowany rekord żądania (zamiast samego PagedRequest)
public record GetProductsRequest(
    string? SearchTerm,
    Guid? CategoryId,
    string? SortColumn,
    string? SortOrder,
    int PageNumber = 1,
    int PageSize = 10
);

public class GetProducts : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/products", HandleAsync)
            .WithTags("Products");
    }

    private static async Task<Ok<PagedResult<ProductResponse>>> HandleAsync(
        [AsParameters] GetProductsRequest request, 
        AppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Products
            .AsNoTracking()
            .Include(p => p.Categories)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(p => 
                p.Title.ToLower().Contains(searchTerm) || 
                (p.Description != null && p.Description.ToLower().Contains(searchTerm)));
        }

        if (request.CategoryId.HasValue)
        {
            query = query.Where(p => p.Categories.Any(c => c.Id == request.CategoryId.Value));
        }

        query = request.SortColumn?.ToLower() switch
        {
            "price" => request.SortOrder?.ToLower() == "desc" 
                ? query.OrderByDescending(p => p.Price) 
                : query.OrderBy(p => p.Price),
                
            "title" => request.SortOrder?.ToLower() == "desc" 
                ? query.OrderByDescending(p => p.Title) 
                : query.OrderBy(p => p.Title),
                
            _ => request.SortOrder?.ToLower() == "asc" 
                ? query.OrderBy(p => p.CreationDate)
                : query.OrderByDescending(p => p.CreationDate)
        };

        var mappedQuery = query.Select(p => new ProductResponse(
            p.Id, p.Title, p.Description, p.Price, p.ImageUrl, 
            p.CreationDate, p.CreatorUserId, 
            p.Categories.Select(c => new CategoryDto(c.Id, c.Name)).ToList()));

        var pagedResult = await mappedQuery.ToPagedResultAsync(
            request.PageNumber, 
            request.PageSize, 
            cancellationToken);

        return TypedResults.Ok(pagedResult);
    }
}