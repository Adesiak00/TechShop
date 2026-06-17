using FluentValidation;

namespace TechShopFinal.Api.Common.Api.Filters;

public class RequestValidationFilter<TRequest> : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var validator = context.HttpContext.RequestServices.GetService<IValidator<TRequest>>();
        
        if (validator is not null)
        {
            var request = context.Arguments.OfType<TRequest>().FirstOrDefault();
            if (request is not null)
            {
                var validationResult = await validator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return TypedResults.ValidationProblem(validationResult.ToDictionary());
                }
            }
        }

        return await next(context);
    }
}