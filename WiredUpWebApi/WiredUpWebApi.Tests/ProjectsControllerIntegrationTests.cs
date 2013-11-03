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
using WiredUpWebApi.Models.ProjectModels;

namespace WiredUpWebApi.Tests
{
#if !DEBUG
    [Ignore]
#endif
    [TestClass]
    public class ProjectsControllerIntegrationTests
    {
        private readonly IUnitOfWorkData db = new UnitOfWorkData();
        private readonly InMemoryHttpServer httpServer = new InMemoryHttpServer("http://localhost");

        private const int Sha1PasswordLength = 40;
        private const int ProjectNameMaxLength = ProjectConstants.NameMaxLength;
        private const int ProjectDescriptionMaxLength = ProjectConstants.DescriptionMaxLength;
        private const int ProjectUrlMaxLength = ProjectConstants.UrlMaxLength;

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
        public void AddProject_WhenDataIsValid_ShouldSaveInDatabase()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(this.firstUser);
                this.db.Users.Add(this.secondUser);
                this.db.SaveChanges();

                var project = new ProjectCreateModel()
                {
                    Description = "Desc",
                    Name = "Name",
                    Url = "Url",
                    MembersIds = new int[] { this.firstUser.Id, this.secondUser.Id }
                };

                var response = this.httpServer.CreatePostRequest(
                    "/api/projects/add?sessionKey=" + this.firstUser.SessionKey, project);
                Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

                var returnedModel = this.GetProjectFromResponse(response);
                Assert.IsNotNull(returnedModel);
                Assert.AreEqual("Name", returnedModel.Name);
                Assert.IsTrue(returnedModel.Id > 0);

