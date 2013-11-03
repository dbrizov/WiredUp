using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace WiredUpWebApi.Models.ProjectModels
{
    [DataContract]
    public class ProjectDetailedModel
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "members")]
        public IEnumerable<string> Members { get; set; }

        public static Expression<Func<Project, ProjectDetailedModel>> FromProject
        {
            get
            {
                return project => new ProjectDetailedModel()
                {
                    Description = project.Description,
                    Name = project.Name,
                    Url = project.Url,
                    Members = project.TeamMembers.Select(p => p.FirstName + " " + p.LastName)
                };
            }
        }
    }
}