using System;
using System.IO;
using System.Threading.Tasks;


namespace Steam_Tool_Console
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            Console.SetIn(new StreamReader(Console.OpenStandardInput(8192)));

            await SteamApi.LoginSteamAccount("memptvfzdkj4torg7wz", "11358S4E43A8942");
 
            // string mod = "9fc699a3429909a15a4009bb19e1b5c806487a2e3f5e6a6ac7a69a5770136242468a9b0b6bc37c7a1cd5ba8c8efc80507918384796d78db87371fd98477fe1d4a9e2e48917de649128d285171cb267a9b39c4817d0b6be5dea8214c6432fec37a2e69e5c5013a627c3f2017d0fa75945841e7d868c6acfbe1b9828f363da15d6b32fae3dd7fb087318004d2caa9d1924eef13fabb19931d66843c0e1107b6b15b224955171ed93a4d972dbb983a5faa5aac9dc48fa0259ca3217303dc241b4294653cd342162c4629bf39c4863635f1658ca6337738a89c75526e90919b14de0c8aac806b16738924580110066675461df88056da7d81610a65725c270621b1b";
            // string exp = "010001";
            // string password = "11358S4E43A8942";
            // string timestamp = "266513400000";
            
            // GetRsaKeyResponse rsa = new GetRsaKeyResponse(){PublicKeyMod = mod, PublicKeyExp = exp, Password = password, Rsatimestamp = timestamp};
            // string pass = EncryptPasswordFactory.EncryptPassword(rsa);
            
            // Console.WriteLine(pass);
            
            Console.WriteLine("--------------------------------------------");
        }
    }
}