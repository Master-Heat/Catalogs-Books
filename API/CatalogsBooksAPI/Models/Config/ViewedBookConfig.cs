using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogsBooksAPI.Models.Config
{
    public class ViewedBookConfig : IEntityTypeConfiguration<ViewedBook>
    {
        public void Configure(EntityTypeBuilder<ViewedBook> builder)
        {
            builder.HasKey(a => new { a.AccountID, a.BookID });
        }
    }

}