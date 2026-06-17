using System;

namespace TechShopFinal.Api.Data.Types;

public interface IEntity
{
    Guid Id { get; set; }
    bool IsDeleted { get; set; }
}
