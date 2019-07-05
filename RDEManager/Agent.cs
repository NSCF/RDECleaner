using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDEManager
{
    public class Agent
    {
        public Agent(string last, string initials)
        {
            this.lastName = last;
            this.initials = initials;
        }

        public string lastName { get; set; }
        public string initials { get; set; }
    }
}
