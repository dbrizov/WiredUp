using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace WiredUpWebApi.Models.SkillModels
{
    [DataContract]
    public class SkillModel
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        public static Expression<Func<Skill, SkillModel>> FromSkill
        {
            get
            {
                return skill => new SkillModel()
                {
                    Id = skill.Id,
                    Name = skill.Name
                };
            }
        }
    }
}