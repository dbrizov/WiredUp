using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using WiredUpWebApi.Models.Constants;

namespace WiredUpWebApi.Models
{
    public class Company
    {
        [Key]
        [Column("CompanyId")]
        public int Id { get; set; }

        [Required]
        [MaxLength(CompanyConstants.NameMaxLength)]
        public string Name { get; set; }

        [Required]
        [MaxLength(CompanyConstants.EmailMaxLength)]
        public string Email { get; set; }

        [MaxLength(CompanyConstants.DescriptionMaxLength)]
        public string Description { get; set; }

        public virtual ICollection<User> Followers { get; set; }

        public virtual ICollection<CompanyPost> Posts { get; set; }

        public Company()
        {
            this.Followers = new HashSet<User>();
            this.Posts = new HashSet<CompanyPost>();
        }
    }
}
