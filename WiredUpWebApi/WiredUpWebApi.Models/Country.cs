using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WiredUpWebApi.Models.Constants;

namespace WiredUpWebApi.Models
{
    public class Country
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(CountryConstants.NameMaxLength)]
        public string Name { get; set; }

        public virtual ICollection<City> Cities { get; set; }
    }
}
