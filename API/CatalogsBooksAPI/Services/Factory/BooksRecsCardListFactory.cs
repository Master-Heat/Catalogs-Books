
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
        BookSearchRepo searchRepo;

        private readonly PasswordHasher<Account> _passwordHasher = new PasswordHasher<Account>();
        public BooksRecsCardListFactory(BooksRecsRepo booksRecsRepo,
                                        BookSearchRepo searchRepo)
        {
            this.booksRecsRepo = booksRecsRepo;
            this.searchRepo = searchRepo;

        }


        // 1. Search Logic: Used to check if they should redirect to Login




        // 2. Creation Logic

        public async Task<List<BookCardDTO>> GenerateBookCardFromBooks(List<Book> books)
        {
            if (books == null) return new List<BookCardDTO>();


            return [.. books.Select(b => new BookCardDTO
            {
                BookID = b.BookID,
                Title = b.Title,
                Description = b.Description,
                CoverImageLink = b.CoverImageLink,
                CoverAlt = b.CoverAlt,

            })];
        }


        public async Task<List<BookCardDTO>> GenerateRecsList(int accoutnID,
        Func<int, Task<List<Book>>> RepoMethod)
        {
            List<Book> books = await RepoMethod(accoutnID);

            var bookCards = await GenerateBookCardFromBooks(books);
            return bookCards;
        }
        public async Task<List<BookCardDTO>> GenerateGeneralRecsList(
        Func<Task<List<Book>>> RepoMethod)
        {
            List<Book> books = await RepoMethod();


            var bookCards = await GenerateBookCardFromBooks(books);
            return bookCards;
        }

        public async Task<List<BookCardDTO>> GenerateRelatedList(int bookid,
            Func<int, Task<List<Book>>> RepoMethod)
        {
            List<Book> books = await RepoMethod(bookid);


            var bookCards = await GenerateBookCardFromBooks(books);
            return bookCards;
        }

        public async Task<List<BookCardDTO>> SmartSearch(string keywork)
        {
            List<Book> BookResult = await searchRepo.GetSmartSearch(keywork);
            return await GenerateBookCardFromBooks(BookResult);
        }


    }
}