using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RDEManager
{
    public class BarcodeParts
    {
        public BarcodeParts(string barcode)
        {
            barcode = barcode.Trim();

            this.number = Regex.Match(barcode, @"\d+").Value;

            int numberIndex = barcode.IndexOf(number);

            this.collectionCode = barcode.Substring(0, numberIndex);

            char[] dashes = { '-', '–', '—' };

            if (barcode.IndexOfAny(dashes) >  -1)
            {
                string suffix = barcode.Split(dashes)[1]; // assumes theres more before the dash
                this.suffix = suffix;
            }
            else //it may be a, b, etc
            {
                char last = barcode[barcode.Length - 1];
                if (char.IsLetter(last))
                {
                    this.suffix = last.ToString();
                }
            }
        }

        public string collectionCode { get; set; }

        public string number { get; set; }

        public string suffix { get; set; }
    }
}
