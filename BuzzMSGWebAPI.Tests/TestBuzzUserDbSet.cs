using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuzzMSGData;
using BuzzMSGEntity.Models;


namespace BuzzMSGWebAPI.Tests
{
    public class TestBuzzUserDbSet : TestDbSet<BuzzUser>
    {
        public override BuzzUser Find(params object[] keyValues)
        {
            return this.SingleOrDefault(b => b.DataBaseID == (int) keyValues.Single());
        }


    }
   
}
