using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDEManager
{
    public static class RecordErrors
    {
        public static string noBarcode = "barcode must be present";
        public static string invalidBarcode = "invalid barcode format";
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
        public static string collectionYearInvalid = "collection year is not valid";
        public static string collectionMonthInvalid = "collection month is not valid";
        public static string collectionDayInvalid = "collection day is not valid";
        public static string detYearInvalid = "det year is not valid";
        public static string detMonthInvalid = "det month is not valid";
        public static string detDayInvalid = "det day is not valid";
        public static string collectionDateInvalid = "collection date is not valid";
        public static string detDateInvalid = "det date is not valid";
        public static string detDateBeforeCollectionDate = "det date is before collection date";


    }
}
