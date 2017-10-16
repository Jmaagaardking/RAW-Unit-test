using System;
using System.Collections.Generic;
using System.Text;

namespace EchoServer
{
    /// <summary>
    /// Request wrapper for the requests sent from client
    /// </summary>
    class Request
    {
        public string Method { get; set; }
        public string Path { get; set; }
        public string Date { get; set; }
        public string Body { get; set; }
    }

}
