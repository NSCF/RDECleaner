using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDEManager
{
    class SchemaDifferences
    {

        public SchemaDifferences()
        {

        }

        public List<string> extraColumns { get; set; }
        public List<string> missingColumns { get; set; }
        public List<SchemaTypeMismatch> typeMismatches { get; set; }
    }
}
