namespace Steam_Tool_Console
{
    public class Models
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
            public bool EmailAuthNeeded;
            public string EmailDomain;
            public string EmailSteamId;
        }
        
        public class GetRsaKeyResponse
        {
            public bool Success;
            public string Password;
            public string PublicKeyExp;
            public string PublicKeyMod;
            public string Rsatimestamp;
        }
    }
}