using CatalogsBooksAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace CatalogsBooksAPI.Models.Config
{
    public class BookConfig : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.HasKey(a => a.BookID);

            // Cascade delete for Reviews
            builder.HasMany(b => b.Reviews)
                   .WithOne(r => r.Book)
                   .HasForeignKey(r => r.BookID)
                   .OnDelete(DeleteBehavior.Cascade);

            // Cascade delete for Series
            builder.HasOne(b => b.Series)
                   .WithOne(s => s.Books)
                   .HasForeignKey<Series>(s => s.BookID)
                   .OnDelete(DeleteBehavior.Cascade);

            // Cascade delete for BookList items
            builder.HasMany(b => b.ListItems)
                   .WithOne(l => l.Book)
                   .HasForeignKey(l => l.BookID)
                   .OnDelete(DeleteBehavior.Cascade);

            // Cascade delete for ViewedBooks
            builder.HasMany(b => b.ViewedBooks)
                   .WithOne()
                   .HasForeignKey(v => v.BookID)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
