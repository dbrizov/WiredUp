namespace WiredUpWebApi.Data.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public sealed class WiredUpDbConfiguration : DbMigrationsConfiguration<WiredUpWebApi.Data.WiredUpDbContext>
    {
        public WiredUpDbConfiguration()
        {
            this.AutomaticMigrationsEnabled = true;
            this.AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(WiredUpWebApi.Data.WiredUpDbContext context)
        {
        }
    }
}
