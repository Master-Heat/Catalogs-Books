using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogsBooksAPI.Models.Config
{
    public class UserPreferedAuthorConfig : IEntityTypeConfiguration<UserPreferedAuthor>
    {
        public void Configure(EntityTypeBuilder<UserPreferedAuthor> builder)
        {
            builder.HasKey(upa => new { upa.AuthorID, upa.AccountID });




        }
    }
}