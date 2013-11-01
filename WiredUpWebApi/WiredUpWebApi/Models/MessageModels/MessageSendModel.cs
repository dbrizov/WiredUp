using System;
using System.Linq;
using System.Runtime.Serialization;

namespace WiredUpWebApi.Models.MessageModels
{
    [DataContract]
    public class MessageSendModel
    {
        [DataMember(Name = "receiverId")]
        public int ReceiverId { get; set; }

        [DataMember(Name = "content")]
        public string Content { get; set; }
    }
}