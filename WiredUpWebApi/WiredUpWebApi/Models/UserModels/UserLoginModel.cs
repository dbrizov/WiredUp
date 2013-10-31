using System;
using System.Linq;
using System.Runtime.Serialization;

namespace WiredUpWebApi.Models.UserModels
{
    [DataContract]
    public class UserLoginModel
    {
        [DataMember(Name = "email")]
        public string Email { get; set; }

        [DataMember(Name = "authCode")]
        public string AuthCode { get; set; }
    }
}