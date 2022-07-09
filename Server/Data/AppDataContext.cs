using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Server.Data;

public class AppDataContext : DbContext
{
    // add the model we needs to the entity 
    public DbSet<Category> Categories { get; set; }
    public DbSet<Post> Posts { get; set; }
    public AppDataContext(DbContextOptions<AppDataContext> options): base(options) { }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region Category

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

        #endregion


        #region Posts seed

        modelBuilder.Entity<Post>(
            entity =>
            {
                entity.HasOne(post => post.Category)
                    .WithMany(category => category.Posts)
                    .HasForeignKey("CategoryId");
            });
        Post[] postsToSeed = new Post[6];

        for (int i = 1; i < 7; i++)
        {
            string postTitle = string.Empty;
            int categoryId = 0;

            switch (i)
            {
                case 1:
                    postTitle = "First post";
                    categoryId = 1;
                    break;
                case 2:
                    postTitle = "Second post";
                    categoryId = 2;
                    break;
                case 3:
                    postTitle = "Third post";
                    categoryId = 3;
                    break;
                case 4:
                    postTitle = "Fourth post";
                    categoryId = 1;
                    break;
                case 5:
                    postTitle = "Fifth post";
                    categoryId = 2;
                    break;
                case 6:
                    postTitle = "Sixth post";
                    categoryId = 3;
                    break;
                default:
                    break;
            }

            postsToSeed[i - 1] = new Post
            {
                PostId = i,
                ThumbnailImagePath = "uploads/placeholder.jpg",
                Title = postTitle,
                Excerpt = $"This is the excerpt for post {i}. An excerpt is a little extraction from a larger piece of text. Sort of like a preview.",
                Content = string.Empty,
                PublishDate = DateTime.UtcNow.ToString("dd/MM/yyyy hh:mm"),
                IsPublished = true,
                Author = "XSama",
                CategoryId = categoryId
            };
        }

        modelBuilder.Entity<Post>().HasData(postsToSeed);

        #endregion
    }
}