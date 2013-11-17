using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Transactions;
using WiredUpWebApi.Data;
using WiredUpWebApi.Models;
using WiredUpWebApi.Models.ConnectionModels;

namespace WiredUpWebApi.Tests
{
#if !DEBUG
    [Ignore]
#endif
    [TestClass]
    public class ConnectionsControllerIntegrationTests
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

        private readonly User thirdUser = new User()
        {
            FirstName = "Pesho",
            LastName = "Rizov",
            AuthCode = new string('a', Sha1PasswordLength),
            Email = "pesho@yahoo.com",
            SessionKey = "third"
        };

        [TestMethod]
        public void All_ShouldReturnAllConnections()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(this.firstUser);
                this.db.Users.Add(this.secondUser);
                this.db.Users.Add(this.thirdUser);
                this.db.SaveChanges();

                var firstConnection = new Connection()
                {
                    User = this.firstUser,
                    OtherUser = this.secondUser
                };

                this.db.Connections.Add(firstConnection);
                this.db.SaveChanges();

                var secondConnection = new Connection()
                {
                    User = this.firstUser,
                    OtherUser = this.thirdUser
                };

                this.db.Connections.Add(secondConnection);
                this.db.SaveChanges();

                var firstResponse = this.httpServer.CreateGetRequest(
                    "/api/connections/all?sessionKey=" + this.firstUser.SessionKey);
                Assert.AreEqual(2, this.GetConnectionsFromResponse(firstResponse).Count());
            }
        }

        [TestMethod]
        public void Delete_ShouldDeleteFromDatabase()
        {
            using (new TransactionScope())
            {
                int oldConnectionsCount = this.db.Connections.All().Count();

                this.db.Users.Add(this.firstUser);
                this.db.Users.Add(this.secondUser);
                this.db.SaveChanges();

                var firstConnection = new Connection()
                {
                    User = this.firstUser,
                    OtherUser = this.secondUser
                };

                var secondConnection = new Connection()
                {
                    User = this.secondUser,
                    OtherUser = this.firstUser
                };

                this.db.Connections.Add(firstConnection);
                this.db.Connections.Add(secondConnection);
                this.db.SaveChanges();

                Assert.AreEqual(oldConnectionsCount + 2, this.db.Connections.All().Count());

                var response = this.httpServer.CreateDeleteRequest(
                    string.Format(
                        "/api/connections/delete/{0}?sessionKey={1}",
                        firstConnection.Id,
                        this.firstUser.SessionKey));

                Assert.AreEqual(oldConnectionsCount, this.db.Connections.All().Count());
            }
        }

        private IEnumerable<ConnectionModel> GetConnectionsFromResponse(HttpResponseMessage response)
        {
            var connectionAsString = response.Content.ReadAsStringAsync().Result;
            var connections =
                JsonConvert.DeserializeObject<IEnumerable<ConnectionModel>>(connectionAsString);

            return connections;
        }
    }
}
