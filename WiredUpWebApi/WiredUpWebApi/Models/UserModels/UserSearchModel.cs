using System;
using System.Linq;
using System.Runtime.Serialization;

namespace WiredUpWebApi.Models.UserModels
{
    [DataContract]
    public class UserSearchModel
    {
        [DataMember(Name = "query")]
        public string Query { get; set; }
    }
}