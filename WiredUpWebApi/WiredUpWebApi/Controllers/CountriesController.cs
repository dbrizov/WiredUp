using System;
using System.Linq;
using WiredUpWebApi.Data;
using WiredUpWebApi.Models.Constants;

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
    }
}