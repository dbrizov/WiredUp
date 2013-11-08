using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;

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

        [DataMember(Name = "aboutMe")]
        public string AboutMe { get; set; }

        [DataMember(Name = "languages")]
        public string Languages { get; set; }

        public static Expression<Func<User, UserDetailedModel>> FromUser
        {
            get
            {
                return user => new UserDetailedModel()
                {
                    DisplayName = user.FirstName + " " + user.LastName,
                    Email = user.Email,
                    Photo = user.Photo,
                    AboutMe = user.AboutMe,
                    Country = user.Country == null ? null : user.Country.Name,
                    Languages = user.Languages
                };
            }
        }
    }
}