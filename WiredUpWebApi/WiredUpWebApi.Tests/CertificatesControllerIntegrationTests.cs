using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Transactions;
using WiredUpWebApi.Data;
using WiredUpWebApi.Models;
using WiredUpWebApi.Models.CertificateModels;
using WiredUpWebApi.Models.Constants;

namespace WiredUpWebApi.Tests
{
#if !DEBUG
    [Ignore]
#endif
    [TestClass]
    public class CertificatesControllerIntegrationTests
    {
        private const int Sha1PasswordLength = 40;
        private const int CertificateNameMaxLength = CertificateConstants.NameMaxLength;
        private const int CertificateUrlMaxLength = CertificateConstants.UrlMaxLength;

        private readonly IUnitOfWorkData db = new UnitOfWorkData();
        private readonly InMemoryHttpServer httpServer = new InMemoryHttpServer("http://localhost");

        private readonly User user = new User()
        {
            FirstName = "Denis",
            LastName = "Rizov",
            AuthCode = new string('a', Sha1PasswordLength),
            Email = "dbrizov@yahoo.com",
            SessionKey = "first"
        };

        [TestMethod]
        public void AddCertificate_WhenDataIsValid_ShouldSaveInDatabase()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(user);
                this.db.SaveChanges();

                var certificate = new CertificateCreateModel()
                {
                    Name = "asd",
                    Url = "asd"
                };

                var response = this.httpServer.CreatePostRequest(
                    string.Format("/api/certificates/add?sessionKey={0}", this.user.SessionKey),
                    certificate);
                Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

