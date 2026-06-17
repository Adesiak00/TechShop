namespace TechShopFinal.Api.Common.Pagination;

public record PagedRequest(int PageNumber = 1, int PageSize = 10);