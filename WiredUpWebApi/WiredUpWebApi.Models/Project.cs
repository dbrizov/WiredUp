using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace WiredUpWebApi.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        public string Description { get; set; }

        public string ProjectUrl { get; set; }

        public virtual ICollection<User> TeamMembers { get; set; }
    }
}
