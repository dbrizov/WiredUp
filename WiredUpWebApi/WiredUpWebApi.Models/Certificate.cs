using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace WiredUpWebApi.Models
{
    public class Certificate
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        public string Url { get; set; }

        public int OwnerId { get; set; }

        public virtual User Owner { get; set; }
    }
}
