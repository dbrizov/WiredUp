using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        public IQueryable<MessageModel> GetSentMessage([FromUri]string sessionKey)
        {
            var user = this.GetUserBySessionKey(sessionKey);
            var messages = this.db.Messages
                .All()
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Where(m => m.SenderId == user.Id)
                .Select(MessageModel.FromMessage);

            return messages;
        }

        [HttpGet]
        [ActionName("received")]
        public IQueryable<MessageModel> GetReceivedMessages([FromUri]string sessionKey)
        {
            var user = this.GetUserBySessionKey(sessionKey);
            var messages = this.db.Messages
                .All()
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Where(m => m.ReceiverId == user.Id)
                .Select(MessageModel.FromMessage);

            return messages;
        }

        [HttpGet]
        [ActionName("all")]
        public IQueryable<MessageModel> GetAllMessages([FromUri]string sessionKey)
        {
            var user = this.GetUserBySessionKey(sessionKey);
            var messages = this.db.Messages
                .All()
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Where(m => m.ReceiverId == user.Id || m.SenderId == user.Id)
                .Select(MessageModel.FromMessage);

            return messages;
        }

        [HttpDelete]
        [ActionName("delete")]
        public HttpResponseMessage DeleteMessage([FromUri]int id, [FromUri]string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(() =>
            {
                var user = this.GetUserBySessionKey(sessionKey);
                var message = user.SentMessages.FirstOrDefault(m => m.Id == id);
                if (message == null)
                {
                    message = user.ReceivedMessages.FirstOrDefault(m => m.Id == id);
                    if (message == null)
                    {
                        throw new ArgumentException("The user does not have a message with the requested id, or the message is not his");
                    }
                }

                this.db.Messages.Delete(message);
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