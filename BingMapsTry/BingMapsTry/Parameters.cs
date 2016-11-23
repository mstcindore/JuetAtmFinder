using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BingMapsTry
{

    public class Parameters
    {
        public Entry[] Entries { get; set; }
        public string Response { get; set; }
    }

    public class Entry
    {
        public float latitude { get; set; }
        public float longitude { get; set; }
        public string name { get; set; }
        public string vicinity { get; set; }
    }

}
