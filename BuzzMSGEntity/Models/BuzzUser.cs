using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.Ef6;

namespace BuzzMSGEntity.Models
{
    public partial class BuzzUser : Entity
    {
        public BuzzUser()
        {
            //this.BuzzUsers1 = new List<BuzzUser>();
            //this.BuzzUsers11 = new List<BuzzUser>();
            //this.BuzzUsers12 = new List<BuzzUser>();
        }
        [Key]
        public int DataBaseID { get; set; }
        public int UserID { get; set; }
        public bool isOnline { get; set; }
        public System.DateTime dateAdded { get; set; }
        public System.DateTime lastResponse { get; set; }
        public string email { get; set; }
        public string verified_email { get; set; }
        public string name { get; set; }
        public string given_name { get; set; }
        public string family_name { get; set; }
        public string link { get; set; }
        public string picture { get; set; }
        public string base64PhotoString { get; set; }
        public string gender { get; set; }
        public string Locale { get; set; }
        public string FaceBook { get; set; }
        public string Country { get; set; }
        public string UserName { get; set; }
        public string refresh_token { get; set; }
        public string current_at { get; set; }
        public string registration_id { get; set; }

        public virtual  ICollection<BuzzUser> Contacts { get; set; }
        public virtual ICollection<BuzzUser> ReceivedContactRequests { get; set; }
        public virtual ICollection<BuzzUser> SentContactRequests { get; set; } 
        //public Nullable<int> BuzzUser_DataBaseID { get; set; }
        //public Nullable<int> BuzzUser_DataBaseID1 { get; set; }
        //public Nullable<int> BuzzUser_DataBaseID2 { get; set; }
        //public virtual ICollection<BuzzUser> BuzzUsers1 { get; set; }
        //public virtual BuzzUser BuzzUser1 { get; set; }
        //public virtual ICollection<BuzzUser> BuzzUsers11 { get; set; }
        //public virtual BuzzUser BuzzUser2 { get; set; }
        //public virtual ICollection<BuzzUser> BuzzUsers12 { get; set; }
        //public virtual BuzzUser BuzzUser3 { get; set; }

       
    }
}
