using System;
using System.Linq;
using System.Runtime.Serialization;

namespace WiredUpWebApi.Models.UserModels
{
    [DataContract]
    public class UserLoggedModel
    {
        [DataMember(Name = "fullName")]
        public string FullName { get; set; }

        [DataMember(Name = "sessionKey")]
        public string SessionKey { get; set; }
    }
}