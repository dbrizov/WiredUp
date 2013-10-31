using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace WiredUpWebApi.Models
{
    public class CompanyPost
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string Content { get; set; }

        [Required]
        public DateTime PostDate { get; set; }

        public int CompanyId { get; set; }

        public virtual Company Company { get; set; }
    }
}
