using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using WiredUpWebApi.Models.Constants;

namespace WiredUpWebApi.Models
{
    public class Certificate
    {
        [Key]
        [Column("CertificateId")]
        public int Id { get; set; }

        [Required]
        [MaxLength(CertificateConstants.NameMaxLength)]
        public string Name { get; set; }

        [MaxLength(CertificateConstants.UrlMaxLength)]
        public string Url { get; set; }

        public int OwnerId { get; set; }

        public virtual User Owner { get; set; }
    }
}
