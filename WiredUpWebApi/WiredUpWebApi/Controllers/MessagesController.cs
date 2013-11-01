using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WiredUpWebApi.Data;
using WiredUpWebApi.Models;
using WiredUpWebApi.Models.Constants;
using WiredUpWebApi.Models.MessageModels;

namespace WiredUpWebApi.Controllers
{
    public class MessagesController : BaseApiController
    {
        private const int MessageContentMaxLength = MessageConstants.ContentMaxLength;

        public MessagesController()
            : base()
        {
        }

        public MessagesController(IUnitOfWorkData db)
            : base(db)
        {
        }

        [HttpPost]
        [ActionName("send")]
        public HttpResponseMessage SendMessage([FromBody]MessageSendModel model, [FromUri]string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(() =>
            {
                var sender = this.GetUserBySessionKey(sessionKey);
                var receiver = this.db.Users.GetById(model.ReceiverId);

                if (receiver == null)
                {
                    throw new ArgumentException("The 'recieverId' is invalid");
                }

                this.ValidateMessageContent(model.Content);

                var newMessage = new Message()
                {
                    Content = model.Content,
                    Sender = sender,
                    Receiver = receiver
                };

                sender.SentMessages.Add(newMessage);
                receiver.ReceivedMessages.Add(newMessage);

                this.db.Users.Update(sender);
                this.db.Users.Update(receiver);
                this.db.Messages.Add(newMessage);

                this.db.SaveChanges();

                var response = this.Request.CreateResponse(HttpStatusCode.Created);
                return response;
            });

            return responseMsg;
        }

        [HttpGet]
        [ActionName("sent")]
        public IEnumerable<MessageDetailedModel> GetSentMessage([FromUri]string sessionKey)
        {
            var user = this.GetUserBySessionKey(sessionKey);
            var messages = user.SentMessages.Select(MessageDetailedModel.FromMessage.Compile());

            return messages;
        }

        [HttpGet]
        [ActionName("received")]
        public IEnumerable<MessageDetailedModel> GetReceivedMessages([FromUri]string sessionKey)
        {
            var user = this.GetUserBySessionKey(sessionKey);
            var messages = user.ReceivedMessages.Select(MessageDetailedModel.FromMessage.Compile());

            return messages;
        }

        [HttpGet]
        [ActionName("all")]
        public IEnumerable<MessageDetailedModel> GetAllMessages([FromUri]string sessionKey)
        {
            var user = this.GetUserBySessionKey(sessionKey);
            var sentMessages = user.SentMessages.Select(MessageDetailedModel.FromMessage.Compile());
            var receviedMessages = user.ReceivedMessages.Select(MessageDetailedModel.FromMessage.Compile());

            var allMessages = new List<MessageDetailedModel>();
            allMessages.AddRange(sentMessages);
            allMessages.AddRange(receviedMessages);

            return allMessages;
        }

        [HttpDelete]
        [ActionName("delete")]
        public HttpResponseMessage DeleteMessage([FromUri]int id, [FromUri]string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(() =>
            {
                if (!this.IsSessionKeyValid(sessionKey))
                {
                    throw new ArgumentException("Invalid session key");
                }

                this.db.Messages.Delete(id);
                this.db.SaveChanges();

                var response = this.Request.CreateResponse(HttpStatusCode.OK);
                return response;
            });

            return responseMsg;
        }

        private void ValidateMessageContent(string messageContent)
        {
            if (string.IsNullOrWhiteSpace(messageContent))
            {
                throw new ArgumentException("The 'Message Content' is required");
            }

            if (messageContent.Length > MessageContentMaxLength)
            {
                throw new ArgumentException(string.Format(
                    "The 'Message Content' must be less than {0} characters long",
                    MessageContentMaxLength));
            }
        }
    }
}