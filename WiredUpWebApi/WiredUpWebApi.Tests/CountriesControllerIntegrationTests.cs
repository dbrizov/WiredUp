using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Net;
using WiredUpWebApi.Data;
using WiredUpWebApi.Models.Constants;

namespace WiredUpWebApi.Tests
{
#if !DEBUG
    [Ignore]
#endif
    [TestClass]
    public class CountriesControllerIntegrationTests
    {
        private const int CountryNameMaxLength = CountryConstants.NameMaxLength;
        
        private const string AdminCode = "hh12890j109nu190g02b78r0f91bf890nu902n0d81h";

        private readonly IUnitOfWorkData db = new UnitOfWorkData();
        private readonly InMemoryHttpServer httpServer = new InMemoryHttpServer("http://localhost");

        //[TestMethod]
        //public void GetAll_WhenSessionKeyIsValid_ShouldReturnAllCountries()
        //{
        //    int countriesCount = this.db.Countries.All().Count();

        //    var response = this.httpServer.CreatePostRequest(
        //        "/api/countries/addAll?adminCode=" + AdminCode, (object)null);
        //    Assert.AreEqual(HttpStatusCode.Created, response.
        //}
    }
}
