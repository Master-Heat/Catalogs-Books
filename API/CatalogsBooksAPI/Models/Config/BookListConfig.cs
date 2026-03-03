using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogsBooksAPI.Models.Config
{
    public class BookListConfig : IEntityTypeConfiguration<BookList>
    {
        public void Configure(EntityTypeBuilder<BookList> builder)
        {
            builder.HasKey(a => new { a.BookID, a.ListID });
        }
    }
}