using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EasyHttp.Http;
using HttpClient = EasyHttp.Http.HttpClient;
using Newtonsoft.Json;
using static Zemo_Steam.EncryptPasswordFactory;
using RandomUserAgent;


namespace Zemo_Steam
{

    public class TransferParameters
    {
        public string SteamId;
        public string TokenSecure;
        public string Auth;
        public string RememberLogin;
    }
    
    public class LoginResponse
    {
        public bool Success;
        public string Message;
        public bool RequiresTwofactor;
        public bool CaptchaNeeded;
        public string CaptchaGid;
        public string LoginComplete;
        public string TransferUrls;
        public TransferParameters TransferParameters;
    }
    
    public class SteamApi
    {
        private static readonly string Useragent = RandomUa.RandomUserAgent;
        private static readonly HttpClient http = new HttpClient {Request = {UserAgent = Useragent, Accept = HttpContentTypes.ApplicationXWwwFormUrlEncoded}};

        private const string SteamBaseUrl = "https://steamcommunity.com/";
        private const string SteamLoginEndpoint = "login";
        private const string SteamGetRsaKeyEndpoint = "login/getrsakey";
        private const string SteamDoLoginEndpoint = "login/dologin";
        private const string SteamCaptchaLogin = "/login/rendercaptcha/?gid=";

        public static async Task<Dictionary<string, string>> LoginSteamAccount(string username, string password)
        {            
            //string createSessionRequestContent = http.Get(SteamBaseUrl + SteamLoginEndpoint).RawText;

            // Search for G_ID in Source Code
            // Regex gSessionIdRx = new Regex(@"g_sessionID = '........................'".Replace("'", "\""), RegexOptions.Compiled | RegexOptions.IgnoreCase);

            // Find matches.
            // MatchCollection createSessionMatches = gSessionIdRx.Matches(createSessionRequestContent);

            // if (createSessionMatches.Count != 1)
            // {
            //     throw new Exception("Error getting G_SessionID");
            // }

            // Fetching G_SessionID from Source
            // string gSessionId = createSessionMatches[0].ToString();

            // gSessionId = gSessionId.Substring(gSessionId.IndexOf("\"", StringComparison.Ordinal) + 1);
            // gSessionId = gSessionId.Substring(0, length: gSessionId.Length - 1);

            GetRsaKeyResponse rsaData = GetRsaData(username, password);

            var encPassword = EncryptPassword(rsaData);

            Console.WriteLine(encPassword);
            
            return new Dictionary<string, string>();
        }

        private static GetRsaKeyResponse GetRsaData(string username, string password){
            var rsaContent = new Dictionary<string, string>()
            {
                {"username", username},
            };

            var rsaRequestResponse = http.Post(SteamBaseUrl + SteamGetRsaKeyEndpoint, rsaContent, HttpContentTypes.ApplicationXWwwFormUrlEncoded);
            
            Dictionary<string, string> responseObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(rsaRequestResponse.RawText);
            GetRsaKeyResponse rsaResponse = new GetRsaKeyResponse
            {
                Success = responseObject["success"] == "True",
                PublicKeyExp = responseObject["publickey_mod"],
                PublicKeyMod = responseObject["publickey_exp"],
                Rsatimestamp = responseObject["rsatimestamp"],
                Password = password
            };
            
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
            
            var doLoginResponse = http.Post(SteamBaseUrl + SteamDoLoginEndpoint, doLoginDict, HttpContentTypes.ApplicationXWwwFormUrlEncoded);
            Dictionary<string, string> doLoginResponseDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(doLoginResponse.RawText);

            TransferParameters transferParameters = new TransferParameters();

            if (doLoginResponseDict["success"] == "True")
            {
                transferParameters.SteamId = doLoginResponseDict["steamid"];
                transferParameters.TokenSecure = doLoginResponseDict["token_secure"];
                transferParameters.Auth = doLoginResponseDict["auth"];
                transferParameters.RememberLogin = doLoginResponseDict["remember_login"];
            }

            LoginResponse loginResponse = new LoginResponse()
            {
                Success = doLoginResponseDict["success"] == "True",
            };

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
            
            return loginResponse;
        }
        
    }
}