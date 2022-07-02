using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Server.Data;

public class AppDataContext : DbContext
{
    public DbSet<Category> Categories { get; set; }
    public AppDataContext(DbContextOptions<AppDataContext> options): base(options) { }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        Category[] categoriesToSeed = new Category[3];

        for (int i = 1; i < 4; i++)
        {
            categoriesToSeed[i - 1] = new Category
            {
                CategoryId = i,
                ThumbnailImagePath = "uploads/placeholder.jpg",
                Name = $"Category {i}",
                Description = $"this is the description of the category {i}"
            };
        }

        modelBuilder.Entity<Category>().HasData(categoriesToSeed);
    }
}