using System;
using System.Linq;
using System.Runtime.Serialization;

namespace WiredUpWebApi.Models.CertificateModels
{
    [DataContract]
    public class CertificateCreateModel
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }
    }
}