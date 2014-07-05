//using BuzzMSGEntity.Models;
using BuzzMSGEntity.Models;
using BuzzMSGServices;
using Moq;
using Repository.Pattern.Repositories;
using Xunit;

namespace BuzzMSGServicesTests3
{
    public class BuzzServiceTests
    {
        [Fact()]
        public void AddBuzzTest_ShouldNotThrowException()
        {
            //Arrange
            Mock<IRepository<BuzzUser> > mockUserRepo = new Mock<IRepository<BuzzUser>>();
            Mock<IRepository<Buzz>> mockBuzzRepo = new Mock<IRepository<Buzz>>();
            BuzzService service = new BuzzService(mockUserRepo.Object,mockBuzzRepo.Object);
            Buzz bz = new Buzz() {BuzzIdentity = "410934857748", FilePath = "", Message = "", Type = "voice"};

            Assert.ThrowsDelegate act = () => service.AddBuzz(bz);
            Assert.DoesNotThrow(act);
           
        }

        [Fact()]
        public void GetBuzzUserByAtTest()
        {
            Assert.True(false, "not implemented yet");
        }
    }
}
