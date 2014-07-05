using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;
using System.Web.WebPages;
using System.Web.WebSockets;
using BuzzMSGEntity.Models;
using BuzzMSGServices;
using BuzzMSGWebAPI.Helpers;
using Newtonsoft.Json;
using Repository.Pattern.UnitOfWork;

namespace BuzzMSGWebAPI.Controllers
{
    public class BuzzController : ApiController
    {
        private readonly IBuzzService _service;
        private readonly IUnitOfWork _unit;
        private readonly BackEndHelpers _helpers;
        //private readonly ApplicationDbContext db = new ApplicationDbContext();

        private const string cid = "913986963380-vr8g4e4uvem17r8s4c18cmmdun64bcl9.apps.googleusercontent.com";
        private const string cis = "aBCPNgSyS6w92Nfvt1gKc7Go";
        private const string api_key = "AIzaSyCh50BrzLLiKMi3gEBpXCGV9ZaCWkzpmC4";


       
        public struct GoogleAccesTokenHolder
        {
            public string access_token { get; set; }
            public string token_type { get; set; }

            public string expires_in { get; set; }

            public string id_token { get; set; }

            public string refresh_token { get; set; }
        }
       
        public BuzzController(IBuzzService service, IUnitOfWork unit)
        {
            _service = service;
            _unit = unit;
            _helpers = new BackEndHelpers();
        }

        [Route("api/Buzz/Buzz")]
        [HttpPost]
        public async Task<IHttpActionResult> Buzz()
        {


            string currentPath = "";
            Int32 senderID = 0;
            Int32 receiverID = 0;
            String buzzType = "";
            DateTime time = DateTime.Now;
            Boolean isReply = false;
            string message = "";
            string buzzID = "";
            string root = "";

            BuzzUser recipient;
            BuzzUser sender;



           Buzz buzz = new Buzz();
            
           buzzID = Request.GetHeader("buzzidentity");

            if (buzzID == null)
            {
                return BadRequest(message: "Invalid parameter string:buzz_id");
                //return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid parameter string:buzz_id");
            }

            string senderid = Request.GetHeader("fromuserid");
            if (senderid != null)
            {
                if (!Int32.TryParse(senderid, out senderID))
                {
                    return BadRequest(message: "Invalid parameter Int:fromuserid");
                }

            }
            else
            {
                return BadRequest(message: "Invalid parameter Int:fromuserid");
            }

            string recieverid = Request.GetHeader("userToID");
            if (recieverid != null)
            {
                if (!Int32.TryParse(recieverid, out receiverID))
                {
                    return BadRequest(message: "Invalid parameter Int:usertoid");
                }

            }
            else
            {
                return BadRequest(message: "Invalid parameter Int:usertoid");
            }

            buzzType = Request.GetHeader("buzztype");

            string reply = Request.GetHeader("isbuzzreply");

            if (reply != null)
            {
                if (!Boolean.TryParse(reply, out isReply))
                {
                    isReply = false;
                }
            }

            if (buzzType == null)
            {
                return BadRequest(message: "Invalid parameter string:buzztype");
            }
            DateTime dt = _helpers.GetDateTimeFromJavaCurrentMillis(buzzID);




            buzz.BuzzIdentity = buzzID;
            buzz.IsReply = isReply;
            buzz.Time = dt;
            buzz.ToUserID = receiverID;
            buzz.FromUserID = senderID;
            buzz.Type = buzzType;

            recipient = _service.GetBuzzUser(buzz.ToUserID);

            if (recipient == null)
            {
                return BadRequest("The requested user was not found");
            }

            sender = _service.GetBuzzUser(buzz.FromUserID);

            if (sender == null)
            {
                return BadRequest( "The sending party could not be found.");
            }




            if (buzzType == "text")
            {
                if (message.IsEmpty())
                {
                    return BadRequest("Invalid parameter string:buzzmessage");
                }

                buzz.Message = message;
                _service.AddBuzz(buzz);


            }
            else if (buzzType == "voice")
            {
                //string id = Request.Headers.
                if (!Request.Content.IsMimeMultipartContent())
                {
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                }

                root = _helpers.BuzzHelperGetVoiceTempLocation();
                string finalPath = root;
                if (!System.IO.Directory.Exists(root))
                {
                    System.IO.Directory.CreateDirectory(root);
                }
                var provider = new MultipartFormDataStreamProvider(root);

                try
                {
                    await Request.Content.ReadAsMultipartAsync(provider);

                    foreach (MultipartFileData file in provider.FileData)
                    {

                        currentPath = file.LocalFileName;
                        buzz.FilePath = _helpers.BuzzHelperMoveVoiceFileToLocation(buzz, currentPath);
                        //Console.WriteLine(file.Headers.ContentDisposition.FileName);
                    }







                    _service.AddBuzz(buzz);
                    _unit.SaveChanges();

                }
                catch (Exception e)
                {
                    return InternalServerError(e);
                }
            }


            //send notification
            try
            {
                Directory.Delete(root, true);
            }
            catch (Exception)
            {

            }

            var gcmObj = new
            {
                registration_ids = new string[] { recipient.registration_id },
                data = new
                {
                    message = message,
                    type = buzzType,
                    buzzID = buzz.BuzzIdentity,
                    fromUser = buzz.FromUserID.ToString()
                }
            };
            var gcmJsonString = JsonConvert.SerializeObject(gcmObj);

            string response = SendGCMNotification(api_key, gcmJsonString);

            if (response != "")
            {

            }
            var obj = new
            {
                serverID = buzz.Id,
                fromID = buzz.FromUserID,
                toID = buzz.ToUserID,
                //fromName =
                isreply = buzz.IsReply,
                servertime = buzz.ServerTime.ToShortTimeString(),
                isOnline = recipient.isOnline,
                buzzidentity = buzz.BuzzIdentity,
                receivedTime = buzz.ReceivedTime > new DateTime(2010, 1, 1) ? buzz.ReceivedTime.ToShortTimeString() : ""
            };
            return Ok( JsonConvert.SerializeObject(obj));

        }


