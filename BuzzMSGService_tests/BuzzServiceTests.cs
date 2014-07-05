//using BuzzMSGEntity.Models;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Security.Policy;
using BuzzMSGData;
using BuzzMSGEntity.Models;
using BuzzMSGServices;
using Moq;
using Repository.Pattern.Ef6;
using Repository.Pattern.Repositories;
using Xunit;


namespace BuzzMSGServicesTests
{
    public class BuzzServiceTests
    {


        public BuzzService GetBuzzService()
        {
            var mockUserRepo = new Mock<IRepository<BuzzUser>>();
            var mockBuzzRepo = new Mock<IRepository<Buzz>>();
            var service = new BuzzService(mockUserRepo.Object, mockBuzzRepo.Object);

            return service;
        }

        public BuzzUser GetTestUser(string fakeAccessToken,string email = "",string userName = "",string country = "",string locale = "")
        {
            var rand = new Random();
            if (userName.Equals(""))
            {
                userName = "cc lemonhead " + DateTime.Now.Ticks.ToString();
            }
            if (email.Equals(""))
            {
                email = string.Format("{0},{1}@gmail.com", userName, rand.Next(0, 3000));
            }
            if (country.Equals(""))
            {
                country = "United States";
            }
            if (locale.Equals(""))
            {
                locale = "Jacksonville";
            }
            var user = new BuzzUser()
            {
                Contacts = new Collection<BuzzUser>(),
                SentContactRequests = new Collection<BuzzUser>(),
                ReceivedContactRequests = new Collection<BuzzUser>(),
                Country = country,
                FaceBook = "",
                Locale = locale,
                UserName = userName,
                base64PhotoString = "",
                current_at = fakeAccessToken,
                dateAdded = DateTime.Now,
                family_name = userName,
                email = email,
                gender = "female",
                isOnline = true,
                link = "",
                name = userName,
                refresh_token = fakeAccessToken,
                registration_id = fakeAccessToken,
                picture = "",
                verified_email = "true"

            };
            return user;
        }

        public Buzz GetNewBuzz()
        {
            var bz = new Buzz()
            {
                BuzzIdentity = "410934857748", 
                FilePath = "", Message = "no message", 
                Type = "voice" , 
                ToUserID = 2 , 
                FromUserID = 1,
                IsReply = false, 
                Time = DateTime.Now,
                ServerTime = DateTime.Now
            };

            return bz;
        }

        [Fact()]
        public void AddBuzzTest_ShouldNotThrowException()
        {
            //Arrange
            var service = GetBuzzService();
            var bz = GetNewBuzz();

            Assert.ThrowsDelegate act = () => service.AddBuzz(bz);
            Assert.DoesNotThrow(act);
            
           
        }

        [Fact()]
        public void GetBuzzUserByAtTest_ShouldReturnTheUser()
        {
            var mockUserRepo = new Mock<IRepository<BuzzUser>>();
            var mockBuzzRepo = new Mock<IRepository<Buzz>>(); 
            var at = DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture);
            var user = GetTestUser(at);
            mockUserRepo.SetReturnsDefault(user);

            var returnResult = new Mock<IQueryFluent<BuzzUser>>();
            returnResult.Setup(q => q.Select())
                .Returns(
                    new Func<IEnumerable<BuzzUser>>(() => new List<BuzzUser> {user} as IEnumerable<BuzzUser>));
           
            mockUserRepo.Setup(mock => mock.Query(u => u.current_at.Equals(at))).Returns(returnResult.Object);
            var service = new BuzzService(mockUserRepo.Object, mockBuzzRepo.Object);

            Assert.ThrowsDelegate act = () => service.GetBuzzUserByAt(at);

            Assert.DoesNotThrow(act);
            //Assert.NotNull(service.GetBuzzUserByAt(at));


            //mockUserRepo.Verify(r => r.Query(It.IsAny<Func<BuzzUser>>));



        }

        [Fact()]
        public void GetBuzzTest_ShouldCallQuery()
        {
            
        }
    }
}
