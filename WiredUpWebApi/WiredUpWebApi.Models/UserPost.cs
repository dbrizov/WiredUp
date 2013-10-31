using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace WiredUpWebApi.Models
{
    public class UserPost
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string Content { get; set; }

        [Required]
        public DateTime PostDate { get; set; }

        public int UserId { get; set; }

        public virtual User User { get; set; }
    }
}
