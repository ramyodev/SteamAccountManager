using System;
using System.IO;
using System.Threading.Tasks;
using Zemo_Steam;


namespace Steam_Tool_Console
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            Console.SetIn(new StreamReader(Console.OpenStandardInput(8192)));
            await SteamApi.LoginSteamAccount("memptvfzdkj4torg7wz", "6666666");
            Console.WriteLine("--------------------------------------------");
        }
    }
}