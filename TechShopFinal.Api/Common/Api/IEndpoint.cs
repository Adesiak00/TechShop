using System;

namespace TechShopFinal.Api.Common.Api;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}
