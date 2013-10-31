using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WiredUpWebApi.Models.Constants;

namespace WiredUpWebApi.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(CompanyConstants.NameMaxLength)]
        public string Name { get; set; }

        [Required]
        [MaxLength(CompanyConstants.EmailMaxLength)]
        public string Email { get; set; }

        public string Description { get; set; }

        public virtual ICollection<User> Followers { get; set; }

        public virtual ICollection<CompanyPost> Posts { get; set; }
    }
}
