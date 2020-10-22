using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Server
    {
        static void Main(string[] args)
        {
            RemotingConfiguration.Configure("server.exe.config", false);
            Console.WriteLine("Waiting...");
            Console.ReadLine();
        }
    }
}
