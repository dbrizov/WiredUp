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

        public static Expression<Func<Connection, UserModel>> FromConnection
        {
            get
            {
                return conn => new UserModel()
                {
                    Id = conn.OtherUserId,
                    DisplayName = conn.OtherUser.FirstName + " " + conn.OtherUser.LastName,
                    Photo = conn.OtherUser.Photo
                };
            }
        }
    }
}