        [Route("api/Buzz/RegisterUser")]
        [HttpGet]
        public IHttpActionResult RegisterUser(string at)
        {
            try
            {
                string regID = Request.GetHeader("regID");
                string country = Request.GetHeader("country");
                if (country == null)
                {
                    country = "";
                }

                string locale = Request.GetHeader("locale");
                if (locale == null)
                {
                    locale = "";
                }
                IHttpActionResult result = RegisterUserAsync(at, regID, country, locale).Result;
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException);
            }
            return Ok();
        }
        public async Task<IHttpActionResult> RegisterUserAsync(string at, string regID, string country, string locale)
        {

            if (at == null)
            {
                return new ExceptionResult(new Exception("No Access Token..."),this);
            }
            var client = new HttpClient { BaseAddress = new Uri("https://accounts.google.com") };

            string access_token;
            string id_token;
            string email;

            string aud;
            string azp = null;
            string sub;
            string email_verified;
            string iss;
            var json = new GoogleAccesTokenHolder();

            String responseMessage = @"GoogleOAuth2 Validation Failed  ";

            var data = new Dictionary<string, string>();
            data.Add("code", at);
            data.Add("client_id", cid);
            data.Add("client_secret", cis);
            data.Add("grant_type", "authorization_code");


            var b = _service.GetBuzzUserByAt(at: at);


            if (b != null)
            {
                b.registration_id = regID;


                b = _service.AddBuzzUser(b, true);



                return Ok( JsonConvert.SerializeObject(b));
            }
            else
            {
                var response = client.PostAsync(
            "/o/oauth2/token",
            new FormUrlEncodedContent(data)).Result;

                if (response.IsSuccessStatusCode)
                {
                    string responseString = await response.Content.ReadAsStringAsync();

                    if (responseString != null)
                    {
                        json = JsonConvert.DeserializeObject<GoogleAccesTokenHolder>(responseString);

                        if (json.access_token != null)
                        {
                            access_token = json.access_token;
                            id_token = json.id_token;
                            // HttpClient client2 = new HttpClient { BaseAddress = new Uri("http://" + Request.RequestUri.Authority) };

                            var jwt = new JwtSecurityToken(json.id_token);

                            var handler = new JwtSecurityTokenHandler();

                            // var claims = handler.ValidateToken(json.id_token);

                            if (jwt != null)
                            {


                                aud = jwt.Audience;


                                var firstOrDefault = jwt.Claims.FirstOrDefault(c => c.Type == "azp");
                                if (firstOrDefault != null)
                                    azp = firstOrDefault.Value;
                                email = jwt.Claims.SingleOrDefault(c => c.Type == "email").Value;
                                email_verified = jwt.Claims.FirstOrDefault(c => c.Type == "email_verified").Value;
                                iss = jwt.Claims.FirstOrDefault(c => c.Type == "iss").Value;
                                sub = jwt.Claims.FirstOrDefault(c => c.Type == "sub").Value;

                                if (aud != cid)
                                {
                                    responseMessage += " : " + "aud value incorrect.";
                                }
                                else if (azp != cid)
                                {
                                    responseMessage += " : " + "azp value incorrect.";
                                }
                                else
                                {
                                    responseMessage = "jwt validated successfully...";
                                    client.Dispose();
                                    client = new HttpClient();
                                    string query = string.Format("?access_token={0}", access_token);
                                    var request = new HttpRequestMessage { RequestUri = new Uri("https://www.googleapis.com/oauth2/v2/userinfo" + query), Method = HttpMethod.Get };
                                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
                                    request.Headers.Add("Authorization", string.Format("Bearer {0}", access_token));
                                    //client2.SetBearerToken(json.id_token);
                                    // var data2 = new Dictionary<string, string>();
                                    // data2.Add("Authorization", string.Format("Bearer {0}", access_token));

                                    response = client.SendAsync(request).Result;
                                    var tempFolder = "";
                                    if (response.IsSuccessStatusCode)
                                    {
                                        responseString = await response.Content.ReadAsStringAsync();

                                        var userData = JsonConvert.DeserializeObject<BuzzUser>(responseString);



                                        if (userData != null)
                                        {
                                            if (userData.picture != null && userData.picture.Contains("."))
                                            {
                                                try
                                                {
                                                    var string64AndTempFolder = _helpers. GetBase64StringFromPhotoUrl(userData.picture, userData.name + "_" + DateTime.Now.Ticks.ToString(), userData.given_name);
                                                    userData.base64PhotoString = string64AndTempFolder.Item1;
                                                    tempFolder = string64AndTempFolder.Item2;




                                                }
                                                catch (Exception e)
                                                {

                                                }

                                            }

                                            userData.current_at = at;
                                            userData.refresh_token = json.refresh_token;
                                            userData.UserName = userData.name;
                                            userData.registration_id = regID;
                                            userData.Locale = locale;
                                            userData.Country = country;

                                            userData = _service.AddBuzzUser(userData, true);
                                           // HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                                          //  request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());


                                            Task.Factory.StartNew(() => _helpers.DeleteTempDir(tempFolder));


                                            return Ok( JsonConvert.SerializeObject(userData));

                                        }
                                        else
                                        {
                                            responseMessage += " : " + "Problem getting googleapis userdata";
                                        }

                                    }
                                    else
                                    {
                                        responseMessage += " : " + "Problem getting googleapis userinfo : " + response.StatusCode + " : " + response.ReasonPhrase;
                                    }
                                }

                            }




                        }


                    }







                }
                else
                {
                    responseMessage += " : " + response.StatusCode.ToString() + " : " + response.ReasonPhrase;
                }
            }




            return BadRequest( responseMessage);

        }

