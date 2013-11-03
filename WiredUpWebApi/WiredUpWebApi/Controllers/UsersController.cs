﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Http;
using WiredUpWebApi.Data;
using WiredUpWebApi.Models;
using WiredUpWebApi.Models.Constants;
using WiredUpWebApi.Models.UserModels;

namespace WiredUpWebApi.Controllers
{
    public class UsersController : BaseApiController
    {
        private const string ValidNameChars = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM";
        private const string ValidEmailRegex = @"\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,4}";
        private const int FirstNameMaxLength = UserConstants.FirstNameMaxLength;
        private const int LastNameMaxLength = UserConstants.LastNameMaxLength;
        private const int EmailMaxLength = UserConstants.EmailMaxLength;
        private const int Sha1PasswordLength = 40;
        private const string SessionKeyChars = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM";
        private const int SessionKeyLength = UserConstants.SessionKeyMaxLength;

        public UsersController()
            : base()
        {
        }

        public UsersController(IUnitOfWorkData db)
            : base(db)
        {
        }

        [HttpPost]
        [ActionName("register")]
        public HttpResponseMessage RegisterUser([FromBody]UserRegisterModel model)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(() =>
            {
                this.ValidateFirstName(model.FirstName);
                this.ValidateLastName(model.LastName);
                this.ValidateEmail(model.Email);
                this.ValidateAuthCodesMatch(model.AuthCode, model.ConfirmAuthCode);
                this.ValidateAuthCode(model.AuthCode);

                string emailToLower = model.Email.ToLower();

                User existingUser =
                    this.db.Users.All().Where(u => u.Email == emailToLower).FirstOrDefault();

                if (existingUser != null)
                {
                    throw new InvalidOperationException("The user already exists");
                }

                User newUser = new User()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = emailToLower,
                    AuthCode = model.AuthCode,
                };

                this.db.Users.Add(newUser);
                this.db.SaveChanges();

                newUser.SessionKey = this.GenerateSessionKey(newUser.Id);
                this.db.Users.Update(newUser);
                this.db.SaveChanges();

                var userLoggedModel = new UserLoggedModel()
                {
                    Id = newUser.Id,
                    DisplayName = newUser.FirstName + " " + newUser.LastName,
                    SessionKey = newUser.SessionKey
                };

                var response = this.Request.CreateResponse(HttpStatusCode.Created, userLoggedModel);
                return response;
            });

            return responseMsg;
        }

        [HttpPost]
        [ActionName("login")]
        public HttpResponseMessage LoginUser([FromBody]UserLoginModel model)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(() =>
            {
                User user = this.db.Users.All().Where(
                    u => u.Email == model.Email.ToLower() &&
                           u.AuthCode == model.AuthCode).FirstOrDefault();

                if (user == null)
                {
                    // The user does not exist
                    throw new InvalidOperationException("Invalid email or password");
                }

                if (user.SessionKey == null)
                {
                    user.SessionKey = this.GenerateSessionKey(user.Id);
                    this.db.Users.Update(user);
                    this.db.SaveChanges();
                }

                var userLoggedModel = new UserLoggedModel()
                {
                    Id = user.Id,
                    DisplayName = user.FirstName + " " + user.LastName,
                    SessionKey = user.SessionKey
                };

                var response = this.Request.CreateResponse(HttpStatusCode.Created, userLoggedModel);
                return response;
            });

            return responseMsg;
        }

        [HttpPut]
        [ActionName("logout")]
        public HttpResponseMessage LogoutUser([FromUri]string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(() =>
            {
                var user = this.GetUserBySessionKey(sessionKey);

                user.SessionKey = null;
                this.db.Users.Update(user);
                this.db.SaveChanges();

                var response = this.Request.CreateResponse(HttpStatusCode.OK);
                return response;
            });

            return responseMsg;
        }

        [HttpPut]
        [ActionName("changePassword")]
        public HttpResponseMessage ChangePasswordOfUser(
            [FromBody]UserChangePasswordModel model, [FromUri]string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(() =>
            {
                var user = this.GetUserBySessionKey(sessionKey);

                string userOldAuthCode = user.AuthCode;
                this.ValidateAuthCodesMatch(userOldAuthCode, model.OldAuthCode);
                this.ValidateAuthCodesMatch(model.NewAuthCode, model.ConfirmNewAuthCode);
                this.ValidateAuthCode(model.NewAuthCode);

                user.AuthCode = model.NewAuthCode;
                this.db.Users.Update(user);
                this.db.SaveChanges();

                var response = this.Request.CreateResponse(HttpStatusCode.Created);
                return response;
            });

            return responseMsg;
        }

        private string GenerateSessionKey(int userId)
        {
            Random rand = new Random();
            StringBuilder sessionKeyBuilder = new StringBuilder(SessionKeyLength);
            sessionKeyBuilder.Append(userId);

            while (sessionKeyBuilder.Length < SessionKeyLength)
            {
                var index = rand.Next(SessionKeyChars.Length);
                sessionKeyBuilder.Append(SessionKeyChars[index]);
            }

            return sessionKeyBuilder.ToString();
        }

        private void ValidateAuthCode(string authCode)
        {
            if (string.IsNullOrWhiteSpace(authCode) || authCode.Length != Sha1PasswordLength)
            {
                throw new ArgumentException("Password must be SHA1 encrypted");
            }
        }

        private void ValidateAuthCodesMatch(string authCode, string confirmAuthCode)
        {
            if (authCode != confirmAuthCode)
            {
                throw new ArgumentException("The authentication codes don't match");
            }
        }

        private void ValidateFirstName(string firstName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentException("The 'First Name' is required");
            }
            else if (firstName.Length > FirstNameMaxLength)
            {
                throw new ArgumentException(
                    string.Format("The 'First Name' must be less than {0} characters long", FirstNameMaxLength));
            }
            else if (firstName.Any(ch => !ValidNameChars.Contains(ch)))
            {
                throw new ArgumentException("The 'First Name' must contain only chars from a-z and A-Z");
            }
        }

        private void ValidateLastName(string lastName)
        {
            if (string.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentException("The 'Last Name' is required");
            }
            else if (lastName.Length > FirstNameMaxLength)
            {
                throw new ArgumentException(
                    string.Format("The 'Last Name' must be less than {0} characters long", LastNameMaxLength));
            }
            else if (lastName.Any(ch => !ValidNameChars.Contains(ch)))
            {
                throw new ArgumentException("The 'Last Name' must contain only chars from a-z and A-Z");
            }
        }

        private void ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("The 'Email' is required");
            }
            else if (email.Length > EmailMaxLength)
            {
                throw new ArgumentException(
                    string.Format("The 'Email' must be less than {0} characters long", EmailMaxLength));
            }
            else if (!Regex.IsMatch(email, ValidEmailRegex))
            {
                throw new ArgumentException("The 'Email' is invalid");
            }
        }
    }
}