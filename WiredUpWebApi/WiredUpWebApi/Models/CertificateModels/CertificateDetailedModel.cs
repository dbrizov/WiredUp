using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace WiredUpWebApi.Models.CertificateModels
{
    [DataContract]
    public class CertificateDetailedModel
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "owner")]
        public string Owner { get; set; }

        public static Expression<Func<Certificate, CertificateDetailedModel>> FromCertificate
        {
            get
            {
                return certificate => new CertificateDetailedModel()
                {
                    Name = certificate.Name,
                    Owner = certificate.Owner.FirstName + " " + certificate.Owner.LastName,
                    Url = certificate.Url
                };
            }
        }
    }
}