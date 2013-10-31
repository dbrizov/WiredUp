using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using WiredUpWebApi.Models.Constants;

namespace WiredUpWebApi.Models
{
    public class UserPost
    {
        [Key]
        [Column("UserPostId")]
        public int Id { get; set; }

        [Required]
        [MaxLength(PostConstants.ContentMaxLength)]
        public string Content { get; set; }

        [Required]
        public DateTime PostDate { get; set; }

        public int UserId { get; set; }

        public virtual User User { get; set; }
    }
}
