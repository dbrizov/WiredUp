using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace WiredUpWebApi.Models.MessageModels
{
    [DataContract]
    public class MessageModel
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "content")]
        public string Content { get; set; }

        [DataMember(Name = "senderId")]
        public int SenderId { get; set; }

        [DataMember(Name = "senderName")]
        public string SenderName { get; set; }

        [DataMember(Name = "receiverId")]
        public int ReceiverId { get; set; }

        [DataMember(Name = "receiverName")]
        public string ReceiverName { get; set; }

        [DataMember(Name = "postDate")]
        public DateTime PostDate { get; set; }

        public static Expression<Func<Message, MessageModel>> FromMessage
        {
            get
            {
                return msg => new MessageModel()
                {
                    Id = msg.Id,
                    Content = msg.Content,
                    SenderId = msg.SenderId,
                    SenderName = msg.Sender.FirstName + " " + msg.Sender.LastName,
                    ReceiverId = msg.ReceiverId,
                    ReceiverName = msg.Receiver.FirstName + " " + msg.Receiver.LastName,
                    PostDate = msg.PostDate
                };
            }
        }
    }
}