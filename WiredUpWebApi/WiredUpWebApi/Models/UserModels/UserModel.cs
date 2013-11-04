using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace WiredUpWebApi.Models.UserModels
{
    [DataContract]
    public class UserModel
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "displayName")]
        public string DisplayName { get; set; }

        [DataMember(Name = "photo")]
        public byte[] Photo { get; set; }

        public static Expression<Func<User, UserModel>> FromUser
        {
            get
            {
                return user => new UserModel()
                {
                    Id = user.Id,
                    Photo = user.Photo,
                    DisplayName = user.FirstName + " " + user.LastName
                };
            }
        }
    }
}