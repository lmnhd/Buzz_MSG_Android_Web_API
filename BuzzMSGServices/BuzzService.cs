using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuzzMSGEntity.Models;
using Repository.Pattern.Repositories;

namespace BuzzMSGServices
{
    public class BuzzService : IBuzzService
    {
        private readonly IRepository<BuzzUser> _userRepository;
        private readonly IRepository<Buzz> _buzzRepository;
        public BuzzService(IRepository<BuzzUser> userRepository,IRepository<Buzz> buzzRepository )
        {
            _buzzRepository = buzzRepository;
            _userRepository = userRepository;
        }

        public Buzz AddBuzz(Buzz bz)
        {
          
            bz.ServerTime = DateTime.Now;
            bz.ReceivedTime = new DateTime(2000, 1, 1);
            _buzzRepository.Insert(bz);
            
            return bz;
        }
        public BuzzUser GetBuzzUserByAt(String at)
        {
            BuzzUser user = null;
            var q = _userRepository.Query(u => u.current_at.Equals(at));
            if (q != null)
            {
                var select = q.Select();
                user = select.SingleOrDefault();
            }
            
            

            return user;
        }

        public IEnumerable<Buzz> GetUnsentBuzzes(BuzzUser user)
        {
            var defaultDate = new DateTime(2000, 1, 1);
            return _buzzRepository.Query(b => b.ToUserID == user.DataBaseID && b.ReceivedTime.Equals(defaultDate)).Select().OrderBy<Buzz,int>(b => b.Id);
        }
        public Buzz GetBuzz(int fromUserID, string buzzID, Boolean markAsSent)
        {

            var q = _buzzRepository.Query(b => b.FromUserID == fromUserID && b.BuzzIdentity == buzzID);
            var select = q.Select();
            var result = select.SingleOrDefault();
            if (result == null)
            {
                return null;
            }
            if (markAsSent)
            {
                result.ReceivedTime = DateTime.Now;
                _buzzRepository.Update(result);
            }

            return result;
        }

        public void UpdateLastResponseTime(BuzzUser bu)
        {
            bu.lastResponse = DateTime.Now;
            _userRepository.Update(bu);
        }
        public Boolean MarkBuzzAsSent(Buzz bz)
        {
            try
            {
                bz.ReceivedTime = DateTime.Now;
                _buzzRepository.Update(bz);
                return true;
            }
            catch (Exception e)
            {

            }

            return false;
        }

        public IEnumerable<Buzz> GetBuzzes(int fromUserID, int toUserID, int numberOfBuzzes)
        {
            IEnumerable<Buzz> myBs;
            if (fromUserID.Equals(0))
            {
                return new List<Buzz>();
            }

            if (toUserID.Equals(0))
            {
                if (numberOfBuzzes > 0)
                {
                    myBs = _buzzRepository.Query(b => b.FromUserID.Equals(fromUserID)).Select().Take(numberOfBuzzes);
                }
                else
                {
                    myBs = _buzzRepository.Query(b => b.FromUserID.Equals(fromUserID)).Select();
                }

            }
            else
            {
                if (numberOfBuzzes > 0)
                {
                    myBs = _buzzRepository.Query(b => b.FromUserID.Equals(fromUserID) && b.ToUserID.Equals(toUserID)).Select().Take(numberOfBuzzes);
                }
                else
                {
                    myBs = _buzzRepository.Query(b => b.FromUserID.Equals(fromUserID) && b.ToUserID.Equals(toUserID)).Select();
                }



            }

            return myBs;

        }

        public Buzz GetBuzz(int BuzzID)
        {
            return _buzzRepository.Find(BuzzID);
        }
        public Boolean IsUserNameTaken(string usrname)
        {
            var user = _userRepository.Query(u => u.UserName.Equals(usrname, StringComparison.OrdinalIgnoreCase)).Select();
            return user != null;
        }
        public IEnumerable<string> GetUserNameSuggestions(string username)
        {
            var result = new List<string>();
            var users = _userRepository.Query(u => u.UserName.Equals(username, StringComparison.OrdinalIgnoreCase)).Select();
            var count = 3;
            var userCount = users.Count();
            var rand = new Random(userCount);
            if (userCount > 0)
            {
                while (count > 0)
                {
                    var newName = string.Format("{0} {1}", username, rand.Next(userCount + 1, userCount + 100));
                    if (!IsUserNameTaken(newName))
                    {
                        result.Add(newName);
                        count--;
                    }
                }
            }
            return result;
        }

        public IEnumerable<BuzzUser> QueryUsers(string email, string name, int userID, int count = 10, int start = 0)
        {
            return _userRepository.Query(u => u.email.ToLower().Contains(email.ToLower()) || u.UserName.ToLower().Contains(name.ToLower()) || u.name.ToLower().Contains(name.ToLower()) || u.email.ToLower().Contains(name.ToLower()) || email.ToLower().Contains(u.name.ToLower()) || u.DataBaseID.Equals(userID)).Select().OrderBy(u => u.DataBaseID).Skip(start).Take(count);
        }


        public IEnumerable<BuzzUser> QueryUserName(string userName, int count = 10, int start = 0)
        {
            return _userRepository.Query(u => u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase) || u.UserName.ToLower().Contains(userName.ToLower())).Select().OrderBy(u => u.DataBaseID).Skip(start).Take(count);
        }


        public BuzzUser QueryUserByID(int userID)
        {
            return _userRepository.Query(u => u.DataBaseID == userID).Select().FirstOrDefault();
        }

        public BuzzUser AddBuzzUser(BuzzUser user, bool signIn)
        {

            var existing = _userRepository.Query(u => u.email == user.email).Select().FirstOrDefault();
            if (existing != null)
            {
                existing.refresh_token = user.refresh_token;
                existing.current_at = user.current_at;
                existing.Country = user.Country;
                existing.Locale = user.Locale;
                existing.UserName = user.UserName;
                existing.registration_id = user.registration_id;
                existing.base64PhotoString = user.base64PhotoString;
                existing.picture = user.picture;

                existing.lastResponse = DateTime.Now;
                if (signIn)
                {
                    user.isOnline = true;
                }
                _userRepository.Update(existing);


            }
            else
            {

                try
                {
                    user.dateAdded = DateTime.Now;
                    user.lastResponse = DateTime.Now;
                    if (signIn)
                    {
                        user.isOnline = true;
                    }
                    _userRepository.Insert(user);
                    existing = user;

                }
                catch (Exception e)
                {
                    return null;
                }

            }

           
            return existing;
        }

        public BuzzUser GetBuzzUser(int dbID)
        {
            return _userRepository.Query(u => u.DataBaseID.Equals(dbID)).Select().FirstOrDefault();

        }
        public struct ContactsPageResult
        {
            public bool hasMore { get; set; }
            public List<BuzzUser> contacts { get; set; }

        }
        public ContactsPageResult GetBuzzUserContacts(int id, int count, int start)
        {
            var user = _userRepository.Query(u => u.DataBaseID == id).Include(u => u.Contacts).Select().FirstOrDefault();
            var result = new List<BuzzUser>();
            bool isMore = false;
            if (user != null)
            {
                if (user.Contacts != null)
                {
                    var idList = user.Contacts.Skip(start).Take(count);
                    if (idList.Any())
                    {
                        foreach (BuzzUser ui in idList)
                        {
                            var bu = _userRepository.Find(ui);
                            if (bu != null)
                            {
                                result.Add(_userRepository.Find(ui));
                            }

                        }
                        if (user.Contacts.Count() > (start + result.Count()))
                        {
                            isMore = true;
                        }
                    }

                }
            }
            return new ContactsPageResult() { contacts = result, hasMore = isMore };
        }
        public BuzzUser GetBuzzSender(string buzzIdentity)
        {
            var buzz = _buzzRepository.Query(b => b.BuzzIdentity.Equals(buzzIdentity)).Select().FirstOrDefault();
            if (buzz != null)
            {
                var sender = _userRepository.Query(u => u.UserID == buzz.FromUserID).Select().FirstOrDefault();

                if (sender != null)
                {
                    return sender;
                }
            }
            return null;
        }

        public List<BuzzUser> GetSentContactRequests(int fromID, int count = 0, int start = 0)
        {
            var user = _userRepository.Query(u => u.DataBaseID == fromID).Include(u => u.SentContactRequests).Select().FirstOrDefault();
            var result = new List<BuzzUser>();
            if (user != null)
            {
                if (user.SentContactRequests != null)
                {
                    result = user.SentContactRequests.Skip(start).Take(count).ToList();
                   
                }

            }
            return result;
        }
        public List<BuzzUser> GetContactRequests(int toID, int count = 10, int start = 0)
        {
            var user = _userRepository.Query(u => u.DataBaseID == toID).Include(u => u.ReceivedContactRequests).Select().FirstOrDefault();
            var result = new List<BuzzUser>();
            if (user != null)
            {
                if (user.ReceivedContactRequests != null)
                {
                    result = user.ReceivedContactRequests.Skip(start).Take(count).ToList();
                    
                }
            }
            return result;

        }

        public BuzzUser GetContact(int id)
        {
            return _userRepository.Query(b => b.UserID == id).Select().FirstOrDefault();
        }
        public List<BuzzUser> GetContacts(int dbID, int count = 10, int start = 0)
        {
            var me = _userRepository.Query(u => u.DataBaseID == dbID).Include(u => u.Contacts).Select().FirstOrDefault();
            var result = new List<BuzzUser>();
            if (me != null)
            {
                if (me.Contacts != null)
                {
                    result = me.Contacts.Skip(start).Take(count).ToList();
                    

                }
            }
            return result;
        }

        public void AddBuzzRequest(int fromID, int toID)
        {
            var from = _userRepository.Query(u => u.DataBaseID == fromID).Include(u => u.SentContactRequests).Include(u => u.Contacts).Select().FirstOrDefault();
            if (from != null)
            {
                var to = _userRepository.Query(u => u.DataBaseID == toID).Include(u => u.ReceivedContactRequests).Include(u => u.Contacts).Select().FirstOrDefault();

                if (to != null)
                {
                    var existing = to.Contacts.SingleOrDefault(u => u.DataBaseID == fromID);
                    if (existing != null)
                    {
                        return;
                    }
                    if (to.ReceivedContactRequests == null)
                    {
                        to.ReceivedContactRequests = new List<BuzzUser>();
                    }
                    to.ReceivedContactRequests.Add(from);
                    if (from.SentContactRequests == null)
                    {
                        from.SentContactRequests = new List<BuzzUser>();

                    }
                    from.SentContactRequests.Add(to);
                   _userRepository.Update(from);
                    _userRepository.Update(to);
                }
            }

        }
        public void RemoveBuzzRequest(int fromID, int toID)
        {
            var to = _userRepository.Query(u => u.DataBaseID == toID).Include(b => b.ReceivedContactRequests).Select().FirstOrDefault();
            if (to != null)
            {
                if (to.ReceivedContactRequests != null)
                {
                    var from = to.ReceivedContactRequests.SingleOrDefault(u => u.DataBaseID == toID);
                    if (from != null)
                    {
                        to.ReceivedContactRequests.Remove(from);

                       _userRepository.Update(to);
                    }
                }
            }
        }

        public void SignOutUser(BuzzUser user)
        {

            if (user != null)
            {
                user.isOnline = false;
                _userRepository.Update(user);
            }
        }
        public void SignInUser(BuzzUser user)
        {
            user.isOnline = true;
            _userRepository.Update(user);
        }
        public void AddUserToContacts(int userToAddID, int userAddingToID)
        {
            var userToAdd = _userRepository.Query(u => u.DataBaseID == userToAddID).Include(u => u.Contacts).Select().FirstOrDefault();
            if (userToAdd != null)
            {
                var userAddingTo = _userRepository.Query(u => u.DataBaseID == userAddingToID).Include(u => u.Contacts).Select().FirstOrDefault();
                if (userAddingTo != null)
                {
                    if (userAddingTo.Contacts == null)
                    {
                        userAddingTo.Contacts = new List<BuzzUser>();

                    }
                    if (userToAdd.Contacts == null)
                    {
                        userToAdd.Contacts = new List<BuzzUser>();
                    }
                    userAddingTo.Contacts.Add(userToAdd);
                    userToAdd.Contacts.Add(userAddingTo);
                    _userRepository.Update(userAddingTo);
                    _userRepository.Update(userToAdd);
                }
            }
        }
        public void RemoveUserFromContacts(int userToRemoveID, int userRemovingFromID)
        {
            var toRem = _userRepository.Query(u => u.DataBaseID == userToRemoveID).Include(u => u.Contacts).Select().FirstOrDefault();
            if (toRem != null)
            {
                var remFrom = _userRepository.Query(u => u.DataBaseID == userRemovingFromID).Include(u => u.Contacts).Select().FirstOrDefault();

                if (remFrom != null)
                {
                    if (toRem.Contacts != null)
                    {
                        BuzzUser fid = toRem.Contacts.SingleOrDefault(c => c.DataBaseID == remFrom.DataBaseID);
                        if (fid != null)
                        {
                            toRem.Contacts.Remove(fid);

                            if (remFrom.Contacts != null)
                            {
                                BuzzUser tid = remFrom.Contacts.SingleOrDefault(c => c.DataBaseID == toRem.DataBaseID);
                                if (tid != null)
                                {
                                    remFrom.Contacts.Remove(tid);

                                }
                            }
                        }
                    }
                }
              _userRepository.Update(remFrom);
                _userRepository.Update(toRem);
            }
        }

        public void AddTestUsers()
        {
            var count = _userRepository.Query(u => u.DataBaseID != 0).Select().Count();
            if (count < 3)
            {
                var orig = _userRepository.Query(u => u.DataBaseID != 0).Select().FirstOrDefault();
                if (orig != null)
                {
                    for (var i = 0; i < 51; i++)
                    {
                        var name = "testartist" + DateTime.Now.Ticks.ToString();
                        BuzzUser temp = new BuzzUser
                        {
                            base64PhotoString = orig.base64PhotoString,
                            SentContactRequests = new List<BuzzUser>(),
                            Contacts = new List<BuzzUser>(),
                            Country = orig.Country,
                            current_at = orig.current_at,
                            dateAdded = DateTime.Now,
                            email = string.Format("{0}@gmail.com", name),
                            FaceBook = "",
                            family_name = name,
                            gender = "female",
                            given_name = name,
                            lastResponse = DateTime.Now,
                            Locale = orig.Locale,
                            name = name,
                            picture = orig.picture,
                            refresh_token = orig.refresh_token,
                            registration_id = orig.registration_id,
                            UserID = orig.UserID,
                            UserName = name,
                            verified_email = "true",
                            link = orig.link



                        };

                        _userRepository.Insert(temp);

                    }
                   
                }
            }

        }



        public IQueryable<Buzz> GetBuzzs()
        {
            return _buzzRepository.Queryable();
        }


        public IQueryable<BuzzUser> GetBuzzUsers()
        {
            return _userRepository.Queryable();
        }

        public BuzzUser DeleteBuzzUser(BuzzUser buzzUser)
        {
            buzzUser.Contacts.Clear();
            buzzUser.ReceivedContactRequests.Clear();
            buzzUser.SentContactRequests.Clear();
            _userRepository.Delete(buzzUser);
            return buzzUser;
        }
    }



}
