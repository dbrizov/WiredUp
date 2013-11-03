using System;
using System.Linq;
using System.Runtime.Serialization;

namespace WiredUpWebApi.Models.UserModels
{
    [DataContract]
    public class UserEditModel
    {
        [DataMember(Name = "languages")]
        public string Languages { get; set; }
    }
}