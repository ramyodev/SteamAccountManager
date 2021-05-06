using System;
using System.Collections.Generic;

namespace Steam_Tool_Console
{
    public class Models
    {
        public class TransferParameters
        {
            public string steamid { get; set; }
            public string token_secure { get; set; }
            public string auth { get; set; }
            public bool remember_login { get; set; }
            public string webcookie { get; set; }
        }
    
        public class LoginResponse
        {
            public bool success { get; set; }
            public bool emailauth_needed { get; set; }
            public bool requires_twofactor { get; set; }
            public bool captcha_needed{ get; set; }

            public long emailsteamid { get; set; }
            public string message { get; set; }
            public string captcha_gid { get; set; }
            public string login_complete { get; set; }
            public IList<string> transfer_urls { get; set; }
            public TransferParameters transfer_parameters { get; set; }
            public string emaildomain { get; set; }
            public string steamLoginSecure { get; set; }
        }
        
        public class GetRsaKeyResponse
        {
            public bool success { get; set; }
            public string Password { get; set; }
            public string publickey_exp { get; set; }
            public string publickey_mod { get; set; }
            public string timestamp { get; set; }
        }
        
        public class BasicUserData
        {
            public int SteamLevel { get; set; }
            public string ProfilePicture { get; set; }
            public int CsGoHours { get; set; }
            public DateTime CreatedOn { get; set; }
            public string VanityUrl { get; set; }
            public string AccountID { get; set; }
            public string SteamID { get; set; }
            public string Steam2ID { get; set; }
            public string Steam3ID { get; set; }
            
            public bool CsGoGameBan { get; set; }
            public bool CommunityBan { get; set; }
            public bool TradeBan { get; set; }
            public bool VacBan { get; set; }
        }
    }
}