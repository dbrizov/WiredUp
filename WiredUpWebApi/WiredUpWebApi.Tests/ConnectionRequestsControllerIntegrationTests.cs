using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Transactions;
using WiredUpWebApi.Data;
using WiredUpWebApi.Models;
using WiredUpWebApi.Models.ConnectionRequestModels;

namespace WiredUpWebApi.Tests
{
#if !DEBUG
    [Ignore]
#endif
    [TestClass]
    public class ConnectionRequestsControllerIntegrationTests
    {
        private const int Sha1PasswordLength = 40;

        private readonly IUnitOfWorkData db = new UnitOfWorkData();
        private readonly InMemoryHttpServer httpServer = new InMemoryHttpServer("http://localhost");

        private readonly User firstUser = new User()
        {
            FirstName = "Denis",
            LastName = "Rizov",
            AuthCode = new string('a', Sha1PasswordLength),
            Email = "dbrizov@yahoo.com",
            SessionKey = "first"
        };

        private readonly User secondUser = new User()
        {
            FirstName = "Chris",
            LastName = "Rizov",
            AuthCode = new string('a', Sha1PasswordLength),
            Email = "chris@yahoo.com",
            SessionKey = "second"
        };

        [TestMethod]
        public void Send_WhenDataIsValid_ShouldSendConnectionRequest()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(firstUser);
                this.db.Users.Add(secondUser);
                this.db.SaveChanges();

                var conRequestModel = new ConnectionRequestSendModel()
                {
                    ReceiverId = this.secondUser.Id
                };

                var response = this.httpServer.CreatePostRequest(
                    string.Format("/api/connectionRequests/send?sessionKey={0}", this.firstUser.SessionKey),
                    conRequestModel);
                Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

                var connectionRequests = this.db.ConnectionRequests
                    .All()
                    .Where(cr => cr.ReceiverId == this.secondUser.Id);
                Assert.AreEqual(1, connectionRequests.Count());
            }
        }

        [TestMethod]
        public void Send_WhenDataIsInvalid_ShouldReturnBadRequest()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(firstUser);
                this.db.Users.Add(secondUser);
                this.db.SaveChanges();

                var conRequestModel = new ConnectionRequestSendModel()
                {
                    ReceiverId = -1
                };

                var response = this.httpServer.CreatePostRequest(
                    string.Format("/api/connectionRequests/send?sessionKey={0}", this.firstUser.SessionKey),
                    conRequestModel);
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public void Send_WhenSessionKeyIsInvalid_ShouldReturnBadRequest()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(firstUser);
                this.db.Users.Add(secondUser);
                this.db.SaveChanges();

                var conRequestModel = new ConnectionRequestSendModel()
                {
                    ReceiverId = this.secondUser.Id
                };

                var response = this.httpServer.CreatePostRequest(
                    string.Format("/api/connectionRequests/send?sessionKey={0}", "Invalid session key"),
                    conRequestModel);
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public void Accept_WhenDataIsValid_ShouldCreateConnectionAndDeleteTheConnectionRequest()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(firstUser);
                this.db.Users.Add(secondUser);
                this.db.SaveChanges();

                var connectionRequest = new ConnectionRequest()
                {
                    Receiver = this.secondUser,
                    Sender = this.firstUser
                };

                this.db.ConnectionRequests.Add(connectionRequest);
                this.db.SaveChanges();

                var response = this.httpServer.CreatePostRequest(
                    string.Format("/api/connectionRequests/accept/{0}?sessionKey={1}",
                        connectionRequest.Id,
                        this.secondUser.SessionKey),
                    (object)null);
                Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

                var firstUserConnections = this.db.Connections
                    .All()
                    .Where(c => c.OtherUserId == this.secondUser.Id);

                var secondUserConnections = this.db.Connections
                    .All()
                    .Where(c => c.OtherUserId == this.firstUser.Id);

                Assert.AreEqual(1, firstUserConnections.Count());
                Assert.AreEqual(1, secondUserConnections.Count());

                var secondUserConntionRequest = this.db.ConnectionRequests
                    .All()
                    .Where(c => c.ReceiverId == this.secondUser.Id);
                Assert.AreEqual(0, secondUserConntionRequest.Count());
            }
        }

        [TestMethod]
        public void Accept_WhenConnectionIdIsInvalid_ShouldReturnBadRequest()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(firstUser);
                this.db.Users.Add(secondUser);
                this.db.SaveChanges();

                var connectionRequest = new ConnectionRequest()
                {
                    Receiver = this.secondUser,
                    Sender = this.firstUser
                };

                this.db.ConnectionRequests.Add(connectionRequest);
                this.db.SaveChanges();

                var response = this.httpServer.CreatePostRequest(
                    string.Format("/api/connectionRequests/accept/{0}?sessionKey={1}",
                        -1,
                        this.secondUser.SessionKey),
                    (object)null);
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public void Decline_WhenDataIsValid_ShouldDeleteConnectionRequest()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(firstUser);
                this.db.Users.Add(secondUser);
                this.db.SaveChanges();

                var connectionRequest = new ConnectionRequest()
                {
                    Receiver = this.secondUser,
                    Sender = this.firstUser
                };

                this.db.ConnectionRequests.Add(connectionRequest);
                this.db.SaveChanges();

                Assert.AreEqual(1, this.secondUser.ConnectionRequests.Count());

                var response = this.httpServer.CreatePostRequest(
                    string.Format("/api/connectionRequests/decline/{0}?sessionKey={1}",
                        connectionRequest.Id,
                        this.secondUser.SessionKey),
                    (object)null);
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

                var secondUserConntionRequest = this.db.ConnectionRequests
                    .All()
                    .Where(c => c.ReceiverId == this.secondUser.Id);
                Assert.AreEqual(0, secondUserConntionRequest.Count());
            }
        }

        [TestMethod]
        public void All_ShouldReturnCorrectNumberOfConnectionRequests()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(firstUser);
                this.db.Users.Add(secondUser);
                this.db.SaveChanges();

                var connectionRequest = new ConnectionRequest()
                {
                    Receiver = this.secondUser,
                    Sender = this.firstUser
                };

                this.db.ConnectionRequests.Add(connectionRequest);
                this.db.SaveChanges();

                Assert.AreEqual(1, this.secondUser.ConnectionRequests.Count());

                var response = this.httpServer.CreateGetRequest(
                    string.Format("/api/connectionRequests/all?sessionKey={0}",
                        this.secondUser.SessionKey));
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

                var connectionRequestsAsString = response.Content.ReadAsStringAsync().Result;
                var connectionRequests =
                    JsonConvert.DeserializeObject<IEnumerable<ConnectionRequestModel>>(connectionRequestsAsString);

                Assert.AreEqual(1, connectionRequests.Count());
            }
        }
    }
}
