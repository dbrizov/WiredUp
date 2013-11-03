using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Transactions;
using WiredUpWebApi.Data;
using WiredUpWebApi.Models;

namespace WiredUpWebApi.Tests
{
#if !DEBUG
    [Ignore]
#endif
    [TestClass]
    public class CertificatesControllerIntegrationTests
    {
        private const int Sha1PasswordLength = 40;

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
        public void MyTestMethod()
        {
            using (new TransactionScope())
            {

            }
        }
    }
}
