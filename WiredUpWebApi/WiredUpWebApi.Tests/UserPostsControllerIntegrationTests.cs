using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Transactions;
using WiredUpWebApi.Data;
using WiredUpWebApi.Models;
using WiredUpWebApi.Models.Constants;
using WiredUpWebApi.Models.UserPostModels;

namespace WiredUpWebApi.Tests
{
#if !DEBUG
    [Ignore]
#endif
    [TestClass]
    public class UserPostsControllerIntegrationTests
    {
        private const int Sha1PasswordLength = 40;
        private const int UserPostContentMaxLength = PostConstants.ContentMaxLength;

        private readonly IUnitOfWorkData db = new UnitOfWorkData();
        private readonly InMemoryHttpServer httpServer = new InMemoryHttpServer("http://localhost");

        private readonly User firstUser = new User()
        {
            FirstName = "Denis",
            LastName = "Rizov",
            AuthCode = new string('a', Sha1PasswordLength),
            Email = "dbrizov@yahoo.com",
            SessionKey = "first"
        };

        private readonly User secondUser = new User()
        {
            FirstName = "Pesho",
            LastName = "Peshev",
            AuthCode = new string('a', Sha1PasswordLength),
            Email = "pesho@yahoo.com",
            SessionKey = "second"
        };

        [TestMethod]
        public void CreatePost_WhenDataIsValid_ShouldSaveInDatabase()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(this.firstUser);
                this.db.SaveChanges();

                var newPost = new UserPostCreateModel()
                {
                    Content = "Content"
                };

                var response = this.httpServer.CreatePostRequest(
                    "/api/userposts/create?sessionKey=" + this.firstUser.SessionKey, newPost);
                Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

                var returnedPostModel = this.GetUserPostModelFromResponse(response);
                Assert.IsNotNull(returnedPostModel);
                Assert.IsNotNull(returnedPostModel.PostDate);
                Assert.AreEqual(this.firstUser.FirstName + " " + this.firstUser.LastName, returnedPostModel.PostedBy);
                Assert.AreEqual(newPost.Content, returnedPostModel.Content);
                Assert.IsTrue(returnedPostModel.Id > 0);
            }
        }

        [TestMethod]
        public void CreatePost_WhenContentIsNull_ShouldReturnBadRequest()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(this.firstUser);
                this.db.SaveChanges();

                var newPost = new UserPostCreateModel()
                {
                };

                var response = this.httpServer.CreatePostRequest(
                    "/api/userposts/create?sessionKey=" + this.firstUser.SessionKey, newPost);
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public void CreatePost_WhenContentIsEmpty_ShouldReturnBadRequest()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(this.firstUser);
                this.db.SaveChanges();

                var newPost = new UserPostCreateModel()
                {
                    Content = string.Empty
                };

                var response = this.httpServer.CreatePostRequest(
                    "/api/userposts/create?sessionKey=" + this.firstUser.SessionKey, newPost);
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public void CreatePost_WhenContentIsWhiteSpace_ShouldReturnBadRequest()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(this.firstUser);
                this.db.SaveChanges();

                var newPost = new UserPostCreateModel()
                {
                    Content = " ",
                };

                var response = this.httpServer.CreatePostRequest(
                    "/api/userposts/create?sessionKey=" + this.firstUser.SessionKey, newPost);
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public void CreatePost_WhenContentIsTooBig_ShouldReturnBadRequest()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(this.firstUser);
                this.db.SaveChanges();

                var newPost = new UserPostCreateModel()
                {
                    Content = new string('a', UserPostContentMaxLength + 1)
                };

                var response = this.httpServer.CreatePostRequest(
                    "/api/userposts/create?sessionKey=" + this.firstUser.SessionKey, newPost);
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public void CreatePost_WhenSessionKeyIsInvalid_ShouldReturnBadRequest()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(this.firstUser);
                this.db.SaveChanges();

                var newPost = new UserPostCreateModel()
                {
                    Content = " ",
                };

                var response = this.httpServer.CreatePostRequest(
                    "/api/userposts/create?sessionKey=invalid", newPost);
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public void GetAllPosts_ShouldReturnCorrectNumberOfPosts()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(this.firstUser);
                this.db.SaveChanges();

                var firstPost = new UserPost()
                {
                    Content = "Content",
                    PostDate = DateTime.Now
                };

                var secondPost = new UserPost()
                {
                    Content = "Content",
                    PostDate = DateTime.Now
                };

                this.firstUser.Posts.Add(firstPost);
                this.firstUser.Posts.Add(secondPost);
                this.db.Users.Update(this.firstUser);
                this.db.SaveChanges();

                var response = this.httpServer.CreateGetRequest(
                    string.Format(
                        "/api/userposts/all?userId={0}&sessionKey={1}",
                        this.firstUser.Id, this.firstUser.SessionKey));
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

                var posts = this.GetPostsFromResponse(response);
                Assert.IsNotNull(posts);
                Assert.AreEqual(2, posts.Count());
            }
        }

        private UserPostModel GetUserPostModelFromResponse(HttpResponseMessage response)
        {
            string modelAsString = response.Content.ReadAsStringAsync().Result;
            var model = JsonConvert.DeserializeObject<UserPostModel>(modelAsString);

            return model;
        }

        private IEnumerable<UserPostModel> GetPostsFromResponse(HttpResponseMessage response)
        {
            string postsAsString = response.Content.ReadAsStringAsync().Result;
            var posts = JsonConvert.DeserializeObject<IEnumerable<UserPostModel>>(postsAsString);

            return posts;
        }
    }
}
