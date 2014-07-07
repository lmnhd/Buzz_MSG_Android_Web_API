//using BuzzMSGEntity.Models;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting;
using System.Security.Policy;
using BuzzMSGData;
using BuzzMSGEntity.Models;
using BuzzMSGServices;
using Moq;
using Repository.Pattern.Ef6;
using Repository.Pattern.Repositories;
using Xunit;
namespace BuzzMSGServices.Tests
{
    public class BuzzServiceTests
    {
        public BuzzService GetBuzzService(Mock<IRepository<BuzzUser>> userRepo = null ,Mock<IRepository<Buzz> > buzzRepo = null  )
        {
            var mockUserRepo = userRepo ?? new Mock<IRepository<BuzzUser>>();
            var mockBuzzRepo = buzzRepo ?? new Mock<IRepository<Buzz>>();
            var service = new BuzzService(mockUserRepo.Object, mockBuzzRepo.Object);

            return service;
        }

        public BuzzUser GetTestUser(string fakeAccessToken, string email = "", string userName = "", string country = "", string locale = "")
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


        private List<BuzzUser> GetTestUserList(int count,string userName = "")
        {
            var result = new List<BuzzUser>();
            for (var i = 0; i < count; i++)
            {
                if (userName.Equals(""))
                {
                    userName = "cclemonead" + i.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    userName += i.ToString(CultureInfo.InvariantCulture);
                }
                var at = DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture);
                var tu = GetTestUser(at, userName: userName);
                tu.Contacts = new List<BuzzUser>();
                if (result.Any())
                {
                    for (var j = 0; j < result.Count(); j++)
                    {
                        tu.Contacts.Add(result[j]);  
                        tu.ReceivedContactRequests.Add(result[j]);
                        tu.SentContactRequests.Add(result[j]);
                    }
                }
                result.Add(tu);
            }
            result.Reverse();
            return result;
        } 
        public Buzz GetNewBuzz(string identity = "")
        {
            if (identity.Equals(""))
            {
                identity = "410934857748";
            }
            var bz = new Buzz()
            {
                BuzzIdentity = identity,
                FilePath = "",
                Message = "no message",
                Type = "voice",
                ToUserID = 2,
                FromUserID = 1,
                IsReply = false,
                Time = DateTime.Now,
                ServerTime = DateTime.Now
            };

            return bz;
        }


        public IQueryFluent<BuzzUser> GetUserQueryFluent(List<BuzzUser> users )
        {
            var returnResult = new Mock<IQueryFluent<BuzzUser>>();
            returnResult.Setup(q => q.Select())
                .Returns(
                    new Func<IEnumerable<BuzzUser>>(() => users as IEnumerable<BuzzUser>));

            return returnResult.Object;

        }

        public IQueryFluent<Buzz> GetBuzzQueryFluent(List<Buzz> buzzes )
        {
            var returnResult = new Mock<IQueryFluent<Buzz>>();
            returnResult.Setup(q => q.Select()).Returns(new Func<IEnumerable<Buzz>>(() => buzzes as IEnumerable<Buzz>));

            return returnResult.Object;
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
             


          

            //mockUserRepo.Setup(mock => mock.Query(u => u.current_at.Equals(at))).Returns(returnResult.Object);
            mockUserRepo.SetReturnsDefault(GetUserQueryFluent(new List<BuzzUser>(){user}));
            //mockUserRepo.SetupAllProperties();
            var service = new BuzzService(mockUserRepo.Object, mockBuzzRepo.Object);

            Assert.ThrowsDelegate act = () => service.GetBuzzUserByAt(at);

            Assert.DoesNotThrow(act);

            Assert.NotNull(service.GetBuzzUserByAt(at));

            Assert.True(service.GetBuzzUserByAt(at).current_at.Equals(at));

           mockUserRepo = new Mock<IRepository<BuzzUser>>();
            service = new BuzzService(mockUserRepo.Object,mockBuzzRepo.Object);
            act = () => service.GetBuzzUserByAt(at);

            Assert.DoesNotThrow(act);

            Assert.Null(service.GetBuzzUserByAt(at));
            



        }

