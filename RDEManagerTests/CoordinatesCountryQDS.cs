using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDEManagerTests
{
    class CoordinatesCountryQDS
    {
        public decimal lat { get; set; }
        public decimal lng { get; set; }
        public string qds { get; set; }
        public string countryCode { get; set; }
        public string country { get; set; }

        public CoordinatesCountryQDS(string csvLine)
        {
            string[] values = csvLine.Split(',');
            this.lat = Convert.ToDecimal(values[0]);
            this.lng = Convert.ToDecimal(values[1]);
            this.qds = values[2];
            this.countryCode = values[3];
            this.country = values[4];
        }
    }
}
