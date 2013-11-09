using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WiredUpWebApi.Data;
using WiredUpWebApi.Models;
using WiredUpWebApi.Models.Constants;
using WiredUpWebApi.Models.SkillModels;

namespace WiredUpWebApi.Controllers
{
    public class SkillsController : BaseApiController
    {
        private const int SkillNameMaxLength = SkillConstants.NameMaxLength;

        public SkillsController()
            : base()
        {
        }

        public SkillsController(IUnitOfWorkData db)
            : base(db)
        {
        }

        [HttpPost]
        [ActionName("add")]
        public HttpResponseMessage AddSkill(
            [FromBody]SkillModel model, [FromUri]string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(() =>
            {
                this.ValidateSkillName(model.Name);

                var user = this.GetUserBySessionKey(sessionKey);
                var skill = this.db.Skills.All()
                    .FirstOrDefault(s => s.Name.ToLower() == model.Name.Trim().ToLower());

                if (skill == null)
                {
                    skill = new Skill()
                    {
                        Name = model.Name.Trim()
                    };
                }

                user.Skills.Add(skill);
                this.db.Users.Update(user);
                this.db.SaveChanges();

                var returnData = new SkillModel()
                {
                    Id = skill.Id,
                    Name = skill.Name
                };

                var response = this.Request.CreateResponse(HttpStatusCode.Created, returnData);
                return response;
            });

            return responseMsg;
        }

        [HttpGet]
        [ActionName("all")]
        public IQueryable<SkillModel> GetAllSkills([FromUri]string sessionKey)
        {
            if (!this.IsSessionKeyValid(sessionKey))
            {
                throw new ArgumentException("Invalid session key");
            }

            var skills = this.db.Skills.All().Select(SkillModel.FromSkill);
            return skills;
        }

        [HttpGet]
        [ActionName("all")]
        public IQueryable<SkillModel> GetAllSkillsForUser(
            [FromUri]int userId, [FromUri]string sessionKey)
        {
            if (!this.IsSessionKeyValid(sessionKey))
            {
                throw new ArgumentException("Invalid session key");
            }

            var skills = this.db.Skills
                .All()
                .Where(c => c.Users.Any(u => u.Id == userId))
                .Select(SkillModel.FromSkill);
            return skills;
        }

        [HttpPut]
        [ActionName("remove")]
        // Removes a skill from the user only
        public HttpResponseMessage RemoveSkill(
            [FromBody]SkillModel model, [FromUri]string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(() =>
            {
                var user = this.GetUserBySessionKey(sessionKey);

                var skill = user.Skills.FirstOrDefault(s => s.Id == model.Id);
                if (skill == null)
                {
                    throw new ArgumentException("The user does not have such skill");
                }

                user.Skills.Remove(skill);
                this.db.Users.Update(user);
                this.db.SaveChanges();

                var response = this.Request.CreateResponse(HttpStatusCode.OK);
                return response;
            });

            return responseMsg;
        }

        private void ValidateSkillName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("The 'Name' is required");
            }

            if (name.Length > SkillNameMaxLength)
            {
                throw new ArgumentException(
                    string.Format("The 'Name' must be less than {0} characters long", SkillNameMaxLength));
            }
        }
    }
}