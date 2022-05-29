using Application;
using WebSocketSharp.Server;

namespace Stenography_Console
{
    public class Program
    {
        private static HelperFunctions hf = new HelperFunctions();
        private static WebSocketFunctions wsf = new WebSocketFunctions(hf);
        private static WebSocketServer wssv;
        public static void Main(string[] args)
        {
            wssv = wsf.getWssv();
            wsf.WebSocketServicesCreate(wssv); 
            wssv.Start();
            Console.WriteLine("ws://192.168.43.11:7286 running");
            Console.ReadKey(true); 
            wssv.Stop();
        }
    }
}