        [HttpGet]
        [Route("api/Buzz/CheckForMessage")]

        public IHttpActionResult CheckForMessage(int id)
        {
            if (id == 0)
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            var result = new HttpResponseMessage();
            var sender = _service.GetBuzzUser(id);
            var buzzes = _service.GetUnsentBuzzes(sender).ToList();
            var count = buzzes.Count;
            if (count > 0)
            {

                var bz = buzzes[0];


                var hasMore = count > 1;
                var nextID = "";
                var nextFromUser = 0;
                if (hasMore)
                {
                    nextID = buzzes.ElementAt(1).BuzzIdentity;
                    nextFromUser = buzzes.ElementAt(1).FromUserID;
                }

                result.Headers.Add("nextID", nextID);
                result.Headers.Add("nextFromUser", nextFromUser.ToString());

                result = PrepareBuzzSend(bz.BuzzIdentity, result, sender, bz);

                _service.MarkBuzzAsSent(bz);




            }
            return ResponseMessage(result);
        }

        [Route("api/Buzz/GetBuzz")]
        [HttpGet]
        public IHttpActionResult GetBuzz(String id, String fromUser)
        {
            var result = new HttpResponseMessage(HttpStatusCode.PreconditionFailed);
            int senderId;
            var ok = int.TryParse(fromUser, out senderId);

            if (ok)
            {
                var sender = _service.GetBuzzUser(senderId);
                var buzz = _service.GetBuzz(sender.DataBaseID, id, true);

                var response = PrepareBuzzSend(id, result, sender, buzz);



            }


            return ResponseMessage(result);
        }
        //public class BuzzProxy
        //{
        //    string Message;

        //}

