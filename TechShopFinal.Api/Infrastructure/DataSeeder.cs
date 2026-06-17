using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TechShopFinal.Api.Data;
using TechShopFinal.Api.Data.Types;

namespace TechShopFinal.Api.Infrastructure;

public static class DataSeeder
{
    // ZMIANA: Dodajemy RoleManager do argumentów
    public static async Task SeedDataAsync(AppDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
    {
        // 1. Tworzenie podstawowych ról w systemie
        var roles = new[] { "Admin", "User" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }

        var adminEmail = "admin@techshop.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser is null)
        {
            adminUser = new AppUser
            {
                Id = Guid.NewGuid(),
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };
            
            await userManager.CreateAsync(adminUser, "Password123!");
        }

        // 2. Przypisanie roli Admin dla głównego konta
        if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }

        if (!await context.Categories.AnyAsync())
        {
            var laptops = new Category { Id = Guid.NewGuid(), Name = "Laptopy" };
            var phones = new Category { Id = Guid.NewGuid(), Name = "Smartfony" };
            var accessories = new Category { Id = Guid.NewGuid(), Name = "Akcesoria" };

            context.Categories.AddRange(laptops, phones, accessories);
            await context.SaveChangesAsync();

            if (!await context.Products.AnyAsync())
            {
                var product1 = new Product
                {
                    Id = Guid.NewGuid(),
                    Title = "MacBook Pro 16",
                    Description = "Potężny laptop dla profesjonalistów z układem Apple Silicon.",
                    Price = 14999.00m,
                    ImageUrl = "https://example.com/macbook.png",
                    CreationDate = DateTime.UtcNow,
                    CreatorUserId = adminUser.Id, 
                    Categories = [laptops]        
                };

                var product2 = new Product
                {
                    Id = Guid.NewGuid(),
                    Title = "iPhone 15 Pro",
                    Description = "Najnowszy smartfon od Apple z tytanową obudową.",
                    Price = 5999.00m,
                    ImageUrl = "https://example.com/iphone.png",
                    CreationDate = DateTime.UtcNow,
                    CreatorUserId = adminUser.Id,
                    Categories = [phones, accessories] 
                };

                context.Products.AddRange(product1, product2);
                await context.SaveChangesAsync();
            }
        }
    }
}