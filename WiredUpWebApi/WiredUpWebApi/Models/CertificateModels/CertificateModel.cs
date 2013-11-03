using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace WiredUpWebApi.Models.CertificateModels
{
    [DataContract]
    public class CertificateModel
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }


        [DataMember(Name = "name")]
        public string Name { get; set; }

        public static Expression<Func<Certificate, CertificateModel>> FromCertificate
        {
            get
            {
                return certificate => new CertificateModel()
                {
                    Id = certificate.Id,
                    Name = certificate.Name
                };
            }
        }
    }
}