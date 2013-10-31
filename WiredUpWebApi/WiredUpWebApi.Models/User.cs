using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WiredUpWebApi.Models.Constants;

namespace WiredUpWebApi.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(UserConstants.FirstNameMaxLength)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(UserConstants.LastNameMaxLength)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(UserConstants.EmailMaxLength)]
        public string Email { get; set; }

        [Required]
        [MinLength(UserConstants.PasswordMinLength)]
        [MaxLength(UserConstants.PasswordMaxLength)]
        public string Password { get; set; }

        public byte[] Photo { get; set; }

        public int AddressId { get; set; }

        public virtual Address Address { get; set; }

        [MaxLength(UserConstants.LanguagesMaxLength)]
        public string Languages { get; set; }

        public virtual ICollection<User> Connections { get; set; }

        public virtual ICollection<Company> Followings { get; set; }

        public virtual ICollection<Project> Projects { get; set; }

        public virtual ICollection<Certificate> Certificates { get; set; }

        public virtual ICollection<Skill> Skills { get; set; }

        public virtual ICollection<Message> Messages { get; set; }

        public virtual ICollection<UserPost> Posts { get; set; }
    }
}
