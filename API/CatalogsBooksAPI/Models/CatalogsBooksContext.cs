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
        public DbSet<Accounts> Accounts { get; set; }
        public DbSet<Books> Books { get; set; }




        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            //  optionsBuilder.UseLazyLoadingProxies();

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration<Accounts>(new AccountsConfig());
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AccountsConfig).Assembly);

        }
    }
}