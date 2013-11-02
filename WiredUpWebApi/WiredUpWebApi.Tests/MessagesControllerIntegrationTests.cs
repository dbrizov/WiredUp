using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Transactions;
using WiredUpWebApi.Data;
using WiredUpWebApi.Models.Constants;
using WiredUpWebApi.Models.MessageModels;
using WiredUpWebApi.Models.UserModels;

namespace WiredUpWebApi.Tests
{
#if !DEBUG
    [Ignore]
#endif
    [TestClass]
    public class MessagesControllerIntegrationTests
    {
        private const int Sha1PasswordLength = 40;
        private const int MessageContentMaxLength = MessageConstants.ContentMaxLength;

        private readonly IUnitOfWorkData db = new UnitOfWorkData();
        private readonly InMemoryHttpServer httpServer = new InMemoryHttpServer("http://localhost");

        private readonly UserRegisterModel firstUser = new UserRegisterModel()
        {
            FirstName = "Denis",
            LastName = "Rizov",
            AuthCode = new string('a', Sha1PasswordLength),
            ConfirmAuthCode = new string('a', Sha1PasswordLength),
            Email = "dbrizov@yahoo.com"
        };

        private readonly UserRegisterModel secondUser = new UserRegisterModel()
        {
            FirstName = "Pesho",
            LastName = "Peshev",
            AuthCode = new string('a', Sha1PasswordLength),
            ConfirmAuthCode = new string('a', Sha1PasswordLength),
            Email = "pesho@yahoo.com"
        };

        [TestMethod]
        public void Send_WhenDataIsValid_ShouldSaveInDatabase()
        {
            using (new TransactionScope())
            {
                var firstResponse = this.httpServer.CreatePostRequest("/api/users/register", this.firstUser);
                var secondResponse = this.httpServer.CreatePostRequest("/api/users/register", this.secondUser);

                var firstLoggedUser = this.GetUserFromResponse(firstResponse);
                var secondLoggedUser = this.GetUserFromResponse(secondResponse);

                var message = new MessageSendModel()
                {
                    Content = "Some content",
                    ReceiverId = secondLoggedUser.Id
                };

                var response = this.httpServer.CreatePostRequest(
                    "/api/messages/send?sessionKey=" + firstLoggedUser.SessionKey, message);
                Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

                var userOne = this.db.Users.GetById(firstLoggedUser.Id);
                Assert.AreEqual(1, userOne.SentMessages.Count);
                Assert.AreEqual(0, userOne.ReceivedMessages.Count);
                Assert.AreEqual(message.Content, userOne.SentMessages.ElementAt(0).Content);

                var userTwo = this.db.Users.GetById(secondLoggedUser.Id);
                Assert.AreEqual(1, userTwo.ReceivedMessages.Count);
                Assert.AreEqual(0, userTwo.SentMessages.Count);
                Assert.AreEqual(message.Content, userTwo.ReceivedMessages.ElementAt(0).Content);
            }
        }

        [TestMethod]
        public void Send_TwoValidMessages_ShouldSaveInDatabase()
        {
            using (new TransactionScope())
            {
                var firstResponse = this.httpServer.CreatePostRequest("/api/users/register", this.firstUser);
                var secondResponse = this.httpServer.CreatePostRequest("/api/users/register", this.secondUser);

                var firstLoggedUser = this.GetUserFromResponse(firstResponse);
                var secondLoggedUser = this.GetUserFromResponse(secondResponse);

                var message = new MessageSendModel()
                {
                    Content = "Some content",
                    ReceiverId = secondLoggedUser.Id
                };

                var response = this.httpServer.CreatePostRequest(
                    "/api/messages/send?sessionKey=" + firstLoggedUser.SessionKey, message);
                Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

                response = this.httpServer.CreatePostRequest(
                    "/api/messages/send?sessionKey=" + firstLoggedUser.SessionKey, message);
                Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

                var userOne = this.db.Users.GetById(firstLoggedUser.Id);
                Assert.AreEqual(2, userOne.SentMessages.Count);
                Assert.AreEqual(0, userOne.ReceivedMessages.Count);

                var userTwo = this.db.Users.GetById(secondLoggedUser.Id);
                Assert.AreEqual(2, userTwo.ReceivedMessages.Count);
                Assert.AreEqual(0, userTwo.SentMessages.Count);
            }
        }

