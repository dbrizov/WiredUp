using System;
using System.Linq;
using WiredUpWebApi.Models;

namespace WiredUpWebApi.Data
{
    public interface IUnitOfWorkData : IDisposable
    {
        IRepository<Address> UserAddresses { get; }

        IRepository<Certificate> UserCertificates { get; }

        IRepository<City> Cities { get; }

        IRepository<Company> Companies { get; }

        IRepository<CompanyPost> CompanyPosts { get; }

        IRepository<Country> Countries { get; }

        IRepository<Message> Messages { get; }

        IRepository<Project> UserProjects { get; }

        IRepository<Skill> UserSkills { get; }

        IRepository<User> Users { get; }

        IRepository<UserPost> UserPosts { get; }

        int SaveChanges();
    }
}
