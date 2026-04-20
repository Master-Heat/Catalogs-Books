
using CatalogsBooksAPI.DTOs.AccountsDTOs;
using CatalogsBooksAPI.Models;
using CatalogsBooksAPI.Repository;
using Microsoft.Identity.Client;

namespace CatalogsBooksAPI.Services.Factories
{
    public class HomePageFactory
    {
        BooksRecsRepo booksRecsRepo;
        AccountRepo accountRepo;
        BooksRecsCardListFactory cardListFactory;

        public HomePageFactory(BooksRecsRepo booksRecsRepo,
        AccountRepo accountRepo,
        BooksRecsCardListFactory cardListFactory)
        {
            this.booksRecsRepo = booksRecsRepo;
            this.accountRepo = accountRepo;
            this.cardListFactory = cardListFactory;
        }

        public async Task<HomeDashboardDTO> GenerateHomeData(string token, string Email)

        {
            Account account = await accountRepo.GetAccountDataByEmail(Email);
            int accountid = account.AccountID;
            HomeDashboardDTO dashboard = new HomeDashboardDTO()
            {
                Token = token,
                Name = account.UserName,
                CategoryRecs = await cardListFactory.
                                    GenerateRecsList
                                    (
                                    accountid
                                    , booksRecsRepo.GetCategoryRecs
                                    ),

                AuthorRecs = await cardListFactory.
                                    GenerateRecsList
                                    (
                                    accountid,
                                    booksRecsRepo.GetAuthorRecs
                                    ),
                AuthorAndCategoryRecs = await cardListFactory
                                .GenerateRecsList
                                (
                                accountid,
                                booksRecsRepo.GetAuthorAndCategoryRecs
                                )


            };
            return dashboard;


        }

    }
}