                var returnedData = this.GetCertificateModelFromResponse(response);
                Assert.IsNotNull(returnedData);
                Assert.AreEqual(0, this.user.Certificates.Count());
            }
        }

        [TestMethod]
        public void AddCertificate_WhenNameIsNull_ShouldReturnBadRequest()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(user);
                this.db.SaveChanges();

                var certificate = new CertificateCreateModel()
                {
                    Url = "asd"
                };

                var response = this.httpServer.CreatePostRequest(
                    string.Format("/api/certificates/add?sessionKey={0}", this.user.SessionKey),
                    certificate);
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public void AddCertificate_WhenNameIsWhiteSpace_ShouldReturnBadRequest()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(user);
                this.db.SaveChanges();

                var certificate = new CertificateCreateModel()
                {
                    Name = " ",
                    Url = "asd"
                };

                var response = this.httpServer.CreatePostRequest(
                    string.Format("/api/certificates/add?sessionKey={0}", this.user.SessionKey),
                    certificate);
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public void AddCertificate_WhenNameIsEmptyString_ShouldReturnBadRequest()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(user);
                this.db.SaveChanges();

                var certificate = new CertificateCreateModel()
                {
                    Name = string.Empty,
                    Url = "asd"
                };

                var response = this.httpServer.CreatePostRequest(
                    string.Format("/api/certificates/add?sessionKey={0}", this.user.SessionKey),
                    certificate);
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public void AddCertificate_WhenNameIsTooBig_ShouldReturnBadRequest()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(user);
                this.db.SaveChanges();

                var certificate = new CertificateCreateModel()
                {
                    Name = new string('a', CertificateNameMaxLength + 1),
                    Url = "asd"
                };

                var response = this.httpServer.CreatePostRequest(
                    string.Format("/api/certificates/add?sessionKey={0}", this.user.SessionKey),
                    certificate);
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public void AddCertificate_WhenUrlIsTooBig_ShouldReturnBadRequest()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(user);
                this.db.SaveChanges();

                var certificate = new CertificateCreateModel()
                {
                    Url = new string('a', CertificateUrlMaxLength + 1),
                    Name = "asd"
                };

                var response = this.httpServer.CreatePostRequest(
                    string.Format("/api/certificates/add?sessionKey={0}", this.user.SessionKey),
                    certificate);
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public void GetAllCertificates_WhenDataIsCorrect_ShouldReturnCorrectNumberOfCertificates()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(user);
                this.db.SaveChanges();

                var certificate = new Certificate()
                {
                    Name = "asd",
                    Owner = this.user
                };

                this.db.Certificates.Add(certificate);
                this.db.SaveChanges();

                var response = this.httpServer.CreateGetRequest(
                    string.Format("/api/certificates/all?userId={0}&sessionKey={1}",
                        this.user.Id,
                        this.user.SessionKey));
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

                var data = this.GetAllCertificatesFromResponse(response);
                Assert.AreEqual(1, data.Count());
            }
        }

        [TestMethod]
        public void GetAllCertificates_WhenUserIdIsInvalid_ShouldReturnZeroCertificates()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(user);
                this.db.SaveChanges();

                var certificate = new Certificate()
                {
                    Name = "asd",
                    Owner = this.user
                };

                this.db.Certificates.Add(certificate);
                this.db.SaveChanges();

                var response = this.httpServer.CreateGetRequest(
                    string.Format("/api/certificates/all?userId={0}&sessionKey={1}",
                        0,
                        this.user.SessionKey));
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

                var data = this.GetAllCertificatesFromResponse(response);
                Assert.AreEqual(0, data.Count());
            }
        }

        [TestMethod]
        public void GetAllCertificates_WhenSessionKeyIsInvalid_ShouldReturnInternalServerError()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(user);
                this.db.SaveChanges();

                var certificate = new Certificate()
                {
                    Name = "asd",
                    Owner = this.user
                };

                this.db.Certificates.Add(certificate);
                this.db.SaveChanges();

                var response = this.httpServer.CreateGetRequest(
                    string.Format("/api/certificates/all?userId={0}&sessionKey={1}",
                        this.user.Id,
                        "invalid session key"));
                Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
            }
        }

        [TestMethod]
        public void GetCertificateDetails_WhenDataIsCorrect_ShouldReturnCorrectData()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(user);
                this.db.SaveChanges();

                var certificate = new Certificate()
                {
                    Name = "asd",
                    Owner = this.user
                };

                this.db.Certificates.Add(certificate);
                this.db.SaveChanges();

                var response = this.httpServer.CreateGetRequest(
                    string.Format("/api/certificates/details?id={0}&sessionKey={1}",
                        certificate.Id,
                        this.user.SessionKey));
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

                var data = this.GetCertificateModelFromResponse(response);
                Assert.AreEqual("asd", data.Name);
            }
        }

        [TestMethod]
        public void DeleteCertificate_WhenDataIsValid_ShouldDeleteFromDatabase()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(user);
                this.db.SaveChanges();

                var certificate = new Certificate()
                {
                    Name = "asd",
                    Owner = this.user
                };

                this.db.Certificates.Add(certificate);
                this.db.SaveChanges();

                var response = this.httpServer.CreateDeleteRequest(
                    string.Format("/api/certificates/delete?id={0}&sessionKey={1}",
                        certificate.Id,
                        this.user.SessionKey));
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

                var certificates = this.db.Certificates
                    .All()
                    .Where(c => c.OwnerId == this.user.Id);
                Assert.AreEqual(0, certificates.Count());
            }
        }

        [TestMethod]
        public void DeleteCertificate_WhenDeletingCertificateOfOtherUser_ShouldReturnBadRequest()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(user);
                var secondUser = new User()
                {
                    FirstName = "Pesho",
                    LastName = "Peshev",
                    AuthCode = new string('a', Sha1PasswordLength),
                    Email = "pesho@yahoo.com",
                    SessionKey = "second"
                };

                this.db.Users.Add(secondUser);
                this.db.SaveChanges();

                var certificate = new Certificate()
                {
                    Name = "asd",
                    Owner = this.user
                };

                this.db.Certificates.Add(certificate);
                this.db.SaveChanges();

                var response = this.httpServer.CreateDeleteRequest(
                    string.Format("/api/certificates/delete?id={0}&sessionKey={1}",
                        certificate.Id,
                        secondUser.SessionKey));
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        private CertificateModel GetCertificateModelFromResponse(HttpResponseMessage response)
        {
            string modelAsString = response.Content.ReadAsStringAsync().Result;
            var model = JsonConvert.DeserializeObject<CertificateModel>(modelAsString);

            return model;
        }

        private IEnumerable<CertificateModel> GetAllCertificatesFromResponse(HttpResponseMessage response)
        {
            string modelAsString = response.Content.ReadAsStringAsync().Result;
            var model = JsonConvert.DeserializeObject<IEnumerable<CertificateModel>>(modelAsString);

            return model;
        }
    }
}
