using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WiredUpWebApi.Data;
using WiredUpWebApi.Models;
using WiredUpWebApi.Models.ConnectionRequestModels;
using WiredUpWebApi.Models.UserModels;

namespace WiredUpWebApi.Controllers
{
    public class ConnectionRequestsController : BaseApiController
    {
        public ConnectionRequestsController()
            : base()
        {
        }

        public ConnectionRequestsController(IUnitOfWorkData db)
            : base(db)
        {
        }

        [HttpPost]
        [ActionName("send")]
        public HttpResponseMessage SendRequest(
            [FromBody]ConnectionRequestSendModel model, [FromUri]string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(() =>
            {
                var sender = this.GetUserBySessionKey(sessionKey);
                var receiver = this.db.Users.All().FirstOrDefault(u => u.Id == model.ReceiverId);
                if (receiver == null)
                {
                    throw new ArgumentException("Invalid recevierId");
                }

                var connectionRequest = new ConnectionRequest()
                {
                    Receiver = receiver,
                    Sender = sender
                };

                receiver.ConnectionRequests.Add(connectionRequest);
                this.db.Users.Update(receiver);
                this.db.SaveChanges();

                var response = this.Request.CreateResponse(HttpStatusCode.Created);
                return response;
            });

            return responseMsg;
        }

        [HttpPost]
        [ActionName("accept")]
        public HttpResponseMessage AcceptRequest([FromUri]int id, [FromUri]string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(() =>
            {
                var recevier = this.GetUserBySessionKey(sessionKey);
                var connectionRequest =
                    recevier.ConnectionRequests.FirstOrDefault(c => c.Id == id);

                if (connectionRequest == null)
                {
                    throw new ArgumentException(
                        "Invalid connection request id. The user does not have such connection request");
                }

                var sender = connectionRequest.Sender;

                var firstConnection = new Connection()
                {
                    User = sender,
                    OtherUser = recevier
                };

                this.db.Connections.Add(firstConnection);

                var secondConnection = new Connection()
                {
                    User = recevier,
                    OtherUser = sender
                };

                this.db.Connections.Add(secondConnection);

                this.db.ConnectionRequests.Delete(connectionRequest);

                this.db.SaveChanges();

                var response = this.Request.CreateResponse(HttpStatusCode.Created);
                return response;
            });

            return responseMsg;
        }

        [HttpPost]
        [ActionName("decline")]
        public HttpResponseMessage DeclineRequest([FromUri]int id, [FromUri]string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(() =>
            {
                var recevier = this.GetUserBySessionKey(sessionKey);
                var connectionRequest =
                    recevier.ConnectionRequests.FirstOrDefault(c => c.Id == id);

                if (connectionRequest == null)
                {
                    throw new ArgumentException(
                        "Invalid connection request id. The user does not have such connection request");
                }

                this.db.ConnectionRequests.Delete(connectionRequest);
                this.db.SaveChanges();

                var response = this.Request.CreateResponse(HttpStatusCode.OK);
                return response;
            });

            return responseMsg;
        }

        [HttpGet]
        [ActionName("all")]
        public IQueryable<ConnectionRequestModel> GetConnectionsForCurrentUser([FromUri]string sessionKey)
        {
            var user = this.GetUserBySessionKey(sessionKey);
            var connections =
                user.ConnectionRequests.Select(ConnectionRequestModel.FromConnectionRequest.Compile());

            return connections.AsQueryable();
        }
    }
}