using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using WiredUpWebApi.Models.Constants;

namespace WiredUpWebApi.Models
{
    public class Project
    {
        [Key]
        [Column("ProjectId")]
        public int Id { get; set; }

        [Required]
        [MaxLength(ProjectConstants.NameMaxLength)]
        public string Name { get; set; }

        [MaxLength(ProjectConstants.DescriptionMaxLength)]
        public string Description { get; set; }

        [MaxLength(ProjectConstants.UrlMaxLength)]
        public string Url { get; set; }

        public int UserId { get; set; }

        public virtual User User { get; set; }

        public virtual ICollection<User> TeamMembers { get; set; }

        public Project()
        {
            this.TeamMembers = new HashSet<User>();
        }
    }
}
