
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
        BookviewsRepo bookviewsRepo;

        public HomePageFactory(BooksRecsRepo booksRecsRepo,
        AccountRepo accountRepo,
        BookviewsRepo bookviewsRepo,
        BooksRecsCardListFactory cardListFactory)
        {
            this.booksRecsRepo = booksRecsRepo;
            this.accountRepo = accountRepo;
            this.cardListFactory = cardListFactory;
            this.bookviewsRepo = bookviewsRepo;
        }

        public async Task<HomeDashboardDTO> GenerateHomeData(string Email)

        {
            Account account = await accountRepo.GetAccountDataByEmail(Email);
            int accountid = account.AccountID;
            HomeDashboardDTO dashboard = new HomeDashboardDTO()
            {
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
                                ),

                PopularThisWeek = await cardListFactory
                .GenerateGeneralRecsList(bookviewsRepo.GetPopulatThisWeek),

                PopularAllTime = await cardListFactory
                .GenerateGeneralRecsList(bookviewsRepo.GetPopularAllTime)


            };
            return dashboard;
        }

        public async Task<ChartDTO> GenerateChartsData()
        {

            ChartDTO chart = new ChartDTO()
            {
                HighestRate = await
                cardListFactory.GenerateGeneralRecsList(booksRecsRepo.GetBookByHighestRate),

                PopularAllTime = await
                cardListFactory.GenerateGeneralRecsList(bookviewsRepo.GetPopularAllTime),

                PopularThisWeek = await
                cardListFactory.GenerateGeneralRecsList(bookviewsRepo.GetPopulatThisWeek)
            };
            return chart;
        }

    }
}





