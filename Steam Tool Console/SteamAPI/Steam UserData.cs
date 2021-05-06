using System;
using System.Collections.Generic;
using static Steam_Tool_Console.Models;
using EasyHttp.Http;
using static Steam_Tool_Console.Program;
using static Steam_Tool_Console.SteamApi;


namespace Steam_Tool_Console.SteamAPI
{
    public class SteamUserData
    {
        public static BasicUserData GetBasicUserData(LoginResponse loginData)
        {
            Dictionary<string, string> transferData = new Dictionary<string, string>()
            {
                {"steamLoginSecure", ""}
            };
            
            HttpResponse shopSiteRequest = http.Post("https://help.steampowered.com/login/transfer", transferData, HttpContentTypes.ApplicationXWwwFormUrlEncoded);

            // Console.WriteLine("Komme");
            // Console.WriteLine(shopSiteRequest.RawText);
            // Console.WriteLine(shopSiteRequest.RawHeaders);
            
            return new BasicUserData();
        }
    }
}