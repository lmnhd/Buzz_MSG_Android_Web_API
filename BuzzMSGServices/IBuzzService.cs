using System;
using System.Collections.Generic;
using System.Linq;
using BuzzMSGEntity.Models;

namespace BuzzMSGServices
{
    public interface IBuzzService
    {
        Buzz AddBuzz(Buzz bz);
        BuzzUser GetBuzzUserByAt(String at);
        IEnumerable<Buzz> GetUnsentBuzzes(BuzzUser user);
        Buzz GetBuzz(int fromUserID, string buzzID, Boolean markAsSent);
        void UpdateLastResponseTime(BuzzUser bu);
        Boolean MarkBuzzAsSent(Buzz bz);
        IEnumerable<Buzz> GetBuzzes(int fromUserID, int toUserID, int numberOfBuzzes);
        Buzz GetBuzz(int BuzzID);
        Boolean IsUserNameTaken(string usrname);
        IEnumerable<string> GetUserNameSuggestions(string username);
        IEnumerable<BuzzUser> QueryUsers(string email, string name, int userID, int count = 10, int start = 0);
        IEnumerable<BuzzUser> QueryUserName(string userName, int count = 10, int start = 0);
        BuzzUser QueryUserByID(int userID);
        BuzzUser AddBuzzUser(BuzzUser user, bool signIn);
        BuzzUser GetBuzzUser(int dbID);
        BuzzService.ContactsPageResult GetBuzzUserContacts(int id, int count, int start);
        BuzzUser GetBuzzSender(string buzzIdentity);
        List<BuzzUser> GetSentContactRequests(int fromID, int count = 0, int start = 0);
        List<BuzzUser> GetContactRequests(int toID, int count = 10, int start = 0);
        BuzzUser GetContact(int id);
        List<BuzzUser> GetContacts(int dbID, int count = 10, int start = 0);

        BuzzUser DeleteBuzzUser(BuzzUser buzzUser);
        void AddBuzzRequest(int fromID, int toID);
        void RemoveBuzzRequest(int fromID, int toID);
        void SignOutUser(BuzzUser user);
        void SignInUser(BuzzUser user);
        void AddUserToContacts(int userToAddID, int userAddingToID);
        void RemoveUserFromContacts(int userToRemoveID, int userRemovingFromID);
        void AddTestUsers();
        IQueryable<Buzz> GetBuzzs();

        IQueryable<BuzzUser> GetBuzzUsers();
    }
}