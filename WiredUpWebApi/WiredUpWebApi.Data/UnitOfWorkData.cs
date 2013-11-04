using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using WiredUpWebApi.Models;

namespace WiredUpWebApi.Data
{
    public class UnitOfWorkData : IUnitOfWorkData
    {
        private readonly DbContext dbContext;
        private readonly Dictionary<Type, object> repositories = new Dictionary<Type, object>();
        
        public UnitOfWorkData()
            : this(new WiredUpDbContext())
        {
        }

        public UnitOfWorkData(DbContext context)
        {
            this.dbContext = context;
        }

        private IRepository<T> GetRepository<T>() where T : class
        {
            if (!this.repositories.ContainsKey(typeof(T)))
            {
                var type = typeof(EfRepository<T>);

                this.repositories.Add(typeof(T), Activator.CreateInstance(type, this.dbContext));
            }

            return (IRepository<T>)this.repositories[typeof(T)];
        }

        public int SaveChanges()
        {
            return this.dbContext.SaveChanges();
        }

        public void Dispose()
        {
            this.dbContext.Dispose();
        }

        public IRepository<Certificate> Certificates
        {
            get
            {
                return this.GetRepository<Certificate>();
            }
        }

        public IRepository<Company> Companies
        {
            get
            {
                return this.GetRepository<Company>();
            }
        }

        public IRepository<CompanyPost> CompanyPosts
        {
            get
            {
                return this.GetRepository<CompanyPost>();
            }
        }

        public IRepository<Country> Countries
        {
            get
            {
                return this.GetRepository<Country>();
            }
        }

        public IRepository<Message> Messages
        {
            get
            {
                return this.GetRepository<Message>();
            }
        }

        public IRepository<Project> Projects
        {
            get
            {
                return this.GetRepository<Project>();
            }
        }

        public IRepository<Skill> Skills
        {
            get
            {
                return this.GetRepository<Skill>();
            }
        }

        public IRepository<User> Users
        {
            get
            {
                return this.GetRepository<User>();
            }
        }

        public IRepository<UserPost> UserPosts
        {
            get
            {
                return this.GetRepository<UserPost>();
            }
        }

        public IRepository<ConnectionRequest> ConnectionRequests
        {
            get
            {
                return this.GetRepository<ConnectionRequest>();
            }
        }
    }
}
