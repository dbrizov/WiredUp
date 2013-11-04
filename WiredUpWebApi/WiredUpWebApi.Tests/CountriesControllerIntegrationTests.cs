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
using WiredUpWebApi.Models.Constants;
using WiredUpWebApi.Models.CountryModels;

namespace WiredUpWebApi.Tests
{
#if !DEBUG
    [Ignore]
#endif
    [TestClass]
    public class CountriesControllerIntegrationTests
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
        public void GetAll_WhenSessionKeyIsValid_ShouldReturnAllCountries()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(this.user);
                this.db.SaveChanges();

                int countriesCount = this.db.Countries.All().Count();

                var response = this.httpServer.CreateGetRequest(
                    "/api/countries/all?sessionKey=" + this.user.SessionKey);
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

                var countries = this.GetAllCountriesFromResponse(response);
                Assert.IsTrue(countries.Count() > 0 && countries.Count() < 250);
                Assert.AreEqual(countriesCount, countries.Count());
            }
        }

        [TestMethod]
        public void GetAll_WhenSessionKeyIsInvalid_ShouldReturnInternalServerError()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(this.user);
                this.db.SaveChanges();

                int countriesCount = this.db.Countries.All().Count();

                var response = this.httpServer.CreateGetRequest(
                    "/api/countries/all?sessionKey=" + "invalid session key");
                Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
            }
        }

        private IEnumerable<CountryModel> GetAllCountriesFromResponse(HttpResponseMessage response)
        {
            string modelsAsString = response.Content.ReadAsStringAsync().Result;
            var models = JsonConvert.DeserializeObject<IEnumerable<CountryModel>>(modelsAsString);

            return models;
        }
    }
}
