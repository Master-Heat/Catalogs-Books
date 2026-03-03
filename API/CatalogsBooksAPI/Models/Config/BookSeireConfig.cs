using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogsBooksAPI.Models.Config
{
    public class BookSeireConfig : IEntityTypeConfiguration<BookSeire>
    {
        public void Configure(EntityTypeBuilder<BookSeire> builder)
        {

        }
    }

}