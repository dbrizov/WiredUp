using System;
using System.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WiredUpWebApi.Models.UserModels;
using System.Net;
using Newtonsoft.Json;
using WiredUpWebApi.Data;

namespace WiredUpWebApi.Tests
{
#if !DEBUG
    [Ignore]
#endif
    [TestClass]
    public class UsersControllerIntegrationTests
    {
        private const int Sha1PasswordLength = 40;

        private IUnitOfWorkData db = new UnitOfWorkData();

        [TestMethod]
        public void Register_WhenDataIsValid_ShouldSaveInDatabase()
        {
            using (new TransactionScope())
            {
                UserRegisterModel testUser = new UserRegisterModel()
                {
                    FirstName = "Denis",
                    LastName = "Rizov",
                    AuthCode = new string('a', Sha1PasswordLength),
                    ConfirmAuthCode = new string('a', Sha1PasswordLength),
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

        [TestMethod]
        public void Register_WhenFirstNameIsNull_ShoudlNotSaveInDatabase()
        {
            using (new TransactionScope())
            {
                UserRegisterModel testUser = new UserRegisterModel()
                {
                    LastName = "Rizov",
                    AuthCode = new string('a', Sha1PasswordLength),
                    ConfirmAuthCode = new string('a', Sha1PasswordLength),
                    Email = "dbrizov@yahoo.com"
                };

                var httpServer = new InMemoryHttpServer("http://localhost/");
                var response = httpServer.CreatePostRequest("/api/users/register", testUser);

                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public void Register_WhenFirstNameIsEmptyString_ShoudlNotSaveInDatabase()
        {
            using (new TransactionScope())
            {
                UserRegisterModel testUser = new UserRegisterModel()
                {
                    FirstName = string.Empty,
                    LastName = "Rizov",
                    AuthCode = new string('a', Sha1PasswordLength),
                    ConfirmAuthCode = new string('a', Sha1PasswordLength),
                    Email = "dbrizov@yahoo.com"
                };

                var httpServer = new InMemoryHttpServer("http://localhost/");
                var response = httpServer.CreatePostRequest("/api/users/register", testUser);

                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public void Register_WhenFirstNameIsWhiteSpace_ShoudlNotSaveInDatabase()
        {
            using (new TransactionScope())
            {
                UserRegisterModel testUser = new UserRegisterModel()
                {
                    FirstName = "      ",
                    LastName = "Rizov",
                    AuthCode = new string('a', Sha1PasswordLength),
                    ConfirmAuthCode = new string('a', Sha1PasswordLength),
                    Email = "dbrizov@yahoo.com"
                };

                var httpServer = new InMemoryHttpServer("http://localhost/");
                var response = httpServer.CreatePostRequest("/api/users/register", testUser);

                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public void Register_WhenLastNameIsNull_ShoudlNotSaveInDatabase()
        {
            using (new TransactionScope())
            {
                UserRegisterModel testUser = new UserRegisterModel()
                {
                    FirstName = "Rizov",
                    AuthCode = new string('a', Sha1PasswordLength),
                    ConfirmAuthCode = new string('a', Sha1PasswordLength),
                    Email = "dbrizov@yahoo.com"
                };

                var httpServer = new InMemoryHttpServer("http://localhost/");
                var response = httpServer.CreatePostRequest("/api/users/register", testUser);

                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public void Register_WhenLastNameIsEmptyString_ShoudlNotSaveInDatabase()
        {
            using (new TransactionScope())
            {
                UserRegisterModel testUser = new UserRegisterModel()
                {
                    FirstName = "Rizov",
                    LastName = string.Empty,
                    AuthCode = new string('a', Sha1PasswordLength),
                    ConfirmAuthCode = new string('a', Sha1PasswordLength),
                    Email = "dbrizov@yahoo.com"
                };

                var httpServer = new InMemoryHttpServer("http://localhost/");
                var response = httpServer.CreatePostRequest("/api/users/register", testUser);

                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public void Register_WhenLastNameIsWhiteSpace_ShoudlNotSaveInDatabase()
        {
            using (new TransactionScope())
            {
                UserRegisterModel testUser = new UserRegisterModel()
                {
                    FirstName = "Rizov",
                    LastName = "      ",
                    AuthCode = new string('a', Sha1PasswordLength),
                    ConfirmAuthCode = new string('a', Sha1PasswordLength),
                    Email = "dbrizov@yahoo.com"
                };

                var httpServer = new InMemoryHttpServer("http://localhost/");
                var response = httpServer.CreatePostRequest("/api/users/register", testUser);

                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public void Register_WhenAuthCodesDontMatch_ShoudlNotSaveInDatabase()
        {
            using (new TransactionScope())
            {
                UserRegisterModel testUser = new UserRegisterModel()
                {
                    FirstName = "Rizov",
                    LastName = "      ",
                    AuthCode = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaab",
                    ConfirmAuthCode = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
                    Email = "dbrizov@yahoo.com"
                };

                var httpServer = new InMemoryHttpServer("http://localhost/");
                var response = httpServer.CreatePostRequest("/api/users/register", testUser);

                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public void Register_WhenAuthCodeIsInvalid_IsSmaller_ShoudlNotSaveInDatabase()
        {
            using (new TransactionScope())
            {
                UserRegisterModel testUser = new UserRegisterModel()
                {
                    FirstName = "Rizov",
                    LastName = "      ",
                    AuthCode = new string('a', Sha1PasswordLength - 1),
                    ConfirmAuthCode = new string('a', Sha1PasswordLength - 1),
                    Email = "dbrizov@yahoo.com"
                };

                var httpServer = new InMemoryHttpServer("http://localhost/");
                var response = httpServer.CreatePostRequest("/api/users/register", testUser);

                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public void Register_WhenAuthCodeIsInvalid_IsBigger_ShoudlNotSaveInDatabase()
        {
            using (new TransactionScope())
            {
                UserRegisterModel testUser = new UserRegisterModel()
                {
                    FirstName = "Rizov",
                    LastName = "      ",
                    AuthCode = new string('a', Sha1PasswordLength + 1),
                    ConfirmAuthCode = new string('a', Sha1PasswordLength + 1),
                    Email = "dbrizov@yahoo.com"
                };

                var httpServer = new InMemoryHttpServer("http://localhost/");
                var response = httpServer.CreatePostRequest("/api/users/register", testUser);

                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public void Register_WhenEmailIsNull_ShoudlNotSaveInDatabase()
        {
            using (new TransactionScope())
            {
                UserRegisterModel testUser = new UserRegisterModel()
                {
                    FirstName = "Rizov",
                    LastName = "Rizov",
                    AuthCode = new string('a', Sha1PasswordLength),
                    ConfirmAuthCode = new string('a', Sha1PasswordLength)
                };

                var httpServer = new InMemoryHttpServer("http://localhost/");
                var response = httpServer.CreatePostRequest("/api/users/register", testUser);

                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public void Register_WhenEmailIsEmptyString_ShoudlNotSaveInDatabase()
        {
            using (new TransactionScope())
            {
                UserRegisterModel testUser = new UserRegisterModel()
                {
                    FirstName = "Rizov",
                    LastName = "Rizov",
                    AuthCode = new string('a', Sha1PasswordLength),
                    ConfirmAuthCode = new string('a', Sha1PasswordLength),
                    Email = string.Empty
                };

                var httpServer = new InMemoryHttpServer("http://localhost/");
                var response = httpServer.CreatePostRequest("/api/users/register", testUser);

                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public void Register_WhenEmailIsWhiteSpace_ShoudlNotSaveInDatabase()
        {
            using (new TransactionScope())
            {
                UserRegisterModel testUser = new UserRegisterModel()
                {
                    FirstName = "Rizov",
                    LastName = "Rizov",
                    AuthCode = new string('a', Sha1PasswordLength),
                    ConfirmAuthCode = new string('a', Sha1PasswordLength),
                    Email = "  "
                };

                var httpServer = new InMemoryHttpServer("http://localhost/");
                var response = httpServer.CreatePostRequest("/api/users/register", testUser);

                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public void Register_WhenEmailIsInvalid_FirstScenario_ShoudlNotSaveInDatabase()
        {
            using (new TransactionScope())
            {
                UserRegisterModel testUser = new UserRegisterModel()
                {
                    FirstName = "Rizov",
                    LastName = "Rizov",
                    AuthCode = new string('a', Sha1PasswordLength),
                    ConfirmAuthCode = new string('a', Sha1PasswordLength),
                    Email = "dbrizovyahoo.com"
                };

                var httpServer = new InMemoryHttpServer("http://localhost/");
                var response = httpServer.CreatePostRequest("/api/users/register", testUser);

                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public void Register_WhenEmailIsInvalid_SecondScenario_ShoudlNotSaveInDatabase()
        {
            using (new TransactionScope())
            {
                UserRegisterModel testUser = new UserRegisterModel()
                {
                    FirstName = "Rizov",
                    LastName = "Rizov",
                    AuthCode = new string('a', Sha1PasswordLength),
                    ConfirmAuthCode = new string('a', Sha1PasswordLength),
                    Email = "dbrizov@.com"
                };

                var httpServer = new InMemoryHttpServer("http://localhost/");
                var response = httpServer.CreatePostRequest("/api/users/register", testUser);

                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public void Register_WhenEmailIsInvalid_ThirdScenario_ShoudlNotSaveInDatabase()
        {
            using (new TransactionScope())
            {
                UserRegisterModel testUser = new UserRegisterModel()
                {
                    FirstName = "Rizov",
                    LastName = "Rizov",
                    AuthCode = new string('a', Sha1PasswordLength),
                    ConfirmAuthCode = new string('a', Sha1PasswordLength),
                    Email = "dbrizov@yahoocom"
                };

                var httpServer = new InMemoryHttpServer("http://localhost/");
                var response = httpServer.CreatePostRequest("/api/users/register", testUser);

                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public void Register_WhenEmailIsInvalid_FourthScenario_ShoudlNotSaveInDatabase()
        {
            using (new TransactionScope())
            {
                UserRegisterModel testUser = new UserRegisterModel()
                {
                    FirstName = "Rizov",
                    LastName = "Rizov",
                    AuthCode = new string('a', Sha1PasswordLength),
                    ConfirmAuthCode = new string('a', Sha1PasswordLength),
                    Email = "dbrizov@yahoo.m"
                };

                var httpServer = new InMemoryHttpServer("http://localhost/");
                var response = httpServer.CreatePostRequest("/api/users/register", testUser);

                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public void Register_WhenEmailIsInvalid_FifthScenario_ShoudlNotSaveInDatabase()
        {
            using (new TransactionScope())
            {
                UserRegisterModel testUser = new UserRegisterModel()
                {
                    FirstName = "Rizov",
                    LastName = "Rizov",
                    AuthCode = new string('a', Sha1PasswordLength),
                    ConfirmAuthCode = new string('a', Sha1PasswordLength),
                    Email = "dbrizov@yahoo"
                };

                var httpServer = new InMemoryHttpServer("http://localhost/");
                var response = httpServer.CreatePostRequest("/api/users/register", testUser);

                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public void Register_WhenEmailIsInvalid_SixthScenario_ShoudlNotSaveInDatabase()
        {
            using (new TransactionScope())
            {
                UserRegisterModel testUser = new UserRegisterModel()
                {
                    FirstName = "Rizov",
                    LastName = "Rizov",
                    AuthCode = new string('a', Sha1PasswordLength),
                    ConfirmAuthCode = new string('a', Sha1PasswordLength),
                    Email = "dbrizov@yahoo"
                };

                var httpServer = new InMemoryHttpServer("http://localhost/");
                var response = httpServer.CreatePostRequest("/api/users/register", testUser);

                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public void Register_WhenUserAlreadyExists_ShoudlNotSaveInDatabase()
        {
            using (new TransactionScope())
            {
                UserRegisterModel testUser = new UserRegisterModel()
                {
                    FirstName = "Denis",
                    LastName = "Rizov",
                    AuthCode = new string('a', Sha1PasswordLength),
                    ConfirmAuthCode = new string('a', Sha1PasswordLength),
                    Email = "dbrizov@yahoo.com"
                };

                var httpServer = new InMemoryHttpServer("http://localhost/");
                var firstResponse = httpServer.CreatePostRequest("/api/users/register", testUser);

                Assert.AreEqual(HttpStatusCode.Created, firstResponse.StatusCode);
                Assert.IsNotNull(firstResponse.Content);

                var contentString = firstResponse.Content.ReadAsStringAsync().Result;
                var userLoggedModel = JsonConvert.DeserializeObject<UserLoggedModel>(contentString);
                Assert.AreEqual("Denis Rizov", userLoggedModel.DisplayName);
                Assert.IsTrue(userLoggedModel.Id > 0);
                Assert.IsNotNull(userLoggedModel.SessionKey);

                var secondResponse = httpServer.CreatePostRequest("/api/users/register", testUser);
                Assert.AreEqual(HttpStatusCode.BadRequest, secondResponse.StatusCode);
            }
        }

        [TestMethod]
        public void Login_WhenDataIsValid_ShouldGenerateSessionKey()
        {
            using (new TransactionScope())
            {
                UserRegisterModel regUser = new UserRegisterModel()
                {
                    FirstName = "Denis",
                    LastName = "Rizov",
                    AuthCode = new string('a', Sha1PasswordLength),
                    ConfirmAuthCode = new string('a', Sha1PasswordLength),
                    Email = "dbrizov@yahoo.com"
                };

                var httpServer = new InMemoryHttpServer("http://localhost/");
                var firstResponse = httpServer.CreatePostRequest("/api/users/register", regUser);

                Assert.AreEqual(HttpStatusCode.Created, firstResponse.StatusCode);
                Assert.IsNotNull(firstResponse.Content);

                var contentString = firstResponse.Content.ReadAsStringAsync().Result;
                var userLoggedModel = JsonConvert.DeserializeObject<UserLoggedModel>(contentString);
                Assert.AreEqual("Denis Rizov", userLoggedModel.DisplayName);
                Assert.IsTrue(userLoggedModel.Id > 0);
                Assert.IsNotNull(userLoggedModel.SessionKey);

                UserLoginModel loginUser = new UserLoginModel()
                {
                    AuthCode = regUser.AuthCode,
                    Email = regUser.Email
                };

                var secondResponse = httpServer.CreatePostRequest("api/users/login", loginUser);

                Assert.AreEqual(HttpStatusCode.Created, secondResponse.StatusCode);
                Assert.IsNotNull(firstResponse.Content);

                contentString = firstResponse.Content.ReadAsStringAsync().Result;
                userLoggedModel = JsonConvert.DeserializeObject<UserLoggedModel>(contentString);
                Assert.AreEqual("Denis Rizov", userLoggedModel.DisplayName);
                Assert.IsTrue(userLoggedModel.Id > 0);
                Assert.IsNotNull(userLoggedModel.SessionKey);
            }
        }

        [TestMethod]
        public void Login_EmailIsNull_ShouldReturnBadRequest()
        {
            using (new TransactionScope())
            {
                UserRegisterModel regUser = new UserRegisterModel()
                {
                    FirstName = "Denis",
                    LastName = "Rizov",
                    AuthCode = new string('a', Sha1PasswordLength),
                    ConfirmAuthCode = new string('a', Sha1PasswordLength),
                    Email = "dbrizov@yahoo.com"
                };

                var httpServer = new InMemoryHttpServer("http://localhost/");
                var firstResponse = httpServer.CreatePostRequest("/api/users/register", regUser);

                Assert.AreEqual(HttpStatusCode.Created, firstResponse.StatusCode);
                Assert.IsNotNull(firstResponse.Content);

                var contentString = firstResponse.Content.ReadAsStringAsync().Result;
                var userLoggedModel = JsonConvert.DeserializeObject<UserLoggedModel>(contentString);
                Assert.AreEqual("Denis Rizov", userLoggedModel.DisplayName);
                Assert.IsTrue(userLoggedModel.Id > 0);
                Assert.IsNotNull(userLoggedModel.SessionKey);

                UserLoginModel loginUser = new UserLoginModel()
                {
                    AuthCode = regUser.AuthCode
                };

                var secondResponse = httpServer.CreatePostRequest("api/users/login", loginUser);
                Assert.AreEqual(HttpStatusCode.BadRequest, secondResponse.StatusCode);
            }
        }

        [TestMethod]
        public void Login_EmailIsWhiteSpace_ShouldReturnBadRequest()
        {
            using (new TransactionScope())
            {
                UserRegisterModel regUser = new UserRegisterModel()
                {
                    FirstName = "Denis",
                    LastName = "Rizov",
                    AuthCode = new string('a', Sha1PasswordLength),
                    ConfirmAuthCode = new string('a', Sha1PasswordLength),
                    Email = "dbrizov@yahoo.com"
                };

                var httpServer = new InMemoryHttpServer("http://localhost/");
                var firstResponse = httpServer.CreatePostRequest("/api/users/register", regUser);

                Assert.AreEqual(HttpStatusCode.Created, firstResponse.StatusCode);
                Assert.IsNotNull(firstResponse.Content);

                var contentString = firstResponse.Content.ReadAsStringAsync().Result;
                var userLoggedModel = JsonConvert.DeserializeObject<UserLoggedModel>(contentString);
                Assert.AreEqual("Denis Rizov", userLoggedModel.DisplayName);
                Assert.IsTrue(userLoggedModel.Id > 0);
                Assert.IsNotNull(userLoggedModel.SessionKey);

                UserLoginModel loginUser = new UserLoginModel()
                {
                    AuthCode = regUser.AuthCode,
                    Email = "    "
                };

                var secondResponse = httpServer.CreatePostRequest("api/users/login", loginUser);
                Assert.AreEqual(HttpStatusCode.BadRequest, secondResponse.StatusCode);
            }
        }

        [TestMethod]
        public void Login_NoSuchUserExists_ShouldReturnBadRequest()
        {
            using (new TransactionScope())
            {
                UserRegisterModel regUser = new UserRegisterModel()
                {
                    FirstName = "Denis",
                    LastName = "Rizov",
                    AuthCode = new string('a', Sha1PasswordLength),
                    ConfirmAuthCode = new string('a', Sha1PasswordLength),
                    Email = "dbrizov@yahoo.com"
                };

                var httpServer = new InMemoryHttpServer("http://localhost/");
                var firstResponse = httpServer.CreatePostRequest("/api/users/register", regUser);

                Assert.AreEqual(HttpStatusCode.Created, firstResponse.StatusCode);
                Assert.IsNotNull(firstResponse.Content);

                var contentString = firstResponse.Content.ReadAsStringAsync().Result;
                var userLoggedModel = JsonConvert.DeserializeObject<UserLoggedModel>(contentString);
                Assert.AreEqual("Denis Rizov", userLoggedModel.DisplayName);
                Assert.IsTrue(userLoggedModel.Id > 0);
                Assert.IsNotNull(userLoggedModel.SessionKey);

                UserLoginModel loginUser = new UserLoginModel()
                {
                    AuthCode = "asdasdasd",
                    Email = "asdasd"
                };

                var secondResponse = httpServer.CreatePostRequest("api/users/login", loginUser);
                Assert.AreEqual(HttpStatusCode.BadRequest, secondResponse.StatusCode);
            }
        }

        [TestMethod]
        public void Logout_WhenSessionKeyIsValid_ShouldLogoutTheUser()
        {
            using (new TransactionScope())
            {
                UserRegisterModel regUser = new UserRegisterModel()
                {
                    FirstName = "Denis",
                    LastName = "Rizov",
                    AuthCode = new string('a', Sha1PasswordLength),
                    ConfirmAuthCode = new string('a', Sha1PasswordLength),
                    Email = "dbrizov@yahoo.com"
                };

                var httpServer = new InMemoryHttpServer("http://localhost/");
                var firstResponse = httpServer.CreatePostRequest("/api/users/register", regUser);
                Assert.AreEqual(HttpStatusCode.Created, firstResponse.StatusCode);
                Assert.IsNotNull(firstResponse.Content);

                var contentString = firstResponse.Content.ReadAsStringAsync().Result;
                var userLoggedModel = JsonConvert.DeserializeObject<UserLoggedModel>(contentString);
                Assert.AreEqual("Denis Rizov", userLoggedModel.DisplayName);
                Assert.IsTrue(userLoggedModel.Id > 0);
                Assert.IsNotNull(userLoggedModel.SessionKey);

                var secondResponse = httpServer.CreatePutRequest(
                    "api/users/logout?sessionKey=" + userLoggedModel.SessionKey, null);
                Assert.AreEqual(HttpStatusCode.OK, secondResponse.StatusCode);

                var user = this.db.Users.GetById(userLoggedModel.Id);
                Assert.IsNull(user.SessionKey);
            }
        }

        [TestMethod]
        public void Logout_WhenSessionKeyIsInvalid_ReturnBadRequest()
        {
            using (new TransactionScope())
            {
                UserRegisterModel regUser = new UserRegisterModel()
                {
                    FirstName = "Denis",
                    LastName = "Rizov",
                    AuthCode = new string('a', Sha1PasswordLength),
                    ConfirmAuthCode = new string('a', Sha1PasswordLength),
                    Email = "dbrizov@yahoo.com"
                };

                var httpServer = new InMemoryHttpServer("http://localhost/");
                var firstResponse = httpServer.CreatePostRequest("/api/users/register", regUser);
                Assert.AreEqual(HttpStatusCode.Created, firstResponse.StatusCode);
                Assert.IsNotNull(firstResponse.Content);

                var contentString = firstResponse.Content.ReadAsStringAsync().Result;
                var userLoggedModel = JsonConvert.DeserializeObject<UserLoggedModel>(contentString);
                Assert.AreEqual("Denis Rizov", userLoggedModel.DisplayName);
                Assert.IsTrue(userLoggedModel.Id > 0);
                Assert.IsNotNull(userLoggedModel.SessionKey);

                var secondResponse = httpServer.CreatePutRequest(
                    "api/users/logout?sessionKey=1asdasdasdasd", null);
                Assert.AreEqual(HttpStatusCode.BadRequest, secondResponse.StatusCode);
            }
        }
    }
}
