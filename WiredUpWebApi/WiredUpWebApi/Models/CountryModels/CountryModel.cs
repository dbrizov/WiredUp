using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace WiredUpWebApi.Models.CountryModels
{
    [DataContract]
    public class CountryModel
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        public static Expression<Func<Country, CountryModel>> FromCountry
        {
            get
            {
                return country => new CountryModel()
                {
                    Id = country.Id,
                    Name = country.Name
                };
            }
        }
    }
}