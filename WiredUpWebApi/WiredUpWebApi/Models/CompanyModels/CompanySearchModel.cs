using System;
using System.Linq;
using System.Runtime.Serialization;

namespace WiredUpWebApi.Models.CompanyModels
{
    [DataContract]
    public class CompanySearchModel
    {
        [DataMember(Name = "nameQuery")]
        public string NameQuery { get; set; }
    }
}