using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace CatalogsBooksAPI.Models
{
    public class Book
    {
        [Key]
        public int BookID { get; set; }
        public int AuthorID { get; set; }

        // this will be used only if the auther is user in our website
        [Required]
        public string Title { get; set; }


        public DateOnly? PublicationDate { get; set; }
        public bool CanDownload { get; set; }
        // this variable just to tell if the book avilable on our website or not 
        // use it to tell that download link is from external website 
        public string DownloadLink { get; set; }
        public string Description { get; set; }
        public int CategoryID { get; set; }
        public string CoverImageLink { get; set; }
        public string CoverAlt { get; set; }
        // this will be used to create defualt alt in the db for cover image in image tag
        public int PagesCount { get; set; }


        [JsonIgnore]
        [ForeignKey("AuthorID")]
        virtual public Author Author { get; set; }

        [JsonIgnore]
        virtual public Series Series { get; set; }



        [JsonIgnore]
        virtual public List<BookList> ListItems { get; set; }
        [JsonIgnore]
        virtual public List<Review> Reviews { get; set; }
        [JsonIgnore]
        virtual public List<ViewedBook> ViewedBooks { get; set; }
        //  [JsonIgnore]
        //  virtual public List<Account> Accounts { get; set; }
        [JsonIgnore]
        virtual public List<UserPreferedAuthor> UserPreferedAuthors { get; set; }
        [JsonIgnore]
        virtual public List<UserPreferredCategory> UserPreferedCategories { get; set; }

        [ForeignKey("CategoryID")]
        virtual public Category Category { get; set; }

    }
}