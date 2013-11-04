using System;
using System.Linq;
using WiredUpWebApi.Data;
using WiredUpWebApi.Models.Constants;

namespace WiredUpWebApi.Tests
{
    public class CountriesControllerIntegrationTests
    {
        private const int CountryNameMaxLength = CountryConstants.NameMaxLength;

        private readonly IUnitOfWorkData db = new UnitOfWorkData();
        private readonly InMemoryHttpServer httpServer = new InMemoryHttpServer("http://localhost");
    }
}
