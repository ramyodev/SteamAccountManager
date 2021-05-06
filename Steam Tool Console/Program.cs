using System;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using RandomUserAgent;
using HttpClient = EasyHttp.Http.HttpClient;
using EasyHttp.Http;
using static Steam_Tool_Console.SteamAPI.SteamUserData;


namespace Steam_Tool_Console
{
    internal class Program
    {
        private static readonly string Useragent = RandomUa.RandomUserAgent;
        public static readonly HttpClient http = new HttpClient {Request = {UserAgent = Useragent, Accept = HttpContentTypes.ApplicationXWwwFormUrlEncoded}};
        
        public static async Task Main(string[] args)
        {
            
            Console.SetIn(new StreamReader(Console.OpenStandardInput(8192)));
            
            Models.LoginResponse loginResponse = await SteamApi.LoginSteamAccount("memptvfzdkj4torg7wz", "11358S4E43A8942");
            Models.BasicUserData userData = GetBasicUserData(loginResponse);
            
            Console.WriteLine("--------------------------------------------");
        }
    }
}