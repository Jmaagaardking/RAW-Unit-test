using System;
using System.Collections.Generic;
using System.Text;

namespace EchoServer
{
    class Response
    {
        /// <summary>
        /// Response wrapper for the responses sent to client
        /// </summary>
        public string Status { get; set; }
        public string Body { get; set; }
    }
}
