using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDEManager
{
    public static class RecordErrors
    {
        public static string collNumberError = "collector number must be a number or s.n.";
        public static string countryInvalid = "country may be invalid";
        public static string majorareaEmpty = "majorarea must be empty";
        public static string minorareaEmpty = "minorarea must be empty";
        public static string localityEmpty = "locality must be empty";
        public static string higherTaxaMissing = "higher taxa are missing for recorded taxon names";
        public static string ranksNotInBackbone = "taxa not in backbone";
        public static string coordinateErrors = "there are errors with coordinates and/or associated fields";
        public static string qdsInvalid = "invalid qds format or value";
        public static string qdsNotValidForCountry = "qds is not valid for country";
        public static string qdsCoordsMismatch = "QDS and coordinates are incongruent";
        public static string collectorsNotInList = "collector/s not in list";
        public static string determinerNotInList = "determiner not in list";

    }
}