        private HttpResponseMessage PrepareBuzzSend(string identity, HttpResponseMessage result, BuzzUser sender, Buzz buzz)
        {
            if (sender == null || buzz == null) return result;
            var endPath = string.Format("~/Buzz/{0}/{1}.amr", sender.DataBaseID, identity);
            var path = HttpContext.Current.Server.MapPath(endPath);
            if (!System.IO.File.Exists(path))
            {
                return null;
            }

            var message = new HttpResponseMessage();


            message.Headers.Add("photo", sender.base64PhotoString);
            message.Headers.Add("senderUserName", sender.UserName);
            message.Headers.Add("reply", buzz.IsReply.ToString());
            message.Headers.Add("type", buzz.Type);
            message.Headers.Add("serverTime", _helpers.GetJavaCurrentMillis(buzz.ServerTime).ToString());
            message.Headers.Add("serverID", buzz.Id.ToString());
            message.Headers.Add("identity", buzz.BuzzIdentity);
            message.Headers.Add("userFromID", buzz.FromUserID.ToString());
            message.Headers.Add("message", buzz.Message);
            message.Headers.Add("received", (!buzz.ReceivedTime.Equals(new DateTime(2000, 1, 1))).ToString());
            message.Headers.Add("receivedTime", _helpers.GetJavaCurrentMillis(buzz.ReceivedTime).ToString());

            message.Headers.Add("time", _helpers.GetJavaCurrentMillis(buzz.Time).ToString());


            var stream = new System.IO.FileStream(path, System.IO.FileMode.Open);
            message.Content = new StreamContent(stream);
            message.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");


            message.StatusCode = HttpStatusCode.OK;
            
            return result;
        }
        [Route("api/Buzz/GetSocket")]
        public HttpResponseMessage GetSocket()
        {
            if (true)
            {
                System.Web.HttpContext.Current.AcceptWebSocketRequest(ProcessBuzz);
            }
            return new HttpResponseMessage(HttpStatusCode.SwitchingProtocols);


        }
        private async Task ProcessBuzz(AspNetWebSocketContext context)
        {
            System.Net.WebSockets.WebSocket socket = context.WebSocket;

            await Task.Delay(1000);

            return;
        }

        private string SendGCMNotification(string apiKey, string postData, string postDataContentType = "application/json")
        {
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateServerCertificate);

            //
            // MESSAGE CONTENT
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            //
            // CREATE REQUEST
            var webRequest = (HttpWebRequest)WebRequest.Create("https://android.googleapis.com/gcm/send");
            webRequest.Method = "POST";
            webRequest.KeepAlive = false;
            webRequest.ContentType = postDataContentType;
            webRequest.Headers.Add(string.Format("Authorization: key={0}", apiKey));
            webRequest.ContentLength = byteArray.Length;

            var dataStream = webRequest.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            //
            // SEND MESSAGE
            try
            {
                var response = webRequest.GetResponse();
                var responseCode = ((HttpWebResponse)response).StatusCode;
                if (responseCode.Equals(HttpStatusCode.Unauthorized) || responseCode.Equals(HttpStatusCode.Forbidden))
                {
                    var text = "Unauthorized - need new token";
                }
                else if (!responseCode.Equals(HttpStatusCode.OK))
                {
                    var text = "Response from web service isn't OK";
                }

                var reader = new StreamReader(stream: response.GetResponseStream());
                var responseLine = reader.ReadToEnd();
                reader.Close();

                return responseLine;
            }
            catch (Exception e)
            {
            }
            return "error";
        }


        public  bool ValidateServerCertificate(
        object sender,
        X509Certificate certificate,
        X509Chain chain,
        SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        // GET: api/Buzzs
        public IQueryable<Buzz> GetBuzzs()
        {
            return _service.GetBuzzs();
        }

        // GET: api/Buzzs/5
        [ResponseType(typeof(Buzz))]
        public IHttpActionResult GetBuzz(int id)
        {
            Buzz buzz = _service.GetBuzz(id);
            if (buzz == null)
            {
                return NotFound();
            }

            return Ok(buzz);
        }

        

        // POST: api/Buzzs
        [ResponseType(typeof(Buzz))]
        public IHttpActionResult PostBuzz(Buzz buzz)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            _service.AddBuzz(buzz);
            _unit.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = buzz.Id }, buzz);
        }

        

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _unit.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BuzzExists(int id)
        {
            return _service.GetBuzzs().Count(e => e.Id == id) > 0;
        }




    }


}