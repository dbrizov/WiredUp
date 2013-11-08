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
using WiredUpWebApi.Models.SkillModels;

namespace WiredUpWebApi.Tests
{
#if !DEBUG
    [Ignore]
#endif
    [TestClass]
    public class SkillsControllerIntegrationTests
    {
        private const int Sha1PasswordLength = 40;
        private const int SkillNameMaxName = SkillConstants.NameMaxLength;

        private readonly IUnitOfWorkData db = new UnitOfWorkData();
        private readonly InMemoryHttpServer httpServer = new InMemoryHttpServer("http://localhost");

        private readonly User user = new User()
        {
            FirstName = "Denis",
            LastName = "Rizov",
            AuthCode = new string('a', Sha1PasswordLength),
            Email = "dbrizov@yahoo.com",
            SessionKey = "first"
        };

        [TestMethod]
        public void AddSkill_WhenDataIsValid_ShouldSaveInDatabase()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(this.user);
                this.db.SaveChanges();

                var skill = new SkillModel()
                {
                    Name = "asd"
                };

                var response = this.httpServer.CreatePostRequest(
                    string.Format(
                        "/api/skills/add?sessionKey={0}", this.user.SessionKey), skill);
                Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

                var data = this.GetSkillModelFromResponse(response);
                Assert.AreEqual("asd", data.Name);
                Assert.IsTrue(data.Id > 0);

