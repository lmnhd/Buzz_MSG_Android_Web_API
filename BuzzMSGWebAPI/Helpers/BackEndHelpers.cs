using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Http;
using System.Web.WebSockets;
using System.Web.Http.Hosting;
using System.Threading.Tasks;
using BuzzMSGEntity.Models;
using LemsUTools;
using Newtonsoft.Json;

using System.IdentityModel.Tokens;



namespace BuzzMSGWebAPI.Helpers
{
    public class BackEndHelpers
    {

          public  void DeleteTempDir(string dir)
        {
            try
            {
                if (System.IO.Directory.Exists(dir))
                {
                    System.IO.Directory.Delete(dir, true);
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.InnerException);
            }


        }

        public  List<string> MapTempLocation(string title, string filename = "")
        {
            var tempname = title + DateTime.Now.Ticks.ToString() + @"\";
            var parentDir = tempname;
            if (filename != "")
            {
                if (filename.Contains("."))
                {
                    tempname += filename;
                }

            }
            var result = System.Web.Hosting.HostingEnvironment.MapPath(@"~\misc\" + tempname);

            if (result.Contains("."))
            {
                if (System.IO.File.Exists(result))
                {
                    System.IO.File.Delete(result);
                }
                if (!System.IO.Directory.Exists(System.IO.Directory.GetParent(result).FullName))
                {
                    System.IO.Directory.CreateDirectory(System.IO.Directory.GetParent(result).FullName);
                }
            }
            else
            {
                if (!System.IO.Directory.Exists(result))
                {
                    System.IO.Directory.CreateDirectory(result);
                }
            }
            var thisServerPath = System.Web.Hosting.HostingEnvironment.MapPath(@"~\misc\" + tempname);
            var ftpPath = @"/httpdocs/misc/" + tempname;
            if (System.IO.Directory.Exists(@"C:\Users\BricklyfeA"))
            {
                ftpPath = @"/misc/" + tempname;
            }

            return new List<string>{
                System.Web.Hosting.HostingEnvironment.MapPath(@"~\misc\" + tempname),
                ftpPath,
                System.Web.Hosting.HostingEnvironment.MapPath(@"~\misc\" + parentDir)
            };
        }
        public  string BuzzHelperGetVoiceTempLocation()
        {
            return GetServerTempFileOrPath("BuzzVoiceDownload")[0];
        }
        public string BuzzHelperMoveVoiceFileToLocation(Buzz bz, string filePath)
        {
            string root = HttpContext.Current.Server.MapPath(string.Format(@"~\Buzz\{0}\", bz.FromUserID));
            if (!System.IO.Directory.Exists(root))
            {
                System.IO.Directory.CreateDirectory(root);
            }

            string path = root + bz.BuzzIdentity + ".amr";





            System.IO.File.Move(filePath, path);

            return path;
        }
        public List<string> GetServerTempFileOrPath(string title, string filename = "")
        {
            var result = MapTempLocation(title, filename);

            return result;

        }
        public Tuple<string, string> GetBase64StringFromPhotoUrl(string url, string userName, string photoName)
        {
            var result = "";
            var objWebClient = new System.Net.WebClient();
            var tempPath = string.Format(@"{0}\{1}{2}", userName, photoName, Path.GetExtension(url));
            var pathList = GetServerTempFileOrPath("buzz_user_image_temp", tempPath);
            string finalPath = pathList[0];

            objWebClient.DownloadFile(url, finalPath);
            var imgPhoto = System.Drawing.Image.FromFile(finalPath);

            var pc = new PhotoCenter();

            result = Convert.ToBase64String(pc.GetBytesFromImage(imgPhoto));

            imgPhoto.Dispose();


            return new Tuple<string, string>(result, pathList[2]);
        }

        public DateTime GetDateTimeFromJavaCurrentMillis(string currentMilliString)
        {
            if (currentMilliString.Contains("-"))
            {
                currentMilliString = currentMilliString.Split('-').ElementAt(1);
            }
            long millis = long.Parse(currentMilliString);
            if (millis > 0)
            {
                DateTime dt = new DateTime(1970, 1, 1);
                long tics = millis * TimeSpan.TicksPerMillisecond;

                TimeSpan ts = new TimeSpan(tics);

                var result = dt.Add(ts);

                return result;
            }
            return DateTime.Now;
        }

        public long GetJavaCurrentMillis(DateTime datetime)
        {
            TimeSpan ts = datetime.Subtract(new DateTime(1970, 1, 1));

            return ts.Ticks / TimeSpan.TicksPerMillisecond;



        }

    }

    
    public static class HttpRequestMessageExtensions
    {

        /// <summary>
        /// Returns a dictionary of QueryStrings that's easier to work with 
        /// than GetQueryNameValuePairs KevValuePairs collection.
        /// 
        /// If you need to pull a few single values use GetQueryString instead.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetQueryStrings(this HttpRequestMessage request)
        {
            return request.GetQueryNameValuePairs()
                          .ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Returns an individual querystring value
        /// </summary>
        /// <param name="request"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetQueryString(this HttpRequestMessage request, string key)
        {
            // IEnumerable<KeyValuePair<string,string>> - right!
            var queryStrings = request.GetQueryNameValuePairs();
            if (queryStrings == null)
                return null;

            var match = queryStrings.FirstOrDefault(kv => System.String.Compare(kv.Key, key, System.StringComparison.OrdinalIgnoreCase) == 0);
            return string.IsNullOrEmpty(match.Value) ? null : match.Value;
        }

        /// <summary>
        /// Returns an individual HTTP Header value
        /// </summary>
        /// <param name="request"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetHeader(this HttpRequestMessage request, string key)
        {
            IEnumerable<string> keys = null;
            if (!request.Headers.TryGetValues(key, out keys))
                return null;

            return keys.First();
        }

        /// <summary>
        /// Retrieves an individual cookie from the cookies collection
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cookieName"></param>
        /// <returns></returns>
        public static string GetCookie(this HttpRequestMessage request, string cookieName)
        {
            var cookie = request.Headers.GetCookies(cookieName).FirstOrDefault();
            return cookie != null ? cookie[cookieName].Value : null;
        }
    }
}