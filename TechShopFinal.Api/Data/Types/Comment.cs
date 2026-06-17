using System;

namespace TechShopFinal.Api.Data.Types;

public class Comment: IEntity
{
    public Guid Id { get; set; }
    public required Guid ProductId { get; set; }
    public required string Description { get; set; }
    public DateTime CreationDate { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    public required Guid CreatorUserId { get; set; }  
    public Product Product { get; set; } = null!;

}
