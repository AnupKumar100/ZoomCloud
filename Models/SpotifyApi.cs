using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace SpotifyWebApiCheck.Models
{
    public class SpotifyApi
    {
        public static string GenerateAuthorizationUri()
        {
            StringBuilder ulrBuilder = new StringBuilder(SpotifyConstant.Oauth2uri);
            //ulrBuilder.Append("&client_secret=" + GoogleConstant.ClientSecret);
            ulrBuilder.Append("&response_type=" + SpotifyConstant.Response_type);
            ulrBuilder.Append("&redirect_uri=" + SpotifyConstant.RedirectUri);
            //ulrBuilder.Append("&scope=" + SpotifyConstant.Scopes);
            ulrBuilder.Append("&client_id=" + SpotifyConstant.ClientId);
            ulrBuilder.Append("&state=" + SpotifyConstant.State);
            return ulrBuilder.ToString();
        }

        public static string Authorizeuser()
        {
            return GenerateAuthorizationUri();

        }
    }


    public class SpotifyConstant
    {
        public static string ApplicationName = "GMRTranscription";
        public static string ClientId = "90ab3857adb74b11912742c3ac0acb93";
        public static string ClientSecret = "e708287585b843c58a9f884917a19354";
        public static string RedirectUri = "http://localhost:62914/Home/SpotifyRedirect";
        public static string Oauth2uri = "https://accounts.spotify.com/authorize?";
        public static string Response_type = "token";
        public static string Scopes = "user-read-private user-read-email";
        public static string State = "aek62a";

    }
}