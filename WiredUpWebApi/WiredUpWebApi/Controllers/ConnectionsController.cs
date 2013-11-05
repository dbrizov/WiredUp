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
                var connection = user.Connections.FirstOrDefault(c => c.Id == id);
                if (connection == null)
                {
                    throw new ArgumentException(
                        "Invalid connection id. The users does not have such connection");
                }

                this.db.Connections.Delete(connection);
                this.db.SaveChanges();

                var response = this.Request.CreateResponse(HttpStatusCode.OK);
                return response;
            });

            return responseMsg;
        }
    }
}