using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Blog.Models
{
    public class Comment
    {
        public Comment()
        {
            this.Date = DateTime.Now;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Name *")]
        public string Name { get; set; }
        
        [MaxLength(50)]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Comment *")]
        public string VisitorComment { get; set; }

        public DateTime Date { get; set; }

        [Required]
        [ForeignKey("Article")]
        public int ArticleId { get; set; }

        public virtual Article Article { get; set; }
    }
}