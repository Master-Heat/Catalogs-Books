using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogsBooksAPI.Models.Config
{
    public class UserPreferedCategoryConfig : IEntityTypeConfiguration<UserPreferredCategory>
    {
        public void Configure(EntityTypeBuilder<UserPreferredCategory> builder)
        {
            builder.HasKey(upc => new { upc.AccountID, upc.CategoryID });


            builder.HasOne(upa => upa.Account)
             .WithMany()
             .HasForeignKey(upa => upa.AccountID);

        }
    }
}