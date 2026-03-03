using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogsBooksAPI.Models.Config
{
    public class UserPreferedCategoryConfig : IEntityTypeConfiguration<UserPreferredCategory>
    {
        public void Configure(EntityTypeBuilder<UserPreferredCategory> builder)
        {
            builder.HasKey(upc => new { upc.AccountID, upc.Category, upc.SubCategory });


            // builder.HasIndex(b => new { b.Category, b.SubCategory })
            //    .IsUnique();

            builder.HasOne<Book>()
                .WithMany()
                .HasForeignKey(upc => new { upc.Category, upc.SubCategory })
                .HasPrincipalKey(b => new { b.Category, b.SubCategory });


        }
    }
}