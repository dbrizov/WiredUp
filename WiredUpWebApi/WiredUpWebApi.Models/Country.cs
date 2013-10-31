using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using WiredUpWebApi.Models.Constants;

namespace WiredUpWebApi.Models
{
    public class Country
    {
        [Key]
        [Column("CountryId")]
        public int Id { get; set; }

        [Required]
        [MaxLength(CountryConstants.NameMaxLength)]
        public string Name { get; set; }
    }
}
