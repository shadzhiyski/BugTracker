namespace BugTracker.Data
{
    using System.Data.Entity;

    using BugTracker.Data.Models;

    using Microsoft.AspNet.Identity.EntityFramework;
    using BugTracker.Data.Migrations;

    public class BugTrackerDbContext : IdentityDbContext<User>
    {
        public BugTrackerDbContext()
            : base("BugTracker")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<BugTrackerDbContext, BugTrackerDbMigrationConfiguration>());
        }
        
        public static BugTrackerDbContext Create()
        {
            return new BugTrackerDbContext();
        }

        public DbSet<Bug> Bugs { get; set; }

        public DbSet<Comment> Comments { get; set; }
    }
}
