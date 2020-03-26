using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDEManager
{
    public class SchemaTypeMismatch
    {
        public SchemaTypeMismatch()
        {
            //nothing
        }

        public string columnName {get; set;}
        public string templateType { get; set; }
        public string mismatchedType { get; set; }
    }
}
