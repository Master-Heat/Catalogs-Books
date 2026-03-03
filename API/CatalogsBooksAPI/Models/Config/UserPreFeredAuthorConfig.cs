using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogsBooksAPI.Models.Config
{
    public class UserPreferedAuthorConfig : IEntityTypeConfiguration<UserPreferedAuthor>
    {
        public void Configure(EntityTypeBuilder<UserPreferedAuthor> builder)
        {
            builder.HasKey(upa => new { upa.AccountID, upa.AuthorName });

            builder.HasIndex(b => b.AuthorName).IsUnique();

            builder.HasOne<Book>().
                WithMany().
                HasForeignKey(upa => upa.AuthorName).
                HasPrincipalKey(b => b.AuthorName);
        }
    }
}