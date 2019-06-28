using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDEManager
{
    public class RDETrackingRecord
    {

        public RDETrackingRecord(string start, string herb, string file, string person, string status)
        {
            this.StartDate = start;
            this.Herbarium = herb;
            this.RDEFileName = file;
            this.Capturer = person;
            this.Status = status;

        }

        public string StartDate { get; set; }
        public string Herbarium { get; set; }
        public string RDEFileName { get; set; }
        public string Capturer { get; set; }
        public string Status { get; set; }

        private string startBarcode { get; set; }
        private string endBarcode { get; set; }

    }
}
