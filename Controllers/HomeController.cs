using Newtonsoft.Json;
using RestSharp;
using SpotifyApi.NetCore;
using SpotifyApi.NetCore.Authorization;
using SpotifyWebApiCheck.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SpotifyWebApiCheck.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult AuthorizeSpotify()
        {
            string url = Models.SpotifyApi.Authorizeuser();
            Response.Redirect(url, true);
            return Content("ok");
        }

        public ActionResult SpotifyRedirect()
        {
            return View();
        }

        public ActionResult Spofitycall()
        {
            return View();
        }

        public async Task<ActionResult> SpofityDataAsync()
        {
            List<string> tracksname = new List<string>();
            var http = new HttpClient();
            var accounts = new AccountsService(http);

            // Get an artist by Spotify Artist Id
            //var artists = new ArtistsApi(http, accounts);
            //Artist artist = await artists.GetArtist("1tpXaFf2F55E7kVJON4j4G");
            //string artistName = artist.Name;
            //Trace.WriteLine($"Artist.Name = {artistName}");

            //// Get recommendations based on seed Artist Ids
            //var browse = new BrowseApi(http, accounts);
            //RecommendationsResult result = await browse.GetRecommendations(new[] { "1tpXaFf2F55E7kVJON4j4G", "4Z8W4fKeB5YxbusRsdQVPb" }, null, null);
            //string firstTrackName = result.Tracks[0].Name;
            //Trace.WriteLine($"First recommendation = {firstTrackName}");

            // Page through a list of tracks in a Playlist
            var playlists = new PlaylistsApi(http, accounts);
            int limit = 100;
            PlaylistPaged playlist = await playlists.GetTracks("4h4urfIy5cyCdFOc1Ff4iN", limit: limit);
            int offset = 0;
            int j = 0;
            // using System.Linq
            while (playlist.Items.Any())
            {
                for (int i = 0; i < playlist.Items.Length; i++)
                {
                    Trace.WriteLine($"Track #{j += 1}: {playlist.Items[i].Track.Artists[0].Name} / {playlist.Items[i].Track.Name}");
                    tracksname.Add(playlist.Items[i].Track.Artists[0].Name);

                }
                offset += limit;
                playlist = await playlists.GetTracks("4h4urfIy5cyCdFOc1Ff4iN", limit: limit, offset: offset);
            }

            ViewBag.ply = tracksname;
            return View();
        }


        public ActionResult AuthorizeZoom()
        {
            System.Web.HttpCookie zoomtoken = HttpContext.Request.Cookies.Get("zmToken");
            return View();
        }

        public ActionResult ZoomRedirect()
        {
            System.Web.HttpCookie zoomtoken = HttpContext.Request.Cookies.Get("zmToken");
            if(zoomtoken != null)
            {
                return RedirectToAction("ZoomRecording", "Home");
            }
            string authocode = Request.QueryString["code"];
            //Session["zoomcode"] = Request.QueryString["code"];
            //ViewBag.code = authocode;
            if (string.IsNullOrEmpty(authocode))
            {
                string AuthorizeUrl = "https://zoom.us/oauth/authorize?client_id=aKSZf2SRSJC89Fjy2TysYQ&response_type=code&redirect_uri=http://localhost:62914/Home/ZoomRedirect";
                return Redirect(AuthorizeUrl);
            }

            return RedirectToAction("ZoomRedirectToken", "Home", new { Id = 1, code=authocode });
            //return View();
        }

     
        public ActionResult ZoomRedirectToken(int Id,string code)
        {
            string zoomcode = Request.Params["code"];

            string zoomAuthorizeUrl = "https://zoom.us/oauth/token?grant_type=authorization_code&code=";
            string RedirectUrl = "http://localhost:62914/Home/ZoomRedirect";

            var client = new RestClient(zoomAuthorizeUrl + zoomcode + "&redirect_uri="+RedirectUrl);
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", string.Format(Authorize()));
            IRestResponse response = client.Execute(request);
            if(response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                ZoomToken zoomtoken = JsonConvert.DeserializeObject<ZoomToken>(response.Content);
                //ViewBag.zmtoken = zoomtoken.access_token;

                System.Web.HttpCookie cookie = new System.Web.HttpCookie("zmToken", zoomtoken.access_token);
                cookie.Expires = DateTime.Now.AddMinutes(45);
                Response.Cookies.Add(cookie);
                return RedirectToAction("ZoomRedirect", "Home");
            }
            return Content("Some error happpened<br>Please Try Again Or Contact to Admin");
        }


        private string Authorize()
        {
            string ClientId = "aKSZf2SRSJC89Fjy2TysYQ";
            string ClientSecret = "oxe9sxk5nTnuSvKH9NuUp1deZoSIJvWt";
            var plaintextbytes = System.Text.Encoding.UTF8.GetBytes(ClientId + ":" + ClientSecret);
            var encodedstring = System.Convert.ToBase64String(plaintextbytes);
            return "Basic " + encodedstring;
        }

        public ActionResult ZoomRecording()
        {
            System.Web.HttpCookie zoomtoken = HttpContext.Request.Cookies.Get("zmToken");
            
            if(zoomtoken != null)
            {
                string zomtoken = zoomtoken.Value;
                DateTime today = DateTime.Now;
                today = today.AddDays(-30);
                string fromdate = today.ToString("yyyy-MM-dd");
                var client = new RestClient("https://api.zoom.us/v2/users/me/recordings?from="+fromdate);
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("Authorization", "Bearer " + zomtoken);
                IRestResponse response = client.Execute(request);
                if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    System.Web.HttpCookie cookie = HttpContext.Request.Cookies.Get("zmToken");
                    cookie.Expires = DateTime.Now.AddMilliseconds(-1);
                    Response.Cookies.Add(cookie);
                    return RedirectToAction("ZoomRedirect", "Home");
                }
                //ZoomCloudRecording zoomcloud = JsonConvert.DeserializeObject<ZoomCloudRecording>(response.Content);
                string demotext = System.IO.File.ReadAllText(Server.MapPath("~/ZoomData/") +"demodata.json");
                ZoomCloudRecording zoomcloud = JsonConvert.DeserializeObject<ZoomCloudRecording>(demotext);
                return View(zoomcloud);
            }
            return RedirectToAction("ZoomRedirect", "Home");
            
        }
    }
}