                var skillEntity = this.db.Skills
                    .All()
                    .Where(s => s.Users.Any(u => u.Id == this.user.Id))
                    .FirstOrDefault();
                Assert.IsNotNull(skillEntity);
            }
        }

        [TestMethod]
        public void AddSkill_WhenNameIsNull_ShouldReturnBadRequest()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(this.user);
                this.db.SaveChanges();

                var skill = new SkillModel();

                var response = this.httpServer.CreatePostRequest(
                    string.Format(
                        "/api/skills/add?sessionKey={0}", this.user.SessionKey), skill);
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public void AddSkill_WhenNameIsEmpty_ShouldReturnBadRequest()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(this.user);
                this.db.SaveChanges();

                var skill = new SkillModel()
                {
                    Name = string.Empty
                };

                var response = this.httpServer.CreatePostRequest(
                    string.Format(
                        "/api/skills/add?sessionKey={0}", this.user.SessionKey), skill);
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public void AddSkill_WhenNameIsWhiteSpace_ShouldReturnBadRequest()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(this.user);
                this.db.SaveChanges();

                var skill = new SkillModel()
                {
                    Name = " "
                };

                var response = this.httpServer.CreatePostRequest(
                    string.Format(
                        "/api/skills/add?sessionKey={0}", this.user.SessionKey), skill);
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public void AddSkill_WhenNameIsTooBig_ShouldReturnBadRequest()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(this.user);
                this.db.SaveChanges();

                var skill = new SkillModel()
                {
                    Name = new string('a', SkillNameMaxName + 1)
                };

                var response = this.httpServer.CreatePostRequest(
                    string.Format(
                        "/api/skills/add?sessionKey={0}", this.user.SessionKey), skill);
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public void GetAllSkills_WhenDataIsCorrect_ShouldReturnCorrectSkills()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(this.user);
                this.db.SaveChanges();

                this.user.Skills.Add(new Skill() { Name = "asd" });
                this.user.Skills.Add(new Skill() { Name = "asd" });
                this.user.Skills.Add(new Skill() { Name = "asd" });
                this.db.Users.Update(this.user);
                this.db.SaveChanges();

                int skillsCount = this.user.Skills.Count();

                var response = this.httpServer.CreateGetRequest(
                    string.Format(
                        "/api/skills/all?userId={0}&sessionKey={1}", this.user.Id, this.user.SessionKey));

                var skills = this.GetAllSkillFromResponse(response);
                Assert.AreEqual(skillsCount, skills.Count());
            }
        }

        [TestMethod]
        public void GetAllSkills_WhenUserIdIsInvalid_ShouldReturnZeroSkills()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(this.user);
                this.db.SaveChanges();

                this.user.Skills.Add(new Skill() { Name = "asd" });
                this.user.Skills.Add(new Skill() { Name = "asd" });
                this.user.Skills.Add(new Skill() { Name = "asd" });
                this.db.Users.Update(this.user);
                this.db.SaveChanges();

                var response = this.httpServer.CreateGetRequest(
                    string.Format(
                        "/api/skills/all?userId={0}&sessionKey={1}", 0, this.user.SessionKey));

                var skills = this.GetAllSkillFromResponse(response);
                Assert.AreEqual(0, skills.Count());
            }
        }

        [TestMethod]
        public void GetAllSkills_WhenSessionKeyIsInvalid_ShouldReturnInternalServerError()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(this.user);
                this.db.SaveChanges();
                
                this.user.Skills.Add(new Skill() { Name = "asd" });
                this.user.Skills.Add(new Skill() { Name = "asd" });
                this.user.Skills.Add(new Skill() { Name = "asd" });
                this.db.Users.Update(this.user);
                this.db.SaveChanges();

                var response = this.httpServer.CreateGetRequest(
                    string.Format(
                        "/api/skills/all?userId={0}&sessionKey={1}", this.user.Id, "invalidSessionKey"));
                Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
            }
        }

        [TestMethod]
        public void RemoveSkill_WhenDataIsValid_ShouldRemoveSkillFromUser()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(this.user);
                this.db.SaveChanges();

                var skill = new Skill()
                {
                    Name = "name"
                };

                this.user.Skills.Add(skill);
                this.db.Users.Update(this.user);
                this.db.SaveChanges();

                var skillModel = new SkillModel()
                {
                    Id = skill.Id
                };

                var response = this.httpServer.CreatePutRequest(
                    string.Format("/api/skills/remove?sessionKey={0}", this.user.SessionKey),
                    skillModel);
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

                var skillEntity = this.db.Skills
                    .All()
                    .Where(s => s.Users.Any(u => u.Id == this.user.Id))
                    .FirstOrDefault();
                Assert.IsNull(skillEntity);
            }
        }

        [TestMethod]
        public void RemoveSkill_WhenDataIsInvalid_ShouldReturnBadRequest()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(this.user);
                this.db.SaveChanges();

                var skill = new Skill()
                {
                    Name = "name"
                };

                this.user.Skills.Add(skill);
                this.db.Users.Update(this.user);
                this.db.SaveChanges();

                var skillModel = new SkillModel()
                {
                    Id = -1
                };

                var response = this.httpServer.CreatePutRequest(
                    string.Format("/api/skills/remove?sessionKey={0}", this.user.SessionKey),
                    skillModel);
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [TestMethod]
        public void RemoveSkill_WhenSessionKeyIsInvalid_ShouldReturnBadRequest()
        {
            using (new TransactionScope())
            {
                this.db.Users.Add(this.user);
                this.db.SaveChanges();

                var skill = new Skill()
                {
                    Name = "name"
                };

                this.user.Skills.Add(skill);
                this.db.Users.Update(this.user);
                this.db.SaveChanges();

                var skillModel = new SkillModel()
                {
                    Id = skill.Id
                };

                var response = this.httpServer.CreatePutRequest(
                    string.Format("/api/skills/remove?sessionKey={0}", "invalid session key"),
                    skillModel);
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        private SkillModel GetSkillModelFromResponse(HttpResponseMessage response)
        {
            string modelAsString = response.Content.ReadAsStringAsync().Result;
            var model = JsonConvert.DeserializeObject<SkillModel>(modelAsString);

            return model;
        }

        private IEnumerable<SkillModel> GetAllSkillFromResponse(HttpResponseMessage response)
        {
            string modelAsString = response.Content.ReadAsStringAsync().Result;
            var model = JsonConvert.DeserializeObject<IEnumerable<SkillModel>>(modelAsString);

            return model;
        }
    }
}
