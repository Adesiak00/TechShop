using System;

namespace TechShopFinal.Api.Data.Types;

public class Category : IEntity
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public bool IsDeleted { get; set; } = false;
    public ICollection<Product> Products { get; set; } = [];

}
