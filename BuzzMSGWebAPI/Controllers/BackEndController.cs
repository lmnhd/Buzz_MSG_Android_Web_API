using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using BuzzMSGEntity.Models;
using BuzzMSGWebAPI.Helpers;
using Microsoft.Owin.Security.Provider;

namespace BuzzMSGWebAPI.Controllers
{
    public class BackEndController : ApiController
    {
        private readonly BackEndHelpers _helpers;

        public BackEndController()
        {
            _helpers = new BackEndHelpers();
        }
        [Route("api/Buzz/GetServerTempFileOrPath")]
        [HttpGet]
        public  List<string> GetServerTempFileOrPath(string title, string filename = "")
        {
            

            return _helpers.GetServerTempFileOrPath(title,filename);

        }
      



    }
}
