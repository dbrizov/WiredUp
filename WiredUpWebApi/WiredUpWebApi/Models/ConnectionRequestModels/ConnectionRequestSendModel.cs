using System;
using System.Linq;
using System.Runtime.Serialization;

namespace WiredUpWebApi.Models.ConnectionRequestModels
{
    [DataContract]
    public class ConnectionRequestSendModel
    {
        [DataMember(Name = "receiverId")]
        public int ReceiverId { get; set; }
    }
}