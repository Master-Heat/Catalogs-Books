using CatalogsBooksAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace CatalogsBooksAPI.Models.Config
{
    public class CategoryConfig : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.CategoryID);

            // When a Category is deleted, delete all Books in that category
            builder.HasMany(c => c.Books)
                   .WithOne(b => b.Category)
                   .HasForeignKey(b => b.CategoryID)
                   .OnDelete(DeleteBehavior.Cascade);

            // When a Category is deleted, remove it from users' preferences
            builder.HasMany(c => c.UserPreferredCategories)
                   .WithOne(upc => upc.Category)
                   .HasForeignKey(upc => upc.CategoryID)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}