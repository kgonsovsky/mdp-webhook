using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventGrid
{
    public class MdpSettings
    {
        public class Topic
        {
            public string EndPoint { get; set;}
            public string Secret { get; set; }
            public string ResourceName { get; set; }
            public string Transform { get; set; }

        }
        public Topic[] Topics{ get; set; }
    }
}
