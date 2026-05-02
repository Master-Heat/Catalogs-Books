

using CatalogsBooksAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace CatalogsBooksAPI.Models.Config
{
    public class AuthorConfig : IEntityTypeConfiguration<Author>
    {
        public void Configure(EntityTypeBuilder<Author> builder)
        {
            builder.HasKey(a => a.AuthorID);

            // When an Author is deleted, delete all their Books[cite: 8, 10]
            builder.HasMany(a => a.Books)
                   .WithOne(b => b.Author)
                   .HasForeignKey(b => b.AuthorID)
                   .OnDelete(DeleteBehavior.Cascade);

            // When an Author is deleted, remove them from users' favorites[cite: 9, 10]
            builder.HasMany(a => a.UserPreferedAuthor)
                   .WithOne(upa => upa.Author)
                   .HasForeignKey(upa => upa.AuthorID)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}