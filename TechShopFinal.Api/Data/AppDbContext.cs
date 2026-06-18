using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TechShopFinal.Api.Data.Types;

namespace TechShopFinal.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) 
    : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Comment> Comments => Set<Comment>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Category>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<Product>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<Comment>().HasQueryFilter(x => !x.IsDeleted);

        if (Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite")
        {
            builder.Entity<Product>()
            .Property(p => p.Price)
            .HasConversion<double>();
        }
    }
    
    
}