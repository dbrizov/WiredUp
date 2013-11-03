using System;
using System.Linq;
using System.Runtime.Serialization;

namespace WiredUpWebApi.Models.UserModels
{
    [DataContract]
    public class UserEditModel
    {
        [DataMember(Name = "languages")]
        public string Languages { get; set; }

        [DataMember(Name = "countryId")]
        public int CountryId { get; set; }

        [DataMember(Name = "photo")]
        public byte[] Photo { get; set; }
    }
}