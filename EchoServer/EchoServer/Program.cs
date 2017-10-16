using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Serialization;

namespace EchoServer
{
    class Program
    {
        static void Main(string[] args)
        {
            new EchoSrv();
        }

        
    }
}
