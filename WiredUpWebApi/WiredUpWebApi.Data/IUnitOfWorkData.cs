using System;
using System.Linq;
using WiredUpWebApi.Models;

namespace WiredUpWebApi.Data
{
    public interface IUnitOfWorkData : IDisposable
    {
        IRepository<Certificate> Certificates { get; }

        IRepository<Company> Companies { get; }

        IRepository<CompanyPost> CompanyPosts { get; }

        IRepository<Country> Countries { get; }

        IRepository<Message> Messages { get; }

        IRepository<Project> Projects { get; }

        IRepository<Skill> Skills { get; }

        IRepository<User> Users { get; }

        IRepository<UserPost> UserPosts { get; }

        int SaveChanges();
    }
}
