using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace WiredUpWebApi.Models.ProjectModels
{
    [DataContract]
    public class ProjectModel
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        public static Expression<Func<Project, ProjectModel>> FromProject
        {
            get
            {
                return project => new ProjectModel()
                {
                    Id = project.Id,
                    Name = project.Name
                };
            }
        }
    }
}