using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDEManager
{
    public class BotanicalRecordDuplicateTracker
    {

        public BotanicalRecordDuplicateTracker()
        {
            //do nothing
        }

        public BotanicalRecordDuplicateTracker(string root, List<string> barcodes)
        {
            this.root = root;
            this.dupBarcodes = barcodes;
            this.processed = false;

            this.dupBarcodes.Sort();
        }

        public string root { get; set; }

        public List<string> dupBarcodes { get; set; }

        public bool processed { get; set; }
    }
}
