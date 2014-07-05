using System.Data.Entity.ModelConfiguration;
using BuzzMSGEntity.Models;

namespace BuzzMSGEntity.Mapping
{
    public class BuzzMap : EntityTypeConfiguration<Buzz>
    {
        public BuzzMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("Buzzs");
            this.Property(t => t.Id).HasColumnName("ID");
            this.Property(t => t.FromUserID).HasColumnName("FromUserID");
            this.Property(t => t.ToUserID).HasColumnName("ToUserID");
            this.Property(t => t.Time).HasColumnName("Time");
            this.Property(t => t.ServerTime).HasColumnName("ServerTime");
            this.Property(t => t.ReceivedTime).HasColumnName("ReceivedTime");
            this.Property(t => t.Type).HasColumnName("Type");
            this.Property(t => t.IsReply).HasColumnName("IsReply");
            this.Property(t => t.Message).HasColumnName("Message");
            this.Property(t => t.BuzzIdentity).HasColumnName("BuzzIdentity");
            this.Property(t => t.FilePath).HasColumnName("FilePath");
        }
    }
}
