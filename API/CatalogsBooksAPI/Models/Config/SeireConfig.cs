using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogsBooksAPI.Models.Config
{
    public class SeireConfig : IEntityTypeConfiguration<Seire>
    {
        public void Configure(EntityTypeBuilder<Seire> builder)
        {

        }
    }

}