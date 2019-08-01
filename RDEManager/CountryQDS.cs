using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileHelpers;

namespace RDEManager
{
    [DelimitedRecord(",")]
    class CountryQDS
    {
        public CountryQDS()
        {

        }

        public CountryQDS(string qds, string code)
        {
            this.QDS = qds;
            this.CountryCode = code;
        }

        public string QDS { get; set; }
        public string CountryCode { get; set; }
    }
}
