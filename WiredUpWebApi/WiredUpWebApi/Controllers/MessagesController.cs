using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WiredUpWebApi.Models.MessageModels;

namespace WiredUpWebApi.Controllers
{
    public class MessagesController : BaseApiController
    {
        [HttpPost]
        public HttpResponseMessage SendMessage([FromBody]MessageSendModel model, [FromUri]string sessionKey)
        {
            throw new NotImplementedException();
        }
    }
}