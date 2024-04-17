using System;
using System.ServiceModel;
using System.Net;

namespace Server
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            string hostName = Dns.GetHostName();
            var iPAddress = Dns.GetHostAddresses(hostName)[0];

            Console.WriteLine("IP address: " + iPAddress);
            Console.WriteLine("Port address: ");

            string port = Console.ReadLine();

            NetTcpBinding binding = new NetTcpBinding();
            Uri baseAddress = new Uri($"net.tcp://localhost:{port}/wcfserver");
            using (ServiceHost serviceHost = new ServiceHost(typeof(WCFServer), baseAddress))
            {
                serviceHost.AddServiceEndpoint(typeof(IWCFServer), binding, baseAddress);
                serviceHost.Open();
                Console.WriteLine($"The WCF server is ready at {baseAddress}.");
                Console.WriteLine("Press <ENTER> to terminate service...");
                Console.WriteLine();
                Console.ReadLine();
            }
        }
    }
}
