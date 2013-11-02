using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WiredUpWebApi.Data;
using WiredUpWebApi.Models;
using WiredUpWebApi.Models.Constants;
using WiredUpWebApi.Models.UserPostModels;

namespace WiredUpWebApi.Controllers
{
    public class UserPostsController : BaseApiController
    {
        private const int PostContentMaxLength = PostConstants.ContentMaxLength;

        public UserPostsController()
            : base()
        {
        }

        public UserPostsController(IUnitOfWorkData db)
            : base(db)
        {
        }

        [HttpPost]
        [ActionName("create")]
        public HttpResponseMessage CreatePost([FromBody]UserPostCreateModel model, [FromUri]string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(() =>
            {
                this.ValidatePostContent(model.Content);

                var user = this.GetUserBySessionKey(sessionKey);
                var newPost = new UserPost()
                {
                    Content = model.Content,
                    PostDate = DateTime.Now
                };

                user.Posts.Add(newPost);
                this.db.Users.Update(user);
                this.db.SaveChanges();

                var returnModel = new UserPostModel()
                {
                    Id = newPost.Id,
                    Content = model.Content,
                    PostedBy = user.FirstName + " " + user.LastName,
                    PostDate = newPost.PostDate
                };

                var response = this.Request.CreateResponse(HttpStatusCode.Created, returnModel);
                return response;
            });

            return responseMsg;
        }

        [HttpGet]
        [ActionName("all")]
        public IQueryable<UserPostModel> GetAll([FromUri]int userId, [FromUri]string sessionKey)
        {
            if (!this.IsSessionKeyValid(sessionKey))
            {
                throw new ArgumentException("Invalid session key");
            }

            var posts = this.db.UserPosts
                .All()
                .Include(p => p.User)
                .Where(p => p.UserId == userId)
                .Select(UserPostModel.FromUserPost);

            return posts;
        }

        [HttpGet]
        [ActionName("details")]
        public UserPostModel GetSingle([FromUri]int id, [FromUri]int userId, [FromUri]string sessionKey)
        {
            if (!this.IsSessionKeyValid(sessionKey))
            {
                throw new ArgumentException("Invalid session key");
            }

            var post = this.db.UserPosts
                .All()
                .Where(p => p.Id == id && p.UserId == userId)
                .Select(UserPostModel.FromUserPost)
                .FirstOrDefault();

            if (post == null)
            {
                throw new ArgumentException("A post with such id does not exist", "id");
            }

            return post;
        }

        private void ValidatePostContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentException("The 'Content' of the post is required");
            }

            if (content.Length > PostContentMaxLength)
            {
                throw new ArgumentException(string.Format(
                    "The 'Content' of the post must be less than {0} characters long",
                    PostContentMaxLength));
            }
        }
    }
}