        [TestMethod]
        public void Send_WhenReceiverIdIsInvalid_ShouldReturnBadRequest()
        {
            using (new TransactionScope())
            {
                var firstResponse = this.httpServer.CreatePostRequest("/api/users/register", this.firstUser);
                var secondResponse = this.httpServer.CreatePostRequest("/api/users/register", this.secondUser);

                var firstLoggedUser = this.GetUserFromResponse(firstResponse);
                var secondLoggedUser = this.GetUserFromResponse(secondResponse);

                var message = new MessageSendModel()
                {
                    Content = "Some content",
                    ReceiverId = 0
                };

                var response = this.httpServer.CreatePostRequest(
                    "/api/messages/send?sessionKey=" + firstLoggedUser.SessionKey, message);
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public void Send_WhentContentIsTooBig_ShouldReturnBadRequest()
        {
            using (new TransactionScope())
            {
                var firstResponse = this.httpServer.CreatePostRequest("/api/users/register", this.firstUser);
                var secondResponse = this.httpServer.CreatePostRequest("/api/users/register", this.secondUser);

                var firstLoggedUser = this.GetUserFromResponse(firstResponse);
                var secondLoggedUser = this.GetUserFromResponse(secondResponse);

                var message = new MessageSendModel()
                {
                    Content = new string('a', MessageContentMaxLength + 1),
                    ReceiverId = secondLoggedUser.Id
                };

                var response = this.httpServer.CreatePostRequest(
                    "/api/messages/send?sessionKey=" + firstLoggedUser.SessionKey, message);
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public void Send_WhenContentIsNull_ShouldReturnBadRequest()
        {
            using (new TransactionScope())
            {
                var firstResponse = this.httpServer.CreatePostRequest("/api/users/register", this.firstUser);
                var secondResponse = this.httpServer.CreatePostRequest("/api/users/register", this.secondUser);

                var firstLoggedUser = this.GetUserFromResponse(firstResponse);
                var secondLoggedUser = this.GetUserFromResponse(secondResponse);

                var message = new MessageSendModel()
                {
                    ReceiverId = secondLoggedUser.Id
                };

                var response = this.httpServer.CreatePostRequest(
                    "/api/messages/send?sessionKey=" + firstLoggedUser.SessionKey, message);
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public void Send_WhenContentIsWhiteSpace_ShouldReturnBadRequest()
        {
            using (new TransactionScope())
            {
                var firstResponse = this.httpServer.CreatePostRequest("/api/users/register", this.firstUser);
                var secondResponse = this.httpServer.CreatePostRequest("/api/users/register", this.secondUser);

                var firstLoggedUser = this.GetUserFromResponse(firstResponse);
                var secondLoggedUser = this.GetUserFromResponse(secondResponse);

                var message = new MessageSendModel()
                {
                    Content = " ",
                    ReceiverId = secondLoggedUser.Id
                };

                var response = this.httpServer.CreatePostRequest(
                    "/api/messages/send?sessionKey=" + firstLoggedUser.SessionKey, message);
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public void SendMessage_WhenContentIsEmptyString_ShouldReturnBadRequest()
        {
            using (new TransactionScope())
            {
                var firstResponse = this.httpServer.CreatePostRequest("/api/users/register", this.firstUser);
                var secondResponse = this.httpServer.CreatePostRequest("/api/users/register", this.secondUser);

                var firstLoggedUser = this.GetUserFromResponse(firstResponse);
                var secondLoggedUser = this.GetUserFromResponse(secondResponse);

                var message = new MessageSendModel()
                {
                    Content = string.Empty,
                    ReceiverId = secondLoggedUser.Id
                };

                var response = this.httpServer.CreatePostRequest(
                    "/api/messages/send?sessionKey=" + firstLoggedUser.SessionKey, message);
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public void GetSentMessages_ShouldReturnCorrectNumberOfMessages()
        {
            using (new TransactionScope())
            {
                var firstResponse = this.httpServer.CreatePostRequest("/api/users/register", this.firstUser);
                var secondResponse = this.httpServer.CreatePostRequest("/api/users/register", this.secondUser);

                var firstLoggedUser = this.GetUserFromResponse(firstResponse);
                var secondLoggedUser = this.GetUserFromResponse(secondResponse);

                var message = new MessageSendModel()
                {
                    Content = "Some content",
                    ReceiverId = secondLoggedUser.Id
                };

                // Send the message
                var response = this.httpServer.CreatePostRequest(
                    "/api/messages/send?sessionKey=" + firstLoggedUser.SessionKey, message);
                Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

                // Get the message
                var sentResponse = this.httpServer.CreateGetRequest(
                    "/api/messages/sent?sessionKey=" + firstLoggedUser.SessionKey);
                Assert.AreEqual(HttpStatusCode.OK, sentResponse.StatusCode);

                var messagesAsString = sentResponse.Content.ReadAsStringAsync().Result;
                var messages = JsonConvert.DeserializeObject<IList<MessageDetailedModel>>(messagesAsString);
                Assert.AreEqual(1, messages.Count);
                Assert.AreEqual(message.Content, messages[0].Content);
                Assert.AreEqual(firstLoggedUser.DisplayName, messages[0].SenderName);
                Assert.AreEqual(secondLoggedUser.DisplayName, messages[0].ReceiverName);
            }
        }

        [TestMethod]
        public void GetReceivedMessages_ShouldReturnCorrectNumberOfMessages()
        {
            using (new TransactionScope())
            {
                var firstResponse = this.httpServer.CreatePostRequest("/api/users/register", this.firstUser);
                var secondResponse = this.httpServer.CreatePostRequest("/api/users/register", this.secondUser);

                var firstLoggedUser = this.GetUserFromResponse(firstResponse);
                var secondLoggedUser = this.GetUserFromResponse(secondResponse);

                var message = new MessageSendModel()
                {
                    Content = "Some content",
                    ReceiverId = secondLoggedUser.Id
                };

                // Send the message
                var response = this.httpServer.CreatePostRequest(
                    "/api/messages/send?sessionKey=" + firstLoggedUser.SessionKey, message);
                Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

                // Get the message
                var receivedResponse = this.httpServer.CreateGetRequest(
                    "/api/messages/received?sessionKey=" + secondLoggedUser.SessionKey);
                Assert.AreEqual(HttpStatusCode.OK, receivedResponse.StatusCode);

                var messagesAsString = receivedResponse.Content.ReadAsStringAsync().Result;
                var messages = JsonConvert.DeserializeObject<IList<MessageDetailedModel>>(messagesAsString);
                Assert.AreEqual(1, messages.Count);
                Assert.AreEqual(message.Content, messages[0].Content);
                Assert.AreEqual(firstLoggedUser.DisplayName, messages[0].SenderName);
                Assert.AreEqual(secondLoggedUser.DisplayName, messages[0].ReceiverName);
            }
        }

        [TestMethod]
        public void GetAllMessages_ShouldReturnCorrectNumberOfMessages()
        {
            using (new TransactionScope())
            {
                var firstResponse = this.httpServer.CreatePostRequest("/api/users/register", this.firstUser);
                var secondResponse = this.httpServer.CreatePostRequest("/api/users/register", this.secondUser);

                var firstLoggedUser = this.GetUserFromResponse(firstResponse);
                var secondLoggedUser = this.GetUserFromResponse(secondResponse);

                var message = new MessageSendModel()
                {
                    Content = "Some content",
                    ReceiverId = secondLoggedUser.Id
                };

                // Send the messages
                var response = this.httpServer.CreatePostRequest(
                    "/api/messages/send?sessionKey=" + firstLoggedUser.SessionKey, message);
                Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

                response = this.httpServer.CreatePostRequest(
                    "/api/messages/send?sessionKey=" + firstLoggedUser.SessionKey, message);
                Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

                // Get the messages
                var getAllResponse = this.httpServer.CreateGetRequest(
                    "/api/messages/all?sessionKey=" + firstLoggedUser.SessionKey);
                Assert.AreEqual(HttpStatusCode.OK, getAllResponse.StatusCode);

                var messagesAsString = getAllResponse.Content.ReadAsStringAsync().Result;
                var messages = JsonConvert.DeserializeObject<IList<MessageDetailedModel>>(messagesAsString);
                Assert.AreEqual(2, messages.Count);
            }
        }

        private UserLoggedModel GetUserFromResponse(HttpResponseMessage response)
        {
            var contentString = response.Content.ReadAsStringAsync().Result;
            var userLoggedModel = JsonConvert.DeserializeObject<UserLoggedModel>(contentString);

            return userLoggedModel;
        }
    }
}
