using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EasyHttp.Http;
using Newtonsoft.Json;
using static Steam_Tool_Console.Models;
using static Steam_Tool_Console.Program;


namespace Steam_Tool_Console
{
    public class SteamApi
    {
        public const string SteamBaseUrl = "https://steamcommunity.com/";
        public const string SteamStoreBaseUrl = "https://store.steampowered.com/";
        public const string SteamGetRsaKeyEndpoint = "login/getrsakey";
        public const string SteamDoLoginEndpoint = "login/dologin";
        public const string SteamCaptchaLoginEndpoint = "login/rendercaptcha/?gid=";
        public const string SteamProfileByIdEndpoint = "https://steamcommunity.com/profiles/";
        
        public static async Task<LoginResponse> LoginSteamAccount(string username, string password)
        {
            // Request Public Key Modulo and Exponent from Steam Server (changed all 60 minutes)
            GetRsaKeyResponse rsaData = GetRsaData(username, password);
            var encPassword = EncryptPasswordFactory.EncryptPassword(rsaData);

            // Try logging in to Server
            var doLogin = DoLogin(username, encPassword, rsaData);
            
            // Check if Login successful and if captcha needed
            

            if (doLogin.captcha_needed)
            {
                rsaData = GetRsaData(username, password);
                encPassword = EncryptPasswordFactory.EncryptPassword(rsaData);
                
                string captcha = GetCaptcha(doLogin.captcha_gid);
                doLogin = DoLogin(username, encPassword, rsaData, captchaText: captcha, captchaGid: doLogin.captcha_gid);
            }
            
            // Check if 2FA needed
            if (doLogin.emailauth_needed)
            {
                rsaData = GetRsaData(username, password);
                encPassword = EncryptPasswordFactory.EncryptPassword(rsaData);

                string emailAuthCode = GetEmailAuth(username, doLogin.emaildomain, doLogin.emailsteamid);
                doLogin = DoLogin(username, encPassword, rsaData, emailAuthCode: emailAuthCode, emailSteamId: doLogin.emailsteamid.ToString());

            } else if (doLogin.requires_twofactor)
            {
                rsaData = GetRsaData(username, password);
                encPassword = EncryptPasswordFactory.EncryptPassword(rsaData);

                string twoFactorCode = GetTwoFactor(username);
                doLogin = DoLogin(username, encPassword, rsaData, twoFactorCode: twoFactorCode);
            }
            
            Console.WriteLine("Login success: " + doLogin.success);
            
            return doLogin;
        }

        private static GetRsaKeyResponse GetRsaData(string username, string password){
            // Send Username to Server and get RSA Data
            var rsaContent = new Dictionary<string, string>()
            {
                {"username", username},
            };

            var rsaRequestResponse = http.Post(SteamBaseUrl + SteamGetRsaKeyEndpoint, rsaContent, HttpContentTypes.ApplicationXWwwFormUrlEncoded);
            
            // Read response, serialize it and create GetRsaKeyResponse Object
            GetRsaKeyResponse responseObject = JsonConvert.DeserializeObject<GetRsaKeyResponse>(rsaRequestResponse.RawText);

            responseObject.Password = password;

            if (!responseObject.success)
            {
                throw new Exception("Error getting RSA Object");
            }

            return responseObject;
        }

        private static LoginResponse DoLogin(string username, string encryptedPassword, GetRsaKeyResponse rsaData, string captchaText = "", string captchaGid = "-1", string emailAuthCode = "", string twoFactorCode = "", string emailSteamId = "")
        {
            var doLoginDict = new Dictionary<string, string>()
            {
                {"username", username},
                {"password", encryptedPassword},
                {"loginfriendlyname", ""},
                {"captchagid", captchaGid},
                {"captcha_text", captchaText},
                {"emailsteamid", emailSteamId},
                {"emailauth", emailAuthCode},
                {"rsatimestamp", rsaData.timestamp},
                {"remember_login", "true"},
                {"twofactorcode", twoFactorCode},
                
                {"donotcache", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()}
            };
            
            // Do Login and Serialize Response Data
            var doLoginResponse = http.Post(SteamBaseUrl + SteamDoLoginEndpoint, doLoginDict, HttpContentTypes.ApplicationXWwwFormUrlEncoded);
            string setCookies = doLoginResponse.RawHeaders["Set-Cookie"];
           
            var doLoginResponseDict = JsonConvert.DeserializeObject<LoginResponse>(doLoginResponse.RawText);
            
            Regex regex = new Regex("steamLoginSecure=.*?;");
            var match = regex.Match(setCookies);
            doLoginResponseDict.steamLoginSecure = match.Value.Replace("steamLoginSecure=", "").Replace(";", "");
            
            return doLoginResponseDict;
        }

        private static string GetCaptcha(string captchaId)
        {
            // Request Captcha from Stem Server and Save it as PNG
            HttpResponse captachaRequest = http.Get(SteamBaseUrl + SteamCaptchaLoginEndpoint + captchaId);
            captachaRequest.GetResponse(http.Request.PrepareRequest(), $"{captchaId}.png", false);
            
            // Get Captcha Text
            Console.WriteLine(SteamBaseUrl + SteamCaptchaLoginEndpoint + captchaId);
            Console.WriteLine("Please enter Captcha:");
            string captchaText = Console.ReadLine();
            
            return captchaText;
        }
        
        public static string GetEmailAuth(string username, string emaildomain, long emailsteamid)
        {
            Console.WriteLine($"Please enter Email Auth Code (Domain: {emaildomain}, Username: {username}):");
            string emailAuth = Console.ReadLine();
            
            return emailAuth;
        }
        
        public static string GetTwoFactor(string username)
        {
            Console.WriteLine($"Please enter Two Factor Code from Mobile Authenticator (Username: {username}):");
            string twoFactor = Console.ReadLine();
            
            return twoFactor;
        }
    }
}