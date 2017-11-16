using System.Data.Entity;
using TeamGlobal.Infrastructure.Model;

namespace TeamGlobal.Infrastructure
{
    public class TeamGlobalContext : DbContext
    {
        public TeamGlobalContext() : base("TeamGlobalContext")
        {
        }

        public DbSet<Location> Location { get; set; }
        public DbSet<PortInfo> PortInfo { get; set; }
        public DbSet<PortList> PortList { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.PluralizingTableNameConvention>();
        }
    }
}