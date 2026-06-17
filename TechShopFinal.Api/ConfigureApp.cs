using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechShopFinal.Api.Data;
using TechShopFinal.Api.Data.Types;
using TechShopFinal.Api.Infrastructure;

namespace TechShopFinal.Api;

public static class ConfigureApp
{
    public static async Task UseAppAsync(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // 2. Globalna obsługa wyjątków (zawsze na początku potoku)
        app.UseExceptionHandler(); 
        
        // 3. CORS (przed autoryzacją)
        app.UseCors("AllowFrontend");

       // 4. Autentykacja i Autoryzacja (Kolejność ma znaczenie!)
        app.UseAuthentication();
        app.UseAuthorization();

        app.Use(async (context, next) =>
        {
            await next();

            if ((context.Response.StatusCode == StatusCodes.Status401Unauthorized || 
                 context.Response.StatusCode == StatusCodes.Status403Forbidden) && 
                !context.Response.HasStarted)
            {
                var problemDetailsService = context.RequestServices.GetRequiredService<IProblemDetailsService>();
                
                context.Response.ContentType = "application/problem+json";
                
                await problemDetailsService.WriteAsync(new ProblemDetailsContext
                {
                    HttpContext = context,
                    ProblemDetails = new ProblemDetails
                    {
                        Status = context.Response.StatusCode,
                        Title = context.Response.StatusCode == StatusCodes.Status401Unauthorized ? "Unauthorized" : "Forbidden",
                        Detail = context.Response.StatusCode == StatusCodes.Status401Unauthorized 
                            ? "Brak prawidłowego tokenu uwierzytelniającego lub Twój token wygasł." 
                            : "Nie masz wystarczających uprawnień do wykonania tej operacji."
                    }
                });
            }
        });

        app.MapEndpoints();
        app.MapGroup("/api/auth").MapIdentityApi<AppUser>();

        // 6. Uruchomienie migracji i Seedera asynchronicznie podczas startu aplikacji
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

        await context.Database.MigrateAsync(); 
        await DataSeeder.SeedDataAsync(context, userManager); 
    }
}