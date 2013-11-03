using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace WiredUpWebApi.Models.ProjectModels
{
    [DataContract]
    public class ProjectEditModel
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "membersIds")]
        public IEnumerable<int> MembersIds { get; set; }
    }
}