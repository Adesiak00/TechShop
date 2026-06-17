using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TechShopFinal.Api.Common.Api;
using TechShopFinal.Api.Data;
using TechShopFinal.Api.Data.Types;
using TechShopFinal.Api.Infrastructure;

namespace TechShopFinal.Api;

public static class ConfigureServices
{
    public static void AddServices(this WebApplicationBuilder builder)
    {
        // 1. Rejestracja automatyczna endpointów z naszej architektury REPR
        builder.Services.AddEndpoints(typeof(ConfigureServices).Assembly);

        // 2. Baza danych SQLite
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

        // 3. Identity API, Autentykacja i Autoryzacja (To eliminuje Twój błąd!)
        builder.Services.AddAuthentication();
        builder.Services.AddAuthorization();

        builder.Services.AddIdentityApiEndpoints<AppUser>()
            .AddEntityFrameworkStores<AppDbContext>();

        // 4. Swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // 5. CORS dla frontendu
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy => 
                policy.WithOrigins("http://localhost:5173")
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials());
        });

        // 6. Globalna obsługa błędów
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();

        // 7. Rejestracja walidatorów z pakietu FluentValidation
        builder.Services.AddValidatorsFromAssemblyContaining<IEndpoint>();
    }
}