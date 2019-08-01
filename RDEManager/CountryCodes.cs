using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDEManager
{
    public class CountryCodes
    {
        public CountryCodes()
        {

            this.Codes = new Dictionary<string, string>();

            this.Codes.Add("ANG", "Angola");
            this.Codes.Add("BOT", "Botswana");
            this.Codes.Add("LES", "Lesotho");
            this.Codes.Add("MAA", "Malawi");
            this.Codes.Add("MOZ", "Mozambique");
            this.Codes.Add("NAM", "Namibia");
            this.Codes.Add("SOU", "South Africa");
            this.Codes.Add("SWA", "Swaziland");
            this.Codes.Add("TAN", "Tanzania");
            this.Codes.Add("ZAM", "Zambia");
            this.Codes.Add("ZIM", "Zimbabwe");
        }

        public Dictionary<string, string> Codes { get; set; }
    }
}
