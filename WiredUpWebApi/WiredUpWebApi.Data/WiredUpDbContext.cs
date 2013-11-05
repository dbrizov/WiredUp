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

        public IDbSet<Connection> Connections { get; set; }

        public IDbSet<UserPost> UserPosts { get; set; }

        public IDbSet<ConnectionRequest> ConnectionRequests { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Message>()
                .HasRequired(m => m.Sender)
                .WithMany(user => user.SentMessages)
                .HasForeignKey(m => m.SenderId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Message>()
                .HasRequired(m => m.Receiver)
                .WithMany(user => user.ReceivedMessages)
                .HasForeignKey(m => m.ReceiverId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ConnectionRequest>()
                .HasRequired(c => c.Receiver)
                .WithMany(user => user.ConnectionRequests)
                .HasForeignKey(c => c.ReceiverId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Connection>()
                .HasRequired(c => c.User)
                .WithMany(user => user.Connections)
                .HasForeignKey(c => c.UserId)
                .WillCascadeOnDelete(false);
        }
    }
}
