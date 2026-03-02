using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace CatalogsBooksAPI.Models
{
    public class Book
    {
        [Key]
        public int BookID { get; set; }
        public int? AuthorID { get; set; }
        // this will be used only if the auther is user in our website
        [Required]
        public string Title { get; set; }
        [Required]
        public string AuthorName { get; set; }
        // this is the normal case for books are from other website
        public DateOnly PublicationDate { get; set; }
        public bool CanDownload { get; set; }
        public string DownloadLink { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string CoverImageLink { get; set; }
        public string CoverAlt { get; set; }
        // this will be used to create defualt alt in the db for cover image in image tag
        public int PagesCount { get; set; }


        // todo : for relation you musd add these vitual properties 
        /*
        A) for many to one relationships one nullable variable from class
            1- bookseries
            2- accounts 
                        /? notice to put this above accounts [ForeignKey("AuthorID")] 
        B) for one to many attributes nullable lists of
            1- ListItems
            2- Reviews
            3- booksviewed 
            4- accounts ( authors)
            5- UserPreferedAuthors
            6- UserPreferedCategories
        */

    }
}