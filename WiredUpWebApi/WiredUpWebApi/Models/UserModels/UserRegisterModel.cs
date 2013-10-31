using System;
using System.Linq;
using System.Runtime.Serialization;

namespace WiredUpWebApi.Models.UserModels
{
    [DataContract]
    public class UserRegisterModel
    {
        [DataMember(Name = "firstName")]
        public string FirstName { get; set; }

        [DataMember(Name = "lastName")]
        public string LastName { get; set; }

        [DataMember(Name = "email")]
        public string Email { get; set; }

        [DataMember(Name = "authCode")]
        public string AuthCode { get; set; }
    }
}