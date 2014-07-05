using System.Data.Entity.ModelConfiguration;
using BuzzMSGEntity.Models;

namespace BuzzMSGData.Mapping
{
    public class BuzzUserMap : EntityTypeConfiguration<BuzzUser>
    {
        public BuzzUserMap()
        {
            // Primary Key
            this.HasKey(t => t.DataBaseID);

            // Properties
            // Table & Column Mappings
            this.ToTable("BuzzUsers");
            this.Property(t => t.DataBaseID).HasColumnName("DataBaseID");
            this.Property(t => t.UserID).HasColumnName("UserID");
            this.Property(t => t.isOnline).HasColumnName("isOnline");
            this.Property(t => t.dateAdded).HasColumnName("dateAdded");
            this.Property(t => t.lastResponse).HasColumnName("lastResponse");
            this.Property(t => t.email).HasColumnName("email");
            this.Property(t => t.verified_email).HasColumnName("verified_email");
            this.Property(t => t.name).HasColumnName("name");
            this.Property(t => t.given_name).HasColumnName("given_name");
            this.Property(t => t.family_name).HasColumnName("family_name");
            this.Property(t => t.link).HasColumnName("link");
            this.Property(t => t.picture).HasColumnName("picture");
            this.Property(t => t.base64PhotoString).HasColumnName("base64PhotoString");
            this.Property(t => t.gender).HasColumnName("gender");
            this.Property(t => t.Locale).HasColumnName("Locale");
            this.Property(t => t.FaceBook).HasColumnName("FaceBook");
            this.Property(t => t.Country).HasColumnName("Country");
            this.Property(t => t.UserName).HasColumnName("UserName");
            this.Property(t => t.refresh_token).HasColumnName("refresh_token");
            this.Property(t => t.current_at).HasColumnName("current_at");
            this.Property(t => t.registration_id).HasColumnName("registration_id");
            //this.Property(t => t.Contacts).HasColumnName("Contacts");
            //this.Property(t => t.ReceivedContactRequests).HasColumnName("ReceivedContactRequests");
            //this.Property(t => t.SentContactRequests).HasColumnName("SentContactRequests");

            // Relationships
            //this.HasOptional(t => t.Contacts)
            //    .WithMany(t => t.)
            //    .HasForeignKey(d => d.BuzzUser_DataBaseID);
            //this.HasOptional(t => t.BuzzUser2)
            //    .WithMany(t => t.ReceivedContactRequestsCollection)
            //    .HasForeignKey(d => d.BuzzUser_DataBaseID1);
            //this.HasOptional(t => t.BuzzUser3)
            //    .WithMany(t => t.BuzzUsers12)
            //    .HasForeignKey(d => d.BuzzUser_DataBaseID2);

        }
    }
}
