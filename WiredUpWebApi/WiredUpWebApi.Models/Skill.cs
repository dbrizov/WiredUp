using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using WiredUpWebApi.Models.Constants;

namespace WiredUpWebApi.Models
{
    public class Skill
    {
        [Key]
        [Column("SkillId")]
        public int Id { get; set; }

        [Required]
        [MaxLength(SkillConstants.NameMaxLength)]
        public string Name { get; set; }

        public virtual ICollection<User> Users { get; set; }

        public Skill()
        {
            this.Users = new HashSet<User>();
        }
    }
}
