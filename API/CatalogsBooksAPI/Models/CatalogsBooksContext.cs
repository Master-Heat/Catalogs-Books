using CatalogsBooksAPI.Models.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Options;
namespace CatalogsBooksAPI.Models
{
    public class CatalogsBooksContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<BookList> BookLists { get; set; }
        public DbSet<UserList> UserLists { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ViewedBook> ViewedBooks { get; set; }
        public DbSet<BookSeire> BookSeires { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<UserPreferedAuthor> UserPreferedAuthors { get; set; }
        public DbSet<UserPreferredCategory> UserPreferedCategories { get; set; }

        public CatalogsBooksContext(DbContextOptions<CatalogsBooksContext> options) : base(options)
        {

        }

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