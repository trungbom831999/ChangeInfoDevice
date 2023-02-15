using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinSubTrial
{
    public class Network
    {
        public string code { get; set; }
        public Carrier[] operators { get; set; }
    }
    public class Carrier
    {
        public string code { get; set; }
        public string name { get; set; }
    }

    public class Country
    {
        public string code { get; set; }
        public string name { get; set; }
    }
}
