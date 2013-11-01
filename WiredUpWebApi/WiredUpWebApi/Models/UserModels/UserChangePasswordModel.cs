using System;
using System.Linq;
using System.Runtime.Serialization;

namespace WiredUpWebApi.Models.UserModels
{
    [DataContract]
    public class UserChangePasswordModel
    {
        [DataMember(Name = "oldAuthCode")]
        public string OldAuthCode { get; set; }

        [DataMember(Name = "newAuthCode")]
        public string NewAuthCode { get; set; }

        [DataMember(Name = "confirmNewAuthCode")]
        public string ConfirmNewAuthCode { get; set; }
    }
}