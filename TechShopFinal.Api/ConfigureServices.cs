using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TechShopFinal.Api.Common.Api;
using TechShopFinal.Api.Data;
using TechShopFinal.Api.Data.Types;
using TechShopFinal.Api.Infrastructure;
using TechShopFinal.Api.Data.Interceptors;

namespace TechShopFinal.Api;

public static class ConfigureServices
{
    public static void AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpoints(typeof(ConfigureServices).Assembly);

        builder.Services.AddSingleton<SoftDeleteInterceptor>();
        
        builder.Services.AddDbContext<AppDbContext>((sp, options) =>
        {
            var interceptor = sp.GetRequiredService<SoftDeleteInterceptor>();
            
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
                   .AddInterceptors(interceptor);
        });

        builder.Services.AddAuthentication();
        builder.Services.AddAuthorization();

        builder.Services.AddIdentityApiEndpoints<AppUser>()
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AppDbContext>();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy => 
                policy.WithOrigins("http://localhost:5173")
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials());
        });

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();

        builder.Services.AddValidatorsFromAssemblyContaining<IEndpoint>();
    }
}