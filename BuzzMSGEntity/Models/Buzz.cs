using System;
using System.Collections.Generic;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.Ef6;
namespace BuzzMSGEntity.Models
{
    public partial class Buzz : Entity
    {
        public int Id { get; set; }
        public int FromUserID { get; set; }
        public int ToUserID { get; set; }
        public System.DateTime Time { get; set; }
        public System.DateTime ServerTime { get; set; }
        public System.DateTime ReceivedTime { get; set; }
        public string Type { get; set; }
        public bool IsReply { get; set; }
        public string Message { get; set; }
        public string BuzzIdentity { get; set; }
        public string FilePath { get; set; }

      
       // public ObjectState ObjectState { get; set; }
       
    }
}
