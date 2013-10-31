using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WiredUpWebApi.Models.Constants;

namespace WiredUpWebApi.Models
{
    public class City
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(CityConstants.NameMaxLength)]
        public string Name { get; set; }

        public int CountryId { get; set; }

        public virtual Country Country { get; set; }
    }
}
