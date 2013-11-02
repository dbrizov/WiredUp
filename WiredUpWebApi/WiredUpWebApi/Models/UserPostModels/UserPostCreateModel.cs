using System;
using System.Linq;
using System.Runtime.Serialization;

namespace WiredUpWebApi.Models.UserPostModels
{
    [DataContract]
    public class UserPostCreateModel
    {
        [DataMember(Name = "content")]
        public string Content { get; set; }
    }
}