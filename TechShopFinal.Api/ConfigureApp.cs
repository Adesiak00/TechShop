using Microsoft.AspNetCore.Identity;
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

        // 5. Mapowanie Endpointów aplikacji i Identity
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