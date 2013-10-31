using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using WiredUpWebApi.Models.Constants;

namespace WiredUpWebApi.Models
{
    public class User
    {
        [Key]
        [Column("UserId")]
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
        [MinLength(UserConstants.AuthCodeMinLength)]
        [MaxLength(UserConstants.AuthCodeMaxLength)]
        // Sha1 encrypted password
        public string AuthCode { get; set; }

        [MaxLength(UserConstants.SessionKeyMaxLength)]
        public string SessionKey { get; set; }

        public byte[] Photo { get; set; }

        public int? CountryId { get; set; }

        public virtual Country Country { get; set; }

        [MaxLength(UserConstants.LanguagesMaxLength)]
        public string Languages { get; set; }

        public virtual ICollection<User> Connections { get; set; }

        public virtual ICollection<Company> Followings { get; set; }

        public virtual ICollection<Project> Projects { get; set; }

        public virtual ICollection<Certificate> Certificates { get; set; }

        public virtual ICollection<Skill> Skills { get; set; }

        public virtual ICollection<Message> SentMessages { get; set; }

        public virtual ICollection<Message> RecievedMessages { get; set; }

        public virtual ICollection<UserPost> Posts { get; set; }

        public User()
        {
            this.Connections = new HashSet<User>();
            this.Followings = new HashSet<Company>();
            this.Projects = new HashSet<Project>();
            this.Certificates = new HashSet<Certificate>();
            this.Skills = new HashSet<Skill>();
            this.SentMessages = new HashSet<Message>();
            this.RecievedMessages = new HashSet<Message>();
            this.Posts = new HashSet<UserPost>();
        }
    }
}
