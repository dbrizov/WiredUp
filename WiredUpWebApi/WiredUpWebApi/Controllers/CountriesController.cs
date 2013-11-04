using System;
using System.Linq;
using System.Web.Http;
using WiredUpWebApi.Data;
using WiredUpWebApi.Models.Constants;
using WiredUpWebApi.Models.CountryModels;

namespace WiredUpWebApi.Controllers
{
    public class CountriesController : BaseApiController
    {
        private const int CountryNameMaxLength = CountryConstants.NameMaxLength;

        public CountriesController()
            : base()
        {
        }

        public CountriesController(IUnitOfWorkData db)
            : base(db)
        {
        }

        [HttpGet]
        [ActionName("all")]
        public IQueryable<CountryModel> GetAllCountries([FromUri]string sessionKey)
        {
            if (!this.IsSessionKeyValid(sessionKey))
            {
                throw new ArgumentException("Invalid session key");
            }

            var countries = this.db.Countries
                .All()
                .Select(CountryModel.FromCountry)
                .OrderBy(c => c.Name);

            return countries;
        }
    }
}