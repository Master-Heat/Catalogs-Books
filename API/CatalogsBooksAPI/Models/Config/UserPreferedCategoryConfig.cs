using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogsBooksAPI.Models.Config
{
    public class UserPreferedCategoryConfig : IEntityTypeConfiguration<UserPreferedCategory>
    {
        public void Configure(EntityTypeBuilder<UserPreferedCategory> builder)
        {
            builder.HasKey(upc => new { upc.AccountID, upc.Category, upc.SubCategory });

            builder.HasOne<Book>()
        .WithMany()
        .HasForeignKey(upc => new { upc.Category, upc.SubCategory })
        .HasPrincipalKey(b => new { b.Category, b.SubCategory });
        }
    }
}