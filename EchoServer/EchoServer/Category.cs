using System;
using System.Collections.Generic;
using System.Text;

namespace EchoServer
{
    class Category
    {
        public Category(int cid, string name)
        {
            this.cid = cid;
            this.name = name;
        }

        public int cid { get; set; }

        public string name { get; set; }
    }
}
