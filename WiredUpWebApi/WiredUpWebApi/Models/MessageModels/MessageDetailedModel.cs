using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace WiredUpWebApi.Models.MessageModels
{
    [DataContract]
    public class MessageDetailedModel
    {
        [DataMember(Name = "content")]
        public string Content { get; set; }

        [DataMember(Name = "senderName")]
        public string SenderName { get; set; }

        [DataMember(Name = "receiverName")]
        public string ReceiverName { get; set; }

        public static Expression<Func<Message, MessageDetailedModel>> FromMessage
        {
            get
            {
                return msg => new MessageDetailedModel()
                {
                    Content = msg.Content,
                    SenderName = msg.Sender.FirstName + " " + msg.Sender.LastName,
                    ReceiverName = msg.Receiver.FirstName + " " + msg.Receiver.LastName
                };
            }
        }
    }
}