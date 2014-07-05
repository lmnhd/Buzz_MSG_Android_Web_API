using System.Data.Entity;
using BuzzMSGData.Mapping;
using BuzzMSGEntity.Mapping;
using BuzzMSGEntity.Models;
using Repository.Pattern.Ef6;

namespace BuzzMSGData
{
    public partial class BuzzMsgdbContext : DataContext
    {
        static BuzzMsgdbContext()
        {
            Database.SetInitializer<BuzzMsgdbContext>(null);
        }

        public BuzzMsgdbContext()
            : base("Name=BuzzMSGDBContext")
        {
        }

        public DbSet<Buzz> Buzzs { get; set; }
        public DbSet<BuzzUser> BuzzUsers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new BuzzMap());
            modelBuilder.Configurations.Add(new BuzzUserMap());
        }
    }
}
