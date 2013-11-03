using System;
using System.Linq;
using System.Web.Http;

namespace WiredUpWebApi.Controllers
{
    public class TestController : ApiController
    {
        public string[] GetValues()
        {
            return new string[] { "val1", "val2" };
        }
    }
}
