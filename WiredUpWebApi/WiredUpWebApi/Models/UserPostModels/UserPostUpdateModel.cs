using System;
using System.Linq;
using System.Runtime.Serialization;

namespace WiredUpWebApi.Models.UserPostModels
{
    [DataContract]
    public class UserPostUpdateModel
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "content")]
        public string Content { get; set; }
    }
}