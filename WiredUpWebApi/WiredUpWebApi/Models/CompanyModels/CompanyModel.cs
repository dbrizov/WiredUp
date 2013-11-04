using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace WiredUpWebApi.Models.CompanyModels
{
    [DataContract]
    public class CompanyModel
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        public static Expression<Func<Company, CompanyModel>> FromCompany
        {
            get
            {
                return company => new CompanyModel()
                {
                    Id = company.Id,
                    Name = company.Name
                };
            }
        }
    }
}