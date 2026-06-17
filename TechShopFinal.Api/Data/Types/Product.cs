using System;

namespace TechShopFinal.Api.Data.Types;

public class Product : IEntity
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreationDate { get; set; } = DateTime.UtcNow;
    public required Guid CreatorUserId { get; set; }
    public string? ImageUrl { get; set; }
    public ICollection<Comment> Comments { get; set; } = [];
    public ICollection<Category> Categories { get; set; } = [];

}
