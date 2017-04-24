using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Blog.Models
{
    public class ArticleViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [DisplayName("Title *")]
        public string Title { get; set; }
        
        [Required]
        [DisplayName("Content *")]
        public string Content { get; set; }

        public string AuthorId { get; set; }

        [DisplayName("Category *")]
        public int CategoryId { get; set; }

        public ICollection<Article> Articles { get; set; }

        public List<Category> Categories { get; set; }

        [Required]
        [DisplayName("Tags *")]
        public string Tags { get; set; }
    }
}