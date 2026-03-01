using CatalogsBooksAPI.Models.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Options;
namespace CatalogsBooksAPI.Models
{
    public class CatalogsBooksContext : DbContext
    {
        public CatalogsBooksContext(DbContextOptions<CatalogsBooksContext> options) : base(options)
        {

        }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Book> Books { get; set; }




        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            //  optionsBuilder.UseLazyLoadingProxies();

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration<Account>(new AccountConfig());
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AccountConfig).Assembly);

        }
    }
}