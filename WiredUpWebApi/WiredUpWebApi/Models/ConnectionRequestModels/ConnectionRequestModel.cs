using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace WiredUpWebApi.Models.ConnectionRequestModels
{
    [DataContract]
    public class ConnectionRequestModel
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "senderId")]
        public int SenderId { get; set; }

        [DataMember(Name = "senderDisplayName")]
        public string SenderDisplayName { get; set; }

        public static Expression<Func<ConnectionRequest, ConnectionRequestModel>> FromConnectionRequest
        {
            get
            {
                return conRequest => new ConnectionRequestModel()
                {
                    Id = conRequest.Id,
                    SenderId = conRequest.SenderId,
                    SenderDisplayName = conRequest.Sender.FirstName + " " + conRequest.Sender.LastName
                };
            }
        }
    }
}