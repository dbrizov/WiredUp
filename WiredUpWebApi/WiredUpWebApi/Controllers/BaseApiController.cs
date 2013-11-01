using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WiredUpWebApi.Data;
using WiredUpWebApi.Models;

namespace WiredUpWebApi.Controllers
{
    public class BaseApiController : ApiController
    {
        protected readonly IUnitOfWorkData db;

        public BaseApiController()
            : this(new UnitOfWorkData())
        {
        }

        public BaseApiController(IUnitOfWorkData db)
        {
            this.db = db;
        }

        protected T PerformOperationAndHandleExceptions<T>(Func<T> operation)
        {
            try
            {
                return operation();
            }
            catch (Exception ex)
            {
                var errResponse = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
                throw new HttpResponseException(errResponse);
            }
        }

        protected User GetUserBySessionKey(string sessionKey)
        {
            var user = this.db.Users.All().Where(
                    u => u.SessionKey == sessionKey).FirstOrDefault();

            if (user == null)
            {
                throw new ArgumentException("Invalid session key");
            }

            return user;
        }

        protected bool IsSessionKeyValid(string sessionKey)
        {
            var user = this.db.Users.All().Where(
                    u => u.SessionKey == sessionKey).FirstOrDefault();

            bool isValid = user != null;
            return isValid;
        }
    }
}
