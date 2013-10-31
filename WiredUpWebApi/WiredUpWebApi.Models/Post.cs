using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace WiredUpWebApi.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string Content { get; set; }

        [Required]
        public DateTime PostDate { get; set; }

        public int PostedById { get; set; }

        public virtual User PostedBy { get; set; }
    }
}
