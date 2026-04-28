
using CatalogsBooksAPI.DTOs.AccountsDTOs;
using CatalogsBooksAPI.DTOs.BooksDTOs;
using CatalogsBooksAPI.Models;
using CatalogsBooksAPI.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace CatalogsBooksAPI.Services.Factories
{






    public class BooksRecsCardListFactory
    {
        BooksRecsRepo booksRecsRepo;

        private readonly PasswordHasher<Account> _passwordHasher = new PasswordHasher<Account>();
        public BooksRecsCardListFactory(BooksRecsRepo booksRecsRepo)
        {
            this.booksRecsRepo = booksRecsRepo;
        }


        // 1. Search Logic: Used to check if they should redirect to Login


        // 2. Creation Logic
        public async Task<List<BookCardDTO>> GenerateRecsList(int accoutnID,
        Func<int, Task<List<Book>>> RepoMethod)
        {
            List<Book> books = await RepoMethod(accoutnID);

            var bookCards = books.Select(b => new BookCardDTO
            {
                BookID = b.BookID,
                Title = b.Title,
                Description = b.Description,
                CoverImageLink = b.CoverImageLink,
                CoverAlt = b.CoverAlt,

                // Set these to null/default as requested for now
                MainCategory = null,
                Sbucategory = null,
                ViewsCount = 0
            }).ToList();
            return bookCards;
        }
    }
}