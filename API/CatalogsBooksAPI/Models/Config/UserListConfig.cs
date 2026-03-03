using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogsBooksAPI.Models.Config
{
    public class UserListConfig : IEntityTypeConfiguration<UserList>
    {
        public void Configure(EntityTypeBuilder<UserList> builder)
        {


        }
    }
}