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

        public IDbSet<Certificate> Certificates { get; set; }

        public IDbSet<Company> Companies { get; set; }

        public IDbSet<CompanyPost> CompanyPosts { get; set; }

        public IDbSet<Country> Countries { get; set; }

        public IDbSet<Message> Messages { get; set; }

        public IDbSet<Project> Projects { get; set; }

        public IDbSet<Skill> Skills { get; set; }

        public IDbSet<User> Users { get; set; }

        public IDbSet<UserPost> UserPosts { get; set; }

        public IDbSet<ConnectionRequest> ConnectionRequests { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Message>()
                       .HasRequired(m => m.Sender)
                       .WithMany(usr => usr.SentMessages)
                       .HasForeignKey(m => m.SenderId)
                       .WillCascadeOnDelete(false);

            modelBuilder.Entity<Message>()
                       .HasRequired(m => m.Receiver)
                       .WithMany(usr => usr.ReceivedMessages)
                       .HasForeignKey(m => m.ReceiverId)
                       .WillCascadeOnDelete(false);
        }
    }
}
