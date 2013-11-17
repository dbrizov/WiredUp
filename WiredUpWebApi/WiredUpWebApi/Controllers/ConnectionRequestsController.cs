using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WiredUpWebApi.Data;
using WiredUpWebApi.Models;
using WiredUpWebApi.Models.ConnectionRequestModels;

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
                    throw new ArgumentException("Invalid receiverId");
                }

                if (sender.Id == receiver.Id)
                {
                    throw new ArgumentException("You can't send connection request to yourself");
                }

                var existingConnection =
                    sender.Connections.FirstOrDefault(c => c.OtherUserId == receiver.Id);
                if (existingConnection != null)
                {
                    throw new ArgumentException("You already have a connection with that user");
                }

                // Check if user A is sending a connection request to user B for a second time
                var existingConnectionRequest = receiver.ConnectionRequests
                    .FirstOrDefault(cr => cr.SenderId == sender.Id && cr.ReceiverId == receiver.Id);
                if (existingConnectionRequest != null) 
                {
                    throw new ArgumentException("That user already has a connection request from you");
                }

                // Check if the user A is trying to send a connection request to user B,
                // but user B has already sent a connection request to user A
                existingConnectionRequest = sender.ConnectionRequests
                    .FirstOrDefault(cr => cr.SenderId == receiver.Id && cr.ReceiverId == sender.Id);
                if (existingConnectionRequest != null)
                {
                    throw new ArgumentException("You have a connection request from that user");
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