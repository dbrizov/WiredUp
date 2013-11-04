using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using WiredUpWebApi.Models.CertificateModels;
using WiredUpWebApi.Models.ProjectModels;

namespace WiredUpWebApi.Models.UserModels
{
    [DataContract]
    public class UserDetailedModel
    {
        [DataMember(Name = "displayName")]
        public string DisplayName { get; set; }

        [DataMember(Name = "email")]
        public string Email { get; set; }

        [DataMember(Name = "photo")]
        public byte[] Photo { get; set; }

        [DataMember(Name = "country")]
        public string Country { get; set; }

        [DataMember(Name = "skills")]
        public IEnumerable<string> Skills { get; set; }

        [DataMember(Name = "languages")]
        public string Languages { get; set; }

        [DataMember(Name = "certificates")]
        public IEnumerable<CertificateModel> Certificates { get; set; }

        [DataMember(Name = "projects")]
        public IEnumerable<ProjectModel> Projects { get; set; }

        public static Expression<Func<User, UserDetailedModel>> FromUser
        {
            get
            {
                return user => new UserDetailedModel()
                {
                    DisplayName = user.FirstName + " " + user.LastName,
                    Email = user.Email,
                    Photo = user.Photo,
                    Country = user.Country.Name,
                    Skills = user.Skills.Select(s => s.Name),
                    Languages = user.Languages,
                    Certificates = user.Certificates.Select(CertificateModel.FromCertificate.Compile()),
                    Projects = user.Projects.Select(ProjectModel.FromProject.Compile())
                };
            }
        }
    }
}