using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EasyHttp.Http;
using Newtonsoft.Json;
using RandomUserAgent;
using HttpClient = EasyHttp.Http.HttpClient;
using static Steam_Tool_Console.EncryptPasswordFactory;
using static Steam_Tool_Console.Models;


namespace Steam_Tool_Console
{
    public class SteamApi
    {
        private static readonly string Useragent = RandomUa.RandomUserAgent;
        private static readonly HttpClient http = new HttpClient {Request = {UserAgent = Useragent, Accept = HttpContentTypes.ApplicationXWwwFormUrlEncoded}};

        private const string SteamBaseUrl = "https://steamcommunity.com/";
        private const string SteamGetRsaKeyEndpoint = "login/getrsakey";
        private const string SteamDoLoginEndpoint = "login/dologin";
        private const string SteamCaptchaLogin = "login/rendercaptcha/?gid=";
        
        
        public static async Task<Dictionary<string, string>> LoginSteamAccount(string username, string password)
        {
            // Request Public Key Modulo and Exponent from Steam Server (changed all 60 minutes)
            GetRsaKeyResponse rsaData = GetRsaData(username, password);
            
            var encPassword = EncryptPassword(rsaData);
            
            // Try logging in to Server
            var doLogin = DoLogin(username, encPassword, rsaData);

            if (doLogin.Success)
            {
                Console.WriteLine("Login successful");
            }
            else if (doLogin.CaptchaNeeded)
            {
                string captcha = GetCaptcha(doLogin.CaptchaGid);
                var captchaDoLogin = DoLogin(username, encPassword, rsaData, captchaText: captcha, captchaGid: doLogin.CaptchaGid);
            }
            else if (doLogin.EmailAuthNeeded)
            {
                
            }

            Console.WriteLine(doLogin.Message);
            
            return new Dictionary<string, string>();
        }

        private static GetRsaKeyResponse GetRsaData(string username, string password){
            // Send Username to Server and get RSA Data
            var rsaContent = new Dictionary<string, string>()
            {
                {"username", username},
            };

            var rsaRequestResponse = http.Post(SteamBaseUrl + SteamGetRsaKeyEndpoint, rsaContent, HttpContentTypes.ApplicationXWwwFormUrlEncoded);
            
            // Read response, serialize it and create GetRsaKeyResponse Object
            Dictionary<string, string> responseObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(rsaRequestResponse.RawText);
            GetRsaKeyResponse rsaResponse = new GetRsaKeyResponse();
            
            if (responseObject["success"] == "True")
            {
                rsaResponse.Success = responseObject["success"] == "True";
                rsaResponse.PublicKeyExp = responseObject["publickey_mod"];
                rsaResponse.PublicKeyMod = responseObject["publickey_exp"];
                rsaResponse.Rsatimestamp = responseObject["timestamp"];
                rsaResponse.Password = password;
            }

            if (!rsaResponse.Success)
            {
                throw new Exception("Error getting RSA Object");
            }

            return rsaResponse;
        }

        private static LoginResponse DoLogin(string username, string encryptedPassword, GetRsaKeyResponse rsaData, string captchaText = "", string captchaGid = "-1")
        {
            var doLoginDict = new Dictionary<string, string>()
            {
                {"username", username},
                {"password", encryptedPassword},
                {"emailauth", ""},
                {"loginfriendlyname", ""},
                {"captchagid", captchaGid},
                {"captcha_text", captchaText},
                {"emailsteamid", ""},
                {"rsatimestamp", rsaData.Rsatimestamp},
                {"remember_login", "True"},
                {"donotcache", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()}
            };
            
            // Do Login and Serialize Response Data
            var doLoginResponse = http.Post(SteamBaseUrl + SteamDoLoginEndpoint, doLoginDict, HttpContentTypes.ApplicationXWwwFormUrlEncoded);
            Dictionary<string, string> doLoginResponseDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(doLoginResponse.RawText);

            TransferParameters transferParameters = new TransferParameters();
            
            // If Login was successful get logged in Data
            if (doLoginResponseDict["success"] == "True")
            {
                transferParameters.SteamId = doLoginResponseDict["steamid"];
                transferParameters.TokenSecure = doLoginResponseDict["token_secure"];
                transferParameters.Auth = doLoginResponseDict["auth"];
                transferParameters.RememberLogin = doLoginResponseDict["remember_login"];
            }
            
            // Get all Response Parameters from Login Response and create Response Object

            
            LoginResponse loginResponse = new LoginResponse()
            {
                Success = doLoginResponseDict["success"] == "True",
            };
            
            if (doLoginResponseDict.ContainsKey("message"))
            {
                loginResponse.Message = doLoginResponseDict["message"];
            }
            
            if (doLoginResponseDict.ContainsKey("requires_twofactor"))
            {
                loginResponse.RequiresTwofactor = doLoginResponseDict["requires_twofactor"] == "True";
            }

            if (doLoginResponseDict.ContainsKey("captcha_needed"))
            {
                loginResponse.CaptchaNeeded = doLoginResponseDict["captcha_needed"] == "True";
            }
            
            if (doLoginResponseDict.ContainsKey("captcha_gid"))
            {
                loginResponse.CaptchaGid = doLoginResponseDict["captcha_gid"];
            }
            
            if (doLoginResponseDict.ContainsKey("login_complete"))
            {
                loginResponse.LoginComplete = doLoginResponseDict["login_complete"];
            }

            if (doLoginResponseDict.ContainsKey("transfer_urls"))
            {
                loginResponse.TransferUrls = doLoginResponseDict["transfer_urls"];
            }
           
            if (doLoginResponseDict.ContainsKey("transfer_parameters"))
            {
                loginResponse.TransferParameters = transferParameters;
            }
            
            if (doLoginResponseDict.ContainsKey("emailauth_needed"))
            {
                loginResponse.EmailAuthNeeded = doLoginResponseDict["emailauth_needed"] == "True";
            }
            
            if (doLoginResponseDict.ContainsKey("emaildomain"))
            {
                loginResponse.EmailDomain = doLoginResponseDict["emaildomain"];
            }
            
            if (doLoginResponseDict.ContainsKey("emailsteamid"))
            {
                loginResponse.EmailSteamId = doLoginResponseDict["emailsteamid"];
            }

            return loginResponse;
        }

        private static string GetCaptcha(string captchaId)
        {
            // Request Captcha from Stem Server and Save it as PNG
            HttpResponse captachaRequest = http.Get(SteamBaseUrl + SteamCaptchaLogin + captchaId);
            captachaRequest.GetResponse(http.Request.PrepareRequest(), $"{captchaId}.png", false);
            
            // Get Captcha Text
            Console.WriteLine("Please enter Captcha:");
            string captchaText = Console.ReadLine();
            
            return captchaText;
        }
        
        public static string GetEmailAuth(string captchaId)
        {
            return "";
        }
    }
}