        [Fact()]
        public void GetBuzzTest_ShouldNotThrow_ShouldCallUserRepoQuery()
        {
            var mockUserRepo = new Mock<IRepository<BuzzUser>>();
            var mockBuzzRepo = new Mock<IRepository<Buzz>>();
            const int fromUserID = 1;
            const string buzzID = "rituioirtwirituoweirutowirut";
            mockBuzzRepo.SetReturnsDefault(GetBuzzQueryFluent(new List<Buzz>(){GetNewBuzz()}));


            //mockBuzzRepo.Setup(repo => repo.Query())
            //    .Returns(GetBuzzQueryFluent(new List<Buzz>() {GetNewBuzz()}));
            mockBuzzRepo.Setup(repo => repo.Query(b => b.FromUserID == fromUserID && b.BuzzIdentity == buzzID));
            

            var service = new BuzzService(mockUserRepo.Object, mockBuzzRepo.Object);
            service.GetBuzz(fromUserID, buzzID, true);


            Assert.ThrowsDelegate act = () => service.GetBuzz(fromUserID, buzzID, true);

            Assert.DoesNotThrow(act);

           

            Assert.NotNull(service.GetBuzz(fromUserID, buzzID, true));

            //mockBuzzRepo.Verify(r => r.Query(b => b.FromUserID == fromUserID && b.BuzzIdentity == buzzID));
            


        }

        [Fact()]
        public void UpdateLastResponseTimeTest()
        {
            //Arrange
            const string fakeAt = "rituhfteiiutjirhg88";
            var user = GetTestUser(fakeAt);
            var service = GetBuzzService();
            var fakeBuzz = GetNewBuzz();
            var time1 = new DateTime(1970,2,5);
            user.lastResponse = time1;

            //Act
            Assert.ThrowsDelegate act = () => service.UpdateLastResponseTime(user);


            //Assert
            Assert.DoesNotThrow(act);

            Assert.True(user.lastResponse != time1);




        }

        [Fact()]
        public void MarkBuzzAsSentTest()
        {
            var mockUserRepo = new Mock<IRepository<BuzzUser>>();
            var mockBuzzRepo = new Mock<IRepository<Buzz>>();
            var defaultDate = new DateTime(1970, 1, 1);
            var testBuzz = GetNewBuzz();
            testBuzz.ReceivedTime = defaultDate;

            mockBuzzRepo.Setup(mock => mock.Update(testBuzz));

            var service = new BuzzService(mockUserRepo.Object, mockBuzzRepo.Object);

            Assert.ThrowsDelegate act = () => service.MarkBuzzAsSent(testBuzz);


            //Assert
            Assert.DoesNotThrow(act);
            Assert.True(testBuzz.ReceivedTime != defaultDate);
            mockBuzzRepo.Verify(u => u.Update(testBuzz));


        }

        private List<Buzz> setUpGetBuzzesTest()
        {
            var result = new List<Buzz>();

            for(var i = 0; i < 100; i++)
            {
                result.Add(GetNewBuzz(DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture)));
            }

            return result;
        }
        [Fact()]
        public void GetBuzzesTest()
        {
            //Arrange
            var mockUserRepo = new Mock<IRepository<BuzzUser>>();
            var mockBuzzRepo = new Mock<IRepository<Buzz>>();
            mockBuzzRepo.SetReturnsDefault(GetBuzzQueryFluent(setUpGetBuzzesTest()));
            var service = new BuzzService(mockUserRepo.Object, mockBuzzRepo.Object);
            var fromID = 1;
            var toID = 2;
            

            //Act

            //1. test get with all ids 0 : Should return empty list
            Assert.ThrowsDelegate test1 = () => service.GetBuzzes(0, 0, 20);

            Assert.DoesNotThrow(test1);

            Assert.True( !service.GetBuzzes(0,0,20).Any());

            //2. test get with only fromID with and without numBuzzes 0 : should return numBuzzes

            Assert.ThrowsDelegate test2 = () => service.GetBuzzes(fromID, 0, 20);
            Assert.DoesNotThrow(test2);
            Assert.True(service.GetBuzzes(fromID, 0, 20).Count() == 20);
            Assert.True(service.GetBuzzes(fromID, 0, 67).Count() == 67);
            Assert.False(service.GetBuzzes(fromID, 0, 25).Count() == 30);

            test2 = () => service.GetBuzzes(fromID, 0, 150);
            Assert.DoesNotThrow(test2);
            Assert.True(service.GetBuzzes(fromID, 0, 150).Count() == 100);

            //3. test get with to and from ids

            Assert.ThrowsDelegate test3 = () => service.GetBuzzes(fromID, toID, 20);
            Assert.DoesNotThrow(test3);
            Assert.True(service.GetBuzzes(fromID, toID, 20).Count() == 20);
            Assert.True(service.GetBuzzes(fromID, toID, 67).Count() == 67);
            Assert.False(service.GetBuzzes(fromID, toID, 25).Count() == 30);

            test3 = () => service.GetBuzzes(fromID, toID, 150);
            Assert.DoesNotThrow(test3);
            Assert.True(service.GetBuzzes(fromID, toID, 150).Count() == 100); 
        }

        [Fact()]
        public void GetBuzzTest()
        {
            var buzzRepo = new Mock<IRepository<Buzz>>();
            buzzRepo.SetReturnsDefault(GetNewBuzz());

            var service = GetBuzzService(null,buzzRepo);

            Assert.ThrowsDelegate test = () => service.GetBuzz(0);
            Assert.DoesNotThrow(test);

            test = () => service.GetBuzz(5);

            Assert.DoesNotThrow(test);
        }

        [Fact()]
        public void IsUserNameTakenTest()
        {
            var userRepo = new Mock<IRepository<BuzzUser>>();
            userRepo.SetReturnsDefault(GetUserQueryFluent(new List<BuzzUser>(){GetTestUser("dkdkdk")}));

            var service = GetBuzzService(userRepo);


            Assert.ThrowsDelegate act = () => service.IsUserNameTaken("fjf");

            Assert.DoesNotThrow(act);

            Assert.ThrowsDelegate act2 = () => service.IsUserNameTaken("");

            Assert.DoesNotThrow(act2);



        }

        [Fact()]
        public void GetUserNameSuggestionsTest_ShouldNotThrow()
        {
            var userRepo = new Mock<IRepository<BuzzUser>>();
            userRepo.SetReturnsDefault(GetUserQueryFluent(GetTestUserList(50,"jane doe")));

            var service = GetBuzzService(userRepo);

            Assert.ThrowsDelegate act = () => service.GetUserNameSuggestions("jane doe");

            Assert.DoesNotThrow(act);

            //Assert.True(service.GetUserNameSuggestions("jane doe").Count() == 3);

            

        }

        [Fact()]
        public void QueryUsersTest_ShouldNotThrow()
        {
            var userRepo = new Mock<IRepository<BuzzUser>>();
            userRepo.SetReturnsDefault(GetUserQueryFluent(GetTestUserList(50,"test_user")));
            var service = GetBuzzService(userRepo);
            const string email = "cc.lemonhead@gmail.com";
            const string name = "test_user";
            const int userId = 1;

            Assert.ThrowsDelegate act = () => service.QueryUsers(email, name, userId);
            Assert.DoesNotThrow(act);

            userRepo.SetReturnsDefault(GetUserQueryFluent(new List<BuzzUser>()));

            service = GetBuzzService(userRepo);

            act = () => service.QueryUsers(email, name, userId,50,10);

            Assert.DoesNotThrow(act);
        }

        [Fact()]
        public void QueryUserNameTest_ShouldNotThrow()
        {
            var userRepo = new Mock<IRepository<BuzzUser>>();
            userRepo.SetReturnsDefault(GetUserQueryFluent(GetTestUserList(50, "test_user")));
            var service = GetBuzzService(userRepo);
            const string email = "cc.lemonhead@gmail.com";
            const string name = "test_user";
            const int userId = 1;

            Assert.ThrowsDelegate act = () => service.QueryUserName(name);
            Assert.DoesNotThrow(act);

            userRepo.SetReturnsDefault(GetUserQueryFluent(new List<BuzzUser>()));

            service = GetBuzzService(userRepo);

            act = () => service.QueryUserName( name, 50, 10 );

            Assert.DoesNotThrow(act);
        }

        [Fact()]
        public void QueryUserIdTest_ShouldNotThrow()
        {
            var userRepo = new Mock<IRepository<BuzzUser>>();
            userRepo.SetReturnsDefault(GetUserQueryFluent(GetTestUserList(50,"test_user")));
            var service = GetBuzzService(userRepo);

            const int userId = 1;

            Assert.ThrowsDelegate act = () => service.QueryUserByID(userId);

            Assert.DoesNotThrow(act);

            Assert.ThrowsDelegate act2 = () => service.QueryUserByID(0);

            Assert.DoesNotThrow(act2);
        }

        [Fact()]
        public void AddBuzzUserTest()
        {
            var userRepo = new Mock<IRepository<BuzzUser>>();
            var user = GetTestUser("abcdefg");
            userRepo.SetReturnsDefault(GetUserQueryFluent(new List<BuzzUser>() { user }));

            var service = GetBuzzService(userRepo);

            Assert.ThrowsDelegate act = () => service.AddBuzzUser(user, true);

            Assert.DoesNotThrow(act);

            userRepo = new Mock<IRepository<BuzzUser>>();

            userRepo.SetReturnsDefault(GetUserQueryFluent(new List<BuzzUser>()));
            service = GetBuzzService(userRepo);

            Assert.ThrowsDelegate act2 = () => service.AddBuzzUser(user, true);

            Assert.DoesNotThrow(act2);
        }

        [Fact()]
        public void GetBuzzUserContactsTest_shouldNotThrow()
        {
            var userRepo = new Mock<IRepository<BuzzUser>>();
            userRepo.SetReturnsDefault(GetUserQueryFluent(GetTestUserList(99,"test_contacts")));
           // userRepo.Setup(mock => mock.Query());
           

            var service = GetBuzzService(userRepo);

            BuzzService.ContactsPageResult contactsPage = service.GetBuzzUserContacts(1, 10, 0);

           // userRepo.Verify(mock => mock.Query());

            Assert.NotNull(contactsPage);

            Assert.True(contactsPage.contacts != null);
        }

        [Fact()]
        public void GetBuzzSenderTest_shouldNotThrow()
        {
            var userRepo = new Mock<IRepository<BuzzUser>>();
            userRepo.SetReturnsDefault(GetUserQueryFluent(new List<BuzzUser>(){GetTestUser("abcdefg")}));
            var buzzRepo = new Mock<IRepository<Buzz>>();
            buzzRepo.SetReturnsDefault(GetBuzzQueryFluent(new List<Buzz>(){GetNewBuzz()}));

            var service = GetBuzzService(userRepo,buzzRepo);

            Assert.ThrowsDelegate act = () => service.GetBuzzSender(It.IsAny<string>());

            Assert.DoesNotThrow(act);
        }

        [Fact()]
        public void GetSentContactRequestsTest_shouldNotThrow()
        {
            var userRepo = new Mock<IRepository<BuzzUser>>();
            userRepo.SetReturnsDefault(GetUserQueryFluent(GetTestUserList(35)));

            var service = GetBuzzService(userRepo);

            Assert.ThrowsDelegate act = () => service.GetSentContactRequests(1);

            Assert.DoesNotThrow(act);

            Assert.ThrowsDelegate act2 = () => service.GetSentContactRequests(0);

            Assert.DoesNotThrow(act2);
        }

        [Fact()]
        public void GetContactRequestsTest_shouldNotThrow()
        {
            var userRepo = new Mock<IRepository<BuzzUser>>();
            userRepo.SetReturnsDefault(GetUserQueryFluent(GetTestUserList(35)));

            var service = GetBuzzService(userRepo);

            Assert.ThrowsDelegate act = () => service.GetContactRequests(1);

            Assert.ThrowsDelegate act2 = () => service.GetContactRequests(0);

            Assert.DoesNotThrow(act);

            Assert.DoesNotThrow(act2);
        }

        [Fact()]
        public void GetContactsTest_shouldNotThrow()
        {
            var userRepo = new Mock<IRepository<BuzzUser>>();
            userRepo.SetReturnsDefault(GetUserQueryFluent(GetTestUserList(55)));

            var service = GetBuzzService(userRepo);

            Assert.ThrowsDelegate act = () => service.GetContacts(1, 15, 5);

            Assert.ThrowsDelegate act2 = () => service.GetContacts(0);

            Assert.DoesNotThrow(act);

            Assert.DoesNotThrow(act2);


        }

        [Fact()]
        public void AddBuzzRequestTest_shouldNotThrow()
        {
            var userRepo = new Mock<IRepository<BuzzUser>>();
            userRepo.SetReturnsDefault(GetUserQueryFluent(GetTestUserList(15)));

            var service = GetBuzzService(userRepo);

            Assert.ThrowsDelegate act = () => service.AddBuzzRequest(1, 2);

            Assert.DoesNotThrow(act);
        }

        [Fact()]
        public void RemoveBuzzRequestTest_shouldNotThrow()
        {
            var userRepo = new Mock<IRepository<BuzzUser>>();
            userRepo.SetReturnsDefault(GetUserQueryFluent(GetTestUserList(45)));
            var service = GetBuzzService(userRepo);

            Assert.ThrowsDelegate act = () => service.RemoveBuzzRequest(0, 0);

            Assert.DoesNotThrow(act);
        }

        [Fact()]
        public void SignOutUserTest()
        {
            var service = GetBuzzService();

            Assert.ThrowsDelegate act = () => service.SignOutUser(GetTestUser("faketoken"));

            Assert.DoesNotThrow(act);
        }

        [Fact()]
        public void SignInUserTest()
        {
            var service = GetBuzzService();

            Assert.ThrowsDelegate act = () => service.SignInUser(GetTestUser("faketoken"));

            Assert.DoesNotThrow(act);
        }

        [Fact()]
        public void AddUserToContactsTest()
        {
            var userRepo = new Mock<IRepository<BuzzUser>>();
            userRepo.SetReturnsDefault(GetUserQueryFluent(GetTestUserList(50)));

            var service = GetBuzzService(userRepo);

            Assert.ThrowsDelegate act = () => service.AddUserToContacts(0, 0);

            Assert.ThrowsDelegate act2 = () => service.AddUserToContacts(2, 1);

            Assert.DoesNotThrow(act);

            Assert.DoesNotThrow(act2);
        }

        [Fact()]
        public void RemoveUserFromContactsTest()
        {
            var userRepo = new Mock<IRepository<BuzzUser>>();
            userRepo.SetReturnsDefault(GetUserQueryFluent(GetTestUserList(75)));

            var service = GetBuzzService(userRepo);

            Assert.ThrowsDelegate act = () => service.RemoveUserFromContacts(0, 0);
            Assert.ThrowsDelegate act2 = () => service.RemoveUserFromContacts(2, 1);

            Assert.DoesNotThrow(act);
            Assert.DoesNotThrow(act2);
        }

        [Fact()]
        public void DeleteBuzzUserTest()
        {
            var userRepo = new Mock<IRepository<BuzzUser>>();
            var userList = GetTestUserList(45);
            userRepo.SetReturnsDefault(GetUserQueryFluent(userList));
            var service = GetBuzzService(userRepo);
            var user = userList[5];

            Assert.ThrowsDelegate act = () => service.DeleteBuzzUser(user);

            Assert.DoesNotThrow(act);
        }

    }
}


