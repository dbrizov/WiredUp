using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace WiredUpWebApi.Models
{
    public class Address
    {
        [Key]
        public int Id { get; set; }

        public int CountryId { get; set; }

        public virtual Country Country { get; set; }

        public int CityId { get; set; }

        public virtual City City { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
