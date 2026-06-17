using TechShopFinal.Api.Common.Api.Filters;

namespace TechShopFinal.Api.Common.Api.Extensions;

public static class RouteHandlerBuilderValidationExtensions
{
    public static RouteHandlerBuilder WithRequestValidation<TRequest>(this RouteHandlerBuilder builder)
    {
        return builder.AddEndpointFilter<RequestValidationFilter<TRequest>>();
    }

    public static RouteHandlerBuilder EnsureEntityExists<TEntity>(this RouteHandlerBuilder builder) 
        where TEntity : class, Data.Types.IEntity
    {
        return builder.AddEndpointFilter<EnsureEntityExistsFilter<TEntity>>();
    }
}