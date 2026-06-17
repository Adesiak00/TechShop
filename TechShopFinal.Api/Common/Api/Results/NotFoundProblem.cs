using System;
using Microsoft.AspNetCore.Http.HttpResults;

namespace TechShopFinal.Api.Common.Api.Results;

public static class NotFoundProblem
{
    public static ProblemHttpResult Create<TEntity>(Guid id)
    {
        return TypedResults.Problem(
            statusCode: StatusCodes.Status404NotFound,
            title: $"{typeof(TEntity).Name} not found",
            detail: $"The {typeof(TEntity).Name} with the id '{id}' was not found.");
    }
}
