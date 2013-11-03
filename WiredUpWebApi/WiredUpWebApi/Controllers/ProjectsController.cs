using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WiredUpWebApi.Data;
using WiredUpWebApi.Models;
using WiredUpWebApi.Models.Constants;
using WiredUpWebApi.Models.ProjectModels;

namespace WiredUpWebApi.Controllers
{
    public class ProjectsController : BaseApiController
    {
        private const int ProjectNameMaxLength = ProjectConstants.NameMaxLength;
        private const int ProjectDescriptionMaxLength = ProjectConstants.DescriptionMaxLength;
        private const int ProjectUrlMaxLength = ProjectConstants.UrlMaxLength;

        public ProjectsController()
            : base()
        {
        }

        public ProjectsController(IUnitOfWorkData db)
            : base(db)
        {
        }

        [HttpPost]
        [ActionName("add")]
        public HttpResponseMessage AddProject(
            [FromBody]ProjectCreateModel model, [FromUri]string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(() =>
            {
                this.ValidateProjectName(model.Name);
                this.ValidateProjectDescription(model.Description);
                this.ValidateProjectUrl(model.Url);

                var user = this.GetUserBySessionKey(sessionKey);
                var project = new Project()
                {
                    Description = model.Description,
                    Name = model.Name,
                    Url = model.Url
                };

                project.TeamMembers.Add(user);
                foreach (int userId in model.MembersIds)
                {
                    var member = this.db.Users.GetById(userId);
                    if (member == null)
                    {
                        throw new ArgumentException(
                            string.Format("A user with id={0} does not exist", userId));
                    }

                    project.TeamMembers.Add(member);
                }

                this.db.Projects.Add(project);
                this.db.SaveChanges();

                var returnData = new ProjectModel()
                {
                    Id = project.Id,
                    Name = project.Name
                };

                var response = this.Request.CreateResponse(HttpStatusCode.Created, returnData);
                return response;
            });

            return responseMsg;
        }

        [HttpGet]
        [ActionName("all")]
        public IQueryable<ProjectModel> GetAllProjects([FromUri]int userId, [FromUri]string sessionKey)
        {
            if (!this.IsSessionKeyValid(sessionKey))
            {
                throw new ArgumentException("Invalid sesionKey");
            }

            var projects = this.db.Projects
                .All()
                .Where(p => p.TeamMembers.Any(tm => tm.Id == userId))
                .Select(ProjectModel.FromProject);

            return projects;
        }

        [HttpGet]
        [ActionName("details")]
        public ProjectDetailedModel GetProjectDetails([FromUri]int id, [FromUri]string sessionKey)
        {
            if (!this.IsSessionKeyValid(sessionKey))
            {
                throw new ArgumentException("Invalid session key");
            }

            var project = this.db.Projects
                .All()
                .Where(p => p.Id == id)
                .Select(ProjectDetailedModel.FromProject)
                .FirstOrDefault();

            return project;
        }

        [HttpPut]
        [ActionName("edit")]
        public HttpResponseMessage EditProject(
            [FromBody]ProjectEditModel model, [FromUri]string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(() =>
            {
                this.ValidateProjectName(model.Name);
                this.ValidateProjectDescription(model.Description);
                this.ValidateProjectUrl(model.Url);

                var user = this.GetUserBySessionKey(sessionKey);

                var project = this.db.Projects.GetById(model.Id);
                project.Description = model.Description;
                project.Name = model.Name;
                project.Url = model.Url;

                project.TeamMembers.Clear();
                project.TeamMembers.Add(user);
                foreach (int userId in model.MembersIds)
                {
                    var member = this.db.Users.GetById(userId);
                    if (member == null)
                    {
                        throw new ArgumentException(
                            string.Format("A user with id={0} does not exist", userId));
                    }

                    project.TeamMembers.Add(member);
                }

                this.db.Projects.Update(project);
                this.db.SaveChanges();

                var returnData = new ProjectModel()
                {
                    Id = project.Id,
                    Name = project.Name
                };

                var response = this.Request.CreateResponse(HttpStatusCode.Created, returnData);
                return response;
            });

            return responseMsg;
        }

        [HttpDelete]
        [ActionName("delete")]
        public HttpResponseMessage DeleteProject([FromUri]int id, [FromUri]string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(() =>
            {
                var user = this.GetUserBySessionKey(sessionKey);
                var project = user.Projects.FirstOrDefault(p => p.Id == id);
                if (project == null)
                {
                    throw new ArgumentException(
                        "The user does not have such project, or you are trying to delete someone elses project");
                }

                this.db.Projects.Delete(project);
                this.db.SaveChanges();

                var response = this.Request.CreateResponse(HttpStatusCode.OK);
                return response;
            });

            return responseMsg;
        }

        private void ValidateProjectName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("The 'Name' is required");
            }

            if (name.Length > ProjectNameMaxLength)
            {
                throw new ArgumentException(
                    string.Format(
                        "The 'Name' of the project must be less than {0} characters long",
                        ProjectNameMaxLength));
            }
        }

        private void ValidateProjectDescription(string description)
        {
            if (description != null)
            {
                if (description.Length > ProjectDescriptionMaxLength)
                {
                    throw new ArgumentException(
                        string.Format(
                            "The 'Project Description' must be less than {0} charachtes long",
                            ProjectDescriptionMaxLength));
                }
            }
        }

        private void ValidateProjectUrl(string url)
        {
            if (url != null)
            {
                if (url.Length > ProjectUrlMaxLength)
                {
                    throw new ArgumentException(
                        string.Format(
                            "The 'Project Description' must be less than {0} charachtes long",
                            ProjectUrlMaxLength));
                }
            }
        }
    }
}