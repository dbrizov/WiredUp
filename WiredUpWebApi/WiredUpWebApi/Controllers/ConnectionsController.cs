using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WiredUpWebApi.Data;
using WiredUpWebApi.Models.ConnectionModels;

namespace WiredUpWebApi.Controllers
{
    public class ConnectionsController : BaseApiController
    {
        public ConnectionsController()
            : base()
        {
        }

        public ConnectionsController(IUnitOfWorkData db)
            : base(db)
        {
        }

        [HttpGet]
        [ActionName("all")]
        public IQueryable<ConnectionModel> GetConnectionsForCurrentUser([FromUri]string sessionKey)
        {
            var user = this.GetUserBySessionKey(sessionKey);
            var connections = user.Connections.Select(ConnectionModel.FromConnection.Compile());

            return connections.AsQueryable();
        }

        [HttpDelete]
        [ActionName("delete")]
        public HttpResponseMessage DeleteConnection([FromUri]int id, [FromUri]string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(() =>
            {
                var user = this.GetUserBySessionKey(sessionKey);
                var firstConnection = user.Connections.FirstOrDefault(c => c.Id == id);
                if (firstConnection == null)
                {
                    throw new ArgumentException(
                        "Invalid connection id. The users does not have such connection");
                }

                var otherUser = firstConnection.OtherUser;
                var secondConnection = otherUser.Connections.FirstOrDefault(c => c.OtherUserId == user.Id);
                if (secondConnection == null)
                {
                    throw new ArgumentException(
                        "The other user is not connection with you");
                }

                this.db.Connections.Delete(firstConnection);
                this.db.Connections.Delete(secondConnection);
                this.db.SaveChanges();

                var response = this.Request.CreateResponse(HttpStatusCode.OK);
                return response;
            });

            return responseMsg;
        }
    }
}