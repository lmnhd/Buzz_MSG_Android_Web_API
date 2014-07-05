using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using BuzzMSGEntity.Models;
using BuzzMSGServices;
using BuzzMSGWebAPI.Helpers;
using BuzzMSGWebAPI.Models;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;

namespace BuzzMSGWebAPI.Controllers
{
    public class BuzzUsersController : ApiController
    {
        private readonly IBuzzService _service;
        private readonly IUnitOfWork _unit;

        public BuzzUsersController(IBuzzService service, IUnitOfWork unit)
        {
            _service = service;
            _unit = unit;
        }

        

        [HttpGet]
        [Route("api/Buzz/RemoveContact")]
        public IHttpActionResult RemoveContact(int userID, int contactToRemove)
        {
            _service.RemoveUserFromContacts(contactToRemove, userID);
            return Ok();
        }
        [HttpGet]
        [Route("api/Buzz/GetSentRequests")]
        public IHttpActionResult GetSentRequests(int userID, int count = 10, int page = 0)
        {
            var result = _service.GetSentContactRequests(userID, count, page);
            return Ok(result);
        }
        [HttpGet]
        [Route("api/Buzz/GetReceivedRequests")]
        public IHttpActionResult GetReceivedRequests(int userID, int count = 10, int page = 0)
        {
            var result = _service.GetContactRequests(userID, count, page);
            return Ok(result);
        }
        [HttpGet]
        [Route("api/Buzz/GetContact")]
        public IHttpActionResult GetContact(int id)
        {
           return Ok(_service.GetContact(id));
        }

        [HttpGet]
        [Route("api/Buzz/QueryUsers")]
        public IHttpActionResult QueryUsers(int count = 10, int start = 0)
        {
            var result = new List<BuzzUser>();
            int userID;
            int.TryParse(Request.GetHeader("userID"), out userID);
            var name = Request.GetHeader("name");
            var email = Request.GetHeader("email");
            if (name.Equals(""))
            {
                name = null;
            }
            if (email.Equals(""))
            {
                email = null;
            }

            if (userID != 0)
            {
                var user = _service.GetBuzzUser(userID);
                if (user != null)
                {
                    var returnOBJ = new { result = "contact", list = new List<BuzzUser>(), singleContact = user };
                    return Ok(returnOBJ);
                }

            }
            if (name != null)
            {
                result.AddRange(_service.QueryUserName(name, count, start));

                if (!result.Any())
                {
                    if (email != null)
                    {
                        result.AddRange(_service.QueryUsers(email, name, userID, count - result.Count(), result.Any() ? 0 : start));


                        if (result.Any())
                        {
                            var returnOBJ = new { result = "contacts", list = result, singleContact = "" };
                            return Ok(returnOBJ);
                        }

                    }
                }
            }
            if (email != null && !result.Any())
            {

                result.AddRange(_service.QueryUsers(email, name, userID, count, start));

                if (result.Any())
                {
                    return Ok(new { result = "contacts", list = result, singleContact = "" });
                }


            }
            if (!result.Any())
            {

                return StatusCode(HttpStatusCode.NoContent);
            }
            else
            {
                var returnObj = new { result = "contacts", list = result, singleContact = "" };
                return Ok(returnObj);
            }

        }

        [HttpGet]
        [Route("api/addtestusers")]
        public void Addtestusers()
        {
            _service.AddTestUsers();
        }

        // GET: api/BuzzUsers
        public IQueryable<BuzzUser> GetBuzzUsers()
        {
            return _service.GetBuzzUsers();
        }

        // GET: api/BuzzUsers/5
        [ResponseType(typeof(BuzzUser))]
        public IHttpActionResult GetBuzzUser(int id)
        {
            BuzzUser buzzUser = _service.GetBuzzUser(id);
            if (buzzUser == null)
            {
                return NotFound();
            }

            return Ok(buzzUser);
        }

        // PUT: api/BuzzUsers/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutBuzzUser(int id, BuzzUser buzzUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != buzzUser.DataBaseID)
            {
                return BadRequest();
            }

            buzzUser.ObjectState = ObjectState.Modified;
            

            try
            {
                var saveChanges = _unit.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BuzzUserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/BuzzUsers
        [ResponseType(typeof(BuzzUser))]
        public IHttpActionResult PostBuzzUser(BuzzUser buzzUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _service.AddBuzzUser(buzzUser, false);
            _unit.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = buzzUser.DataBaseID }, buzzUser);
        }

        // DELETE: api/BuzzUsers/5
        [ResponseType(typeof(BuzzUser))]
        public IHttpActionResult DeleteBuzzUser(int id)
        {
            BuzzUser buzzUser = _service.GetBuzzUser(id);
            if (buzzUser == null)
            {
                return NotFound();
            }

            _service.DeleteBuzzUser(buzzUser);
            _unit.SaveChanges();

            return Ok(buzzUser);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _unit.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BuzzUserExists(int id)
        {
            return _service.GetBuzzUsers().Count(e => e.DataBaseID == id) > 0;
        }
    }
}