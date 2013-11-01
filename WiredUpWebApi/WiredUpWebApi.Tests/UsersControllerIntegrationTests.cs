using System;
using System.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WiredUpWebApi.Models.UserModels;
using System.Net;
using Newtonsoft.Json;

namespace WiredUpWebApi.Tests
{
    [TestClass]
    public class UsersControllerIntegrationTests
    {
        [TestMethod]
        public void Register_WhenDataIsValid_ShouldSaveInDatabase()
        {
            using (new TransactionScope())
            {
                UserRegisterModel testUser = new UserRegisterModel()
                {
                    FirstName = "Denis",
                    LastName = "Rizov",
                    AuthCode = "DenisDenisDenisDenisDenisDenisDenisDenis",
                    ConfirmAuthCode = "DenisDenisDenisDenisDenisDenisDenisDenis",
                    Email = "dbrizov@yahoo.com"
                };

                var httpServer = new InMemoryHttpServer("http://localhost/");
                var response = httpServer.CreatePostRequest("/api/users/register", testUser);

                Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
                Assert.IsNotNull(response.Content);

                var contentString = response.Content.ReadAsStringAsync().Result;
                var userLoggedModel = JsonConvert.DeserializeObject<UserLoggedModel>(contentString);
                Assert.AreEqual("Denis Rizov", userLoggedModel.DisplayName);
                Assert.IsTrue(userLoggedModel.Id > 0);
                Assert.IsNotNull(userLoggedModel.SessionKey);
            }
        }
    }
}
