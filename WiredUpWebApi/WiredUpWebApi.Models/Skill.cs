using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WiredUpWebApi.Models.Constants;

namespace WiredUpWebApi.Models
{
    public class Skill
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(SkillConstants.NameMaxLength)]
        public string Name { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
