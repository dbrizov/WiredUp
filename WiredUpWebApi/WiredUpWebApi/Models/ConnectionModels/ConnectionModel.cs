using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace WiredUpWebApi.Models.ConnectionModels
{
    [DataContract]
    public class ConnectionModel
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "otherUserId")]
        public int OtherUserId { get; set; }

        [DataMember(Name = "otherUserDisplayName")]
        public string OtherUserDisplayName { get; set; }

        [DataMember(Name = "otherUserPhoto")]
        public byte[] OtherUserPhoto { get; set; }

        public static Expression<Func<Connection, ConnectionModel>> FromConnection
        {
            get
            {
                return conn => new ConnectionModel()
                {
                    Id = conn.Id,
                    OtherUserId = conn.OtherUserId,
                    OtherUserDisplayName = conn.OtherUser.FirstName + " " + conn.OtherUser.LastName,
                    OtherUserPhoto = conn.OtherUser.Photo
                };
            }
        }
    }
}