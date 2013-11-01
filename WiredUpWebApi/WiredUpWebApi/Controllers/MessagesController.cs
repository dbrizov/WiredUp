﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
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
                    throw new InvalidOperationException("The 'recieverId' is invalid");
                }

                this.ValidateMessageContent(model.Content);

                var newMessage = new Message()
                {
                    Content = model.Content,
                    Sender = sender,
                    Receiver = receiver
                };

                sender.SentMessages.Add(newMessage);
                receiver.RecievedMessages.Add(newMessage);

                this.db.Users.Update(sender);
                this.db.Users.Update(receiver);
                this.db.Messages.Add(newMessage);

                this.db.SaveChanges();

                var response = this.Request.CreateResponse(HttpStatusCode.Created);
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