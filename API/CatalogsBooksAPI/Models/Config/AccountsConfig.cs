using CatalogsBooksAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace CatalogsBooksAPI.Models.Config
{
    public class AccountsConfig : IEntityTypeConfiguration<Accounts>
    {
        public void Configure(EntityTypeBuilder<Accounts> builder)
        {
            builder.HasKey(a => a.AccountID);


        }
    }
}