                var projectEntity = this.db.Projects.GetById(returnedModel.Id);
                Assert.AreEqual(2, projectEntity.TeamMembers.Count);
            }
        }

        [TestMethod]
        public void AddProject_WhenNameIsNull_ShouldReturnBadRequest()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(this.firstUser);
                this.db.Users.Add(this.secondUser);
                this.db.SaveChanges();

                var project = new ProjectCreateModel()
                {
                    Description = "Desc",
                    Url = "Url",
                    MembersIds = new int[] { this.firstUser.Id, this.secondUser.Id }
                };

                var response = this.httpServer.CreatePostRequest(
                    "/api/projects/add?sessionKey=" + this.firstUser.SessionKey, project);
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public void AddProject_WhenNameIsEmpty_ShouldReturnBadRequest()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(this.firstUser);
                this.db.Users.Add(this.secondUser);
                this.db.SaveChanges();

                var project = new ProjectCreateModel()
                {
                    Description = "Desc",
                    Name = string.Empty,
                    Url = "Url",
                    MembersIds = new int[] { this.firstUser.Id, this.secondUser.Id }
                };

                var response = this.httpServer.CreatePostRequest(
                    "/api/projects/add?sessionKey=" + this.firstUser.SessionKey, project);
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public void AddProject_WhenNameIsWhiteSpace_ShouldReturnBadRequest()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(this.firstUser);
                this.db.Users.Add(this.secondUser);
                this.db.SaveChanges();

                var project = new ProjectCreateModel()
                {
                    Description = "Desc",
                    Name = " ",
                    Url = "Url",
                    MembersIds = new int[] { this.firstUser.Id, this.secondUser.Id }
                };

                var response = this.httpServer.CreatePostRequest(
                    "/api/projects/add?sessionKey=" + this.firstUser.SessionKey, project);
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public void AddProject_WhenNameIsInvalid_ShouldReturnBadRequest()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(this.firstUser);
                this.db.Users.Add(this.secondUser);
                this.db.SaveChanges();

                var project = new ProjectCreateModel()
                {
                    Description = "Desc",
                    Name = new string('a', ProjectNameMaxLength + 1),
                    Url = "Url",
                    MembersIds = new int[] { this.firstUser.Id, this.secondUser.Id }
                };

                var response = this.httpServer.CreatePostRequest(
                    "/api/projects/add?sessionKey=" + this.firstUser.SessionKey, project);
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public void AddProject_WhenDescriptionIsInvalid_ShouldReturnBadRequest()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(this.firstUser);
                this.db.Users.Add(this.secondUser);
                this.db.SaveChanges();

                var project = new ProjectCreateModel()
                {
                    Name = "Desc",
                    Description = new string('a', ProjectDescriptionMaxLength + 1),
                    Url = "Url",
                    MembersIds = new int[] { this.firstUser.Id, this.secondUser.Id }
                };

                var response = this.httpServer.CreatePostRequest(
                    "/api/projects/add?sessionKey=" + this.firstUser.SessionKey, project);
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public void AddProject_WhenUrlIsInvalid_ShouldReturnBadRequest()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(this.firstUser);
                this.db.Users.Add(this.secondUser);
                this.db.SaveChanges();

                var project = new ProjectCreateModel()
                {
                    Name = "Desc",
                    Url = new string('a', ProjectUrlMaxLength + 1),
                    Description = "Url",
                    MembersIds = new int[] { this.firstUser.Id, this.secondUser.Id }
                };

                var response = this.httpServer.CreatePostRequest(
                    "/api/projects/add?sessionKey=" + this.firstUser.SessionKey, project);
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public void GetAllProjects_WhenDataIsValid_ShouldReturnCorrectNumberOfProjects()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(this.firstUser);
                this.db.Users.Add(this.secondUser);
                this.db.SaveChanges();

                var project = new Project()
                {
                    Name = "asd",
                    TeamMembers = new List<User>() { this.firstUser, this.secondUser },
                };

                this.db.Projects.Add(project);
                this.db.SaveChanges();

                var response = this.httpServer.CreateGetRequest(
                    string.Format(
                        "/api/projects/all?userId={0}&sessionKey={1}", 
                        this.firstUser.Id,
                        this.firstUser.SessionKey));
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

                var projects = this.GetProjectsFromResponse(response);
                Assert.AreEqual(1, projects.Count());
            }
        }

        [TestMethod]
        public void GetAllProjects_WhenUserIdIsInvalid_ShouldReturnZeroProjects()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(this.firstUser);
                this.db.Users.Add(this.secondUser);
                this.db.SaveChanges();

                var project = new Project()
                {
                    Name = "asd",
                    TeamMembers = new List<User>() { this.firstUser, this.secondUser },
                };

                this.db.Projects.Add(project);
                this.db.SaveChanges();

                var response = this.httpServer.CreateGetRequest(
                    string.Format(
                        "/api/projects/all?userId={0}&sessionKey={1}",
                        0,
                        this.firstUser.SessionKey));
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

                var projects = this.GetProjectsFromResponse(response);
                Assert.AreEqual(0, projects.Count());
            }
        }

        [TestMethod]
        public void GetProjectDetails_WhenDataIsValid_ShouldReturnProject()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(this.firstUser);
                this.db.Users.Add(this.secondUser);
                this.db.SaveChanges();

                var project = new Project()
                {
                    Description = "desc",
                    Url = "url",
                    Name = "name",
                    TeamMembers = new List<User>() { this.firstUser, this.secondUser },
                };

                this.db.Projects.Add(project);
                this.db.SaveChanges();

                var response = this.httpServer.CreateGetRequest(
                    string.Format(
                        "/api/projects/details?id={0}&sessionKey={1}",
                        project.Id,
                        this.firstUser.SessionKey));
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

                var projectDetails = this.GetProjectDetailsFromResponse(response);
                Assert.AreEqual(2, projectDetails.Members.Count());
                Assert.AreEqual("name", projectDetails.Name);
                Assert.AreEqual("desc", projectDetails.Description);
                Assert.AreEqual("url", projectDetails.Url);
            }
        }

        [TestMethod]
        public void DeleteProject_WhenDataIsValid_ShoulDeleteFromDatabase()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(this.firstUser);
                this.db.Users.Add(this.secondUser);
                this.db.SaveChanges();

                var project = new Project()
                {
                    Description = "desc",
                    Url = "url",
                    Name = "name",
                    TeamMembers = new List<User>() { this.firstUser, this.secondUser },
                };

                this.db.Projects.Add(project);
                this.db.SaveChanges();

                var response = this.httpServer.CreateDeleteRequest(
                    string.Format(
                        "/api/projects/delete?id={0}&sessionKey={1}",
                        project.Id,
                        this.firstUser.SessionKey));
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

                var firstUserProjects = this.db.Projects
                    .All()
                    .Where(p => p.TeamMembers.Any(tm => tm.Id == this.firstUser.Id));
                Assert.AreEqual(0, firstUserProjects.Count());

                var secondUserProjects = this.db.Projects
                    .All()
                    .Where(p => p.TeamMembers.Any(tm => tm.Id == this.secondUser.Id));
                Assert.AreEqual(0, secondUserProjects.Count());
            }
        }

        private ProjectModel GetProjectFromResponse(HttpResponseMessage response)
        {
            var modelAsString = response.Content.ReadAsStringAsync().Result;
            var model = JsonConvert.DeserializeObject<ProjectModel>(modelAsString);

            return model;
        }

        private IEnumerable<ProjectModel> GetProjectsFromResponse(HttpResponseMessage response)
        {
            var modelAsString = response.Content.ReadAsStringAsync().Result;
            var model = JsonConvert.DeserializeObject<IEnumerable<ProjectModel>>(modelAsString);

            return model;
        }

        private ProjectDetailedModel GetProjectDetailsFromResponse(HttpResponseMessage response)
        {
            var modelAsString = response.Content.ReadAsStringAsync().Result;
            var model = JsonConvert.DeserializeObject<ProjectDetailedModel>(modelAsString);

            return model;
        }
    }
}
