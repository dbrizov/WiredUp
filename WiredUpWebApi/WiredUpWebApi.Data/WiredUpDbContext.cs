using System;
using System.Data.Entity;
using System.Linq;
using WiredUpWebApi.Models;

namespace WiredUpWebApi.Data
{
    public class WiredUpDbContext : DbContext
    {
        public WiredUpDbContext()
            : base("WiredUpDb")
        {
        }

        public IDbSet<Address> UserAddresses { get; set; }

        public IDbSet<Certificate> UserCertificates { get; set; }

        public IDbSet<City> Cities { get; set; }

        public IDbSet<Company> Companies { get; set; }

        public IDbSet<CompanyPost> CompanyPosts { get; set; }

        public IDbSet<Country> Countries { get; set; }

        public IDbSet<Message> Messages { get; set; }

        public IDbSet<Project> UserProjects { get; set; }

        public IDbSet<Skill> UserSkills { get; set; }

        public IDbSet<User> Users { get; set; }

        public IDbSet<UserPost> UserPosts { get; set; }
    }
}
