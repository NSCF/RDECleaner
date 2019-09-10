using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace RDEManager
{
    public static class RecordErrorFinder
    {

        //RECORDSET LEVEL CHECKS

        //check all images captured and all records have images
        //returns [imageNotCaptured, recordsWithNoImages]
        //tested manually
        public static List<string>[] checkAllImagesCaptured(DataTable records, List<string> imageFileNames)
        {
            //get the barcode values from the image file names
            List<string> barcodesFromFileNames = imageFileNames.Select(fileName => {
                return fileName.Substring(0, fileName.LastIndexOf('.')).Trim();
            }).ToList();

            barcodesFromFileNames = barcodesFromFileNames.Select(s => s.ToUpper()).Distinct().ToList(); //distinct because there may be cases where the same specimen has more than one image (eg. jpg and tif)

            //get the captured barcodes
            List<string> notCaptured = new List<string>();
            List<string> capturedBarcodes = records.AsEnumerable().Select(x => x["barcode"].ToString().ToUpper().Trim()).ToList();

            notCaptured = barcodesFromFileNames.Where(bf => !capturedBarcodes.Contains(bf)).ToList();

            //get those in the list where we don't have images
            List<string> noImages = capturedBarcodes.Where(bc => !barcodesFromFileNames.Contains(bc)).ToList();

            List<string>[] res = { notCaptured, noImages };

            return res;
        }

        //tested manually
        public static Dictionary<string, List<string>> getTaxonNamesNotInBackbone(DataTable records, DataTable taxonBackbone)
        {
            Dictionary<string, List<string>> taxaNotFound = new Dictionary<string, List<string>>();

            List<DataRow> recordsEnum = records.AsEnumerable().ToList();

            // prepare the captured taxa for checking against the backbone
            var capturedTaxonNames = recordsEnum.Select(rec => new {
                family = rec["family"].ToString().Trim(),
                genus = rec["genus"].ToString().Trim(),
                species = rec["sp1"].ToString().Trim(),
                subspecies = rec["sp2"].ToString().Trim(),
                var = rec["sp3"].ToString().Trim()
            }).Distinct().ToList();

            var capturedFamilies = capturedTaxonNames.Where(rec => !String.IsNullOrEmpty(rec.family)).Select(rec => rec.family).Distinct().ToList();
            var capturedGenera = capturedTaxonNames.Where(rec => !String.IsNullOrEmpty(rec.genus)).Select(rec => rec.genus).Distinct().ToList();
            var capturedSpecies = capturedTaxonNames.Where(rec => !String.IsNullOrEmpty(rec.species) && String.IsNullOrEmpty(rec.subspecies)).Select(rec => new { rec.genus, rec.species }).Distinct().ToList();
            var capturedSubspecies = capturedTaxonNames.Where(rec => !String.IsNullOrEmpty(rec.subspecies) && String.IsNullOrEmpty(rec.var)).Select(rec => new { rec.genus, rec.species, rec.subspecies }).Distinct().ToList();
            var capturedVars = capturedTaxonNames.Where(rec => !String.IsNullOrEmpty(rec.var)).Select(rec => new { rec.genus, rec.species, rec.subspecies, rec.var }).Distinct().ToList();

            var checkTaxonNames = taxonBackbone.AsEnumerable()
                .Where(rec => capturedFamilies.Contains(rec["family"].ToString().Trim()))
                .Select(rec => new {
                    family = rec["family"].ToString().Trim(),
                    genus = rec["genus"].ToString().Trim(),
                    species = rec["sp1"].ToString().Trim(),
                    subspecies = rec["sp2"].ToString().Trim(),
                    var = rec["sp3"].ToString().Trim()
                }).Distinct().ToList();

            List<string> backboneFamilies = checkTaxonNames.Select(rec => rec.family).Distinct().ToList();
            List<string> backboneGenera = checkTaxonNames.Where(rec => capturedFamilies.Contains(rec.family)).Select(rec => rec.genus).Distinct().ToList();
            var backboneSpecies = checkTaxonNames.Where(rec => capturedGenera.Contains(rec.genus) && String.IsNullOrEmpty(rec.subspecies)).Select(rec => new { rec.genus, rec.species }).Distinct().ToList();
            var backboneSubspecies = checkTaxonNames.Where(rec => capturedGenera.Contains(rec.genus) && !String.IsNullOrEmpty(rec.subspecies) && String.IsNullOrEmpty(rec.var)).Select(rec => new { rec.genus, rec.species, rec.subspecies }).Distinct().ToList();
            var backboneVars = checkTaxonNames.Where(rec => capturedGenera.Contains(rec.genus) && !String.IsNullOrEmpty(rec.subspecies) && !String.IsNullOrEmpty(rec.var)).Select(rec => new { rec.genus, rec.species, rec.subspecies, rec.var }).Distinct().ToList();

            List<string> invalidFamilies = capturedFamilies.Where(fam => !backboneFamilies.Contains(fam)).ToList();
            List<string> invalidGenera = capturedGenera.Where(gen => !backboneGenera.Contains(gen)).ToList();
            var invalidSpecies = capturedSpecies.Where(sp => !backboneSpecies.Contains(sp)).ToList();
            var invalidSubspecies = capturedSubspecies.Where(ssp => !backboneSubspecies.Contains(ssp)).ToList();
            var invalidVars = capturedVars.Where(var => !backboneVars.Contains(var)).ToList();

            //make the last ones strings
            List<string> invalidSpeciesNames = invalidSpecies.Select(sp => $"{sp.genus} {sp.species}").ToList();
            List<string> invalidSubspeciesNames = invalidSubspecies.Select(ssp => $"{ssp.genus} {ssp.species} {ssp.subspecies}").ToList();
            List<string> invalidVarNames = invalidVars.Select(var => $"{var.genus} {var.species} {var.subspecies} {var.var}").ToList();

            taxaNotFound.Add("families", invalidFamilies);
            taxaNotFound.Add("genera", invalidGenera);
            taxaNotFound.Add("species", invalidSpeciesNames);
            taxaNotFound.Add("infraSpecific1", invalidSubspeciesNames);
            taxaNotFound.Add("infraSpecific2", invalidVarNames);

            return taxaNotFound;

        }

        //ROW LEVEL CHECKS 

        //barcode is empty
        public static bool barcodeHasValue(DataRow row)
        {
            string barcode = row["barcode"].ToString().Trim();
            return !String.IsNullOrEmpty(barcode);
        }

        //collector number should only be a number or s.n.
        //NOT TESTED
        public static bool numberIsAnIntegerOrSN(DataRow row)
        {
            string recordNumber = row["number"].ToString().Trim();


            if (recordNumber.Replace(" ", "").ToLower() != "s.n.")
            {
                try
                {
                    int.Parse(recordNumber);
                }
                catch //it failes
                {
                    return false;
                }

                //it worked
                return true;
            }
            else
            {
                return true;
            }

        }

        //check countries are in our list for Southern Africa
        //NOT TESTED
        public static bool countryInList(DataRow row, CountryCodes countryCodes)
        {

            string country = row["country"].ToString().Trim();
            if (String.IsNullOrEmpty(country))
            {
                return true; //no test
            }
            else
            {
                return countryCodes.Codes.ContainsValue(country);
            }
            
        }

        //NOT TESTED
        public static bool majorAreaIsEmpty(DataRow row)
        {
            string majorarea = row["majorarea"].ToString().Trim();
            return String.IsNullOrEmpty(majorarea);
        }

        //NOT TESTED
        public static bool minorAreaIsEmpty(DataRow row)
        {
            string minorarea = row["minorarea"].ToString().Trim();
            return String.IsNullOrEmpty(minorarea);
        }

        //NOT TESTED
        public static bool localityIsEmpty(DataRow row)
        {
            string locality = row["gazetteer"].ToString().Trim();
            return String.IsNullOrEmpty(locality);
        }

        //check whether a higher taxon is present if a child taxon is captured
        //NOT TESTED
        public static bool higherTaxaAllPresent(DataRow row)
        {

            //
            string family = row["family"].ToString().Trim();
            string genus = row["genus"].ToString().Trim();
            string species = row["sp1"].ToString().Trim();
            string infrataxon1 = row["sp2"].ToString().Trim();
            string infrataxon2 = row["sp3"].ToString().Trim();

            if (genus.Length > 0)
            {
                if (String.IsNullOrEmpty(family))
                {
                    return false;
                }
            }

            if (species.Length > 0)
            {
                if (String.IsNullOrEmpty(genus))
                {
                    return false;
                }
            }

            if (infrataxon1.Length > 0)
            {
                if (String.IsNullOrEmpty(species))
                {
                    return false;
                }
            }

            if (infrataxon2.Length > 0)
            {
                if (String.IsNullOrEmpty(infrataxon1))
                {
                    return false;
                }
            }

            //else
            return true;

        }

        //get the taxon ranks for this record not in the backbone
        //NOT TESTED
        public static string getRanksNotInBackbone(DataRow row, DataTable taxa)
        {
            var taxonBackbone = taxa.AsEnumerable();

            List<string> ranksNotIn = new List<string>();

            var familyIn = taxonBackbone.Where(bbr => bbr["family"].ToString().Trim() == row["family"].ToString().Trim()).FirstOrDefault();
            if (familyIn == null)
            {
                ranksNotIn.Add("family");
            }

            string genus = row["genus"].ToString().Trim();
            if (!String.IsNullOrEmpty(genus))
            {
                var genusIn = taxonBackbone.Where(bbr => bbr["genus"].ToString().Trim() == row["genus"].ToString().Trim()).FirstOrDefault();
                if (genusIn == null)
                {
                    ranksNotIn.Add("genus");
                }
            }
            
            string species = row["sp1"].ToString().Trim();
            if (!String.IsNullOrEmpty(species))
            {
                var speciesIn = taxonBackbone.Where(bbr =>
                    bbr["genus"].ToString().Trim() == row["genus"].ToString().Trim() &&
                    bbr["sp1"].ToString().Trim() == row["sp1"].ToString().Trim()
                ).FirstOrDefault();

                if (speciesIn == null)
                {
                    ranksNotIn.Add("sp1");
                }
            }

            string subspecies = row["sp2"].ToString().Trim();
            if (!String.IsNullOrEmpty(subspecies))
            {
                var subspeciesIn = taxonBackbone.Where(bbr =>
                    bbr["genus"].ToString().Trim() == row["genus"].ToString().Trim() &&
                    bbr["sp1"].ToString().Trim() == row["sp1"].ToString().Trim() &&
                    bbr["sp2"].ToString().Trim() == row["sp2"].ToString().Trim()
                ).FirstOrDefault();

                if (subspeciesIn == null)
                {
                    ranksNotIn.Add("sp2");
                }
            }

            string variety = row["sp3"].ToString().Trim();
            if (!String.IsNullOrEmpty(variety))
            {
                var varietyIn = taxonBackbone.Where(bbr =>
                    bbr["genus"].ToString().Trim() == row["genus"].ToString().Trim() &&
                    bbr["sp1"].ToString().Trim() == row["sp1"].ToString().Trim() &&
                    bbr["sp2"].ToString().Trim() == row["sp2"].ToString().Trim() &&
                    bbr["sp3"].ToString().Trim() == row["sp3"].ToString().Trim()
                ).FirstOrDefault();

                if (varietyIn == null)
                {
                    ranksNotIn.Add("sp3");
                }
            }

            return String.Join("; ", ranksNotIn.ToArray());

        }

        //for finding duplicate records using accession number
        //NOT TESTED
        //deprecated
        public static int botanicalRecordDuplicates(DataRow row, DataTable records)
        {
            string accnum = row["accession"].ToString().Trim();
            if (String.IsNullOrEmpty(accnum))
            {
                throw new Exception("No accession number for this record");
            }

            int duplicateCount = records.AsEnumerable().Where(dtr => dtr["accession"].ToString().Trim() == accnum).Count();

            return duplicateCount;

        }

        //check QDS is valid for country. Returns true for no QDS or no QDS list for the country
        //NOT TESTED
        public static bool QDSValidForCountry(DataRow row, Dictionary<string, List<string>> countryQDSs)
        {

            //countryCodes is a list of countries and their codes

            string qds = row["qds"].ToString().Trim();
            if (qds.Length > 0 && isQDSValid(row))
            {
                string country = row["country"].ToString().Trim();
                if (String.IsNullOrEmpty(country))
                {
                    return true; //no test
                }
                else
                {
                    if (countryQDSs.Keys.Contains(country))
                    {
                        return countryQDSs[country].Contains(qds);
                    }
                    else
                    {
                        return true; // no test
                    }
                }
                
            } //else
            return true; //no test
        }

        //check QDS is a valid QDS format, ie four numbers and two letters
        //TESTED AND WORKING
        public static bool isQDSValid(DataRow row)
        {
            string qds = row["qds"].ToString().Trim();
            if (String.IsNullOrEmpty(qds))
            {
                return true;
            }
            else
            {

                Regex qdsRx = new Regex(@"^\d{4}[ABCD]{0,2}$");
                if (qdsRx.Match(qds).Success)
                {
                    //check that the degree parts are within range
                    int latpart = int.Parse(qds.Substring(0, 2));
                    if (latpart < 0  || latpart > 35)
                    {
                        return false;
                    }
                    else
                    {
                        int lngpart = int.Parse(qds.Substring(2, 2));
                        if (lngpart < 11 || lngpart > 41)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        //check coords match QDS
        //TESTED AND WORKS
        public static bool coordsMatchQDS(DataRow row)
        {
            string latStr = row["lat"].ToString().Trim();
            string ns = row["ns"].ToString().Trim();
            string lngStr = row["long"].ToString().Trim();
            string ew = row["ew"].ToString().Trim();
            string unit = row["llunit"].ToString().Trim();
            string qds = row["qds"].ToString().Trim();

            if (coordsAreValid(row) && llunitIsValid(unit) && !String.IsNullOrEmpty(qds))
            {
                string decimalCoords = getDecimalCoords(row);
                if (qds == getQDSFromCoords(decimalCoords))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        //check all collectors in master table
        //NOT TESTED, getAgentNamesNotInList() is tested
        public static string getCollectorsNotInList(DataRow row, DataTable masterAgents)
        {
            string collectors = row["collector"].ToString().Trim();
            string additional = row["addcoll"].ToString().Trim();


            if (additional != "")
            {
                collectors += "; " + additional;
            }

            if (String.IsNullOrEmpty(collectors))
            {
                return "";
            }
            else
            {
                return getAgentNamesNotInList(collectors, masterAgents);
            }

            
        }

        //check determiner in master table
        public static bool isDeterminerInList(DataRow row, DataTable masterAgents)
        {
            string detby = row["detby"].ToString().Trim();

            string result = getAgentNamesNotInList(detby, masterAgents);

            return String.IsNullOrEmpty(result);

        }

        //THESE DATES ARE A NIGHTMARE!!
        public static bool collYearIsValid(DataRow row)
        {
            string collYear = row["collyy"].ToString().Trim();
            return yearIsValid(collYear);
        }

        public static bool collMonthIsValid(DataRow row)
        {
            string collMon = row["collmm"].ToString().Trim();
            string collDay = row["colldd"].ToString().Trim();
            return monthIsValid(collMon, collDay);
        }

        public static bool collDayIsValid(DataRow row)
        {
            string collDay = row["colldd"].ToString().Trim();

            if (!String.IsNullOrEmpty(collDay) && collDay != "0")
            {
                if (!collMonthIsValid(row))
                {
                    return false; //day cannot be valid if there is no month
                }
                else
                {
                    try
                    {
                        int day = int.Parse(collDay);
                        int mon = int.Parse(row["collmm"].ToString().Trim());
                        if (collYearIsValid(row))
                        {
                            int year = int.Parse(row["collyy"].ToString().Trim());
                            return dayIsValid(year, mon, day);
                        }
                        else
                        {
                            return dayIsValid(mon, day);
                        }
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
            else
            {
                return true;
            }
        }

        public static bool detYearIsValid(DataRow row)
        {
            string detYear = row["detyy"].ToString().Trim();
            return yearIsValid(detYear);
        }

        public static bool detMonthIsValid(DataRow row)
        {
            string detMon = row["detmm"].ToString().Trim();
            string detDay = row["detdd"].ToString().Trim();
            return monthIsValid(detMon, detDay);

        }

        public static bool detDayIsValid(DataRow row)
        {
            string detDay = row["detdd"].ToString().Trim();

            if (!String.IsNullOrEmpty(detDay) && detDay != "0")
            {
                if (!detMonthIsValid(row))
                {
                    return false; //day cannot be valid if there is no month
                }
                else
                {
                    try
                    {
                        int day = int.Parse(detDay);
                        int mon = int.Parse(row["detmm"].ToString().Trim());
                        if (detYearIsValid(row))
                        {
                            int year = int.Parse(row["detyy"].ToString().Trim());
                            return dayIsValid(year, mon, day);
                        }
                        else
                        {
                            return dayIsValid(mon, day);
                        }
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
            else
            {
                return true;
            }
        }

        public static bool collDateIsValid(DataRow row)
        {
            return collYearIsValid(row) && collMonthIsValid(row) && collDayIsValid(row);
        }

        public static bool detDateIsValid(DataRow row)
        {
            return detYearIsValid(row) && detMonthIsValid(row) && detDayIsValid(row);
        }

        public static bool detDateAfterCollDate(DataRow row)
        {

            if(collDateIsValid(row) && detDateIsValid(row))
            {
                string detDayStr = row["detdd"].ToString().Trim();
                string detMonthStr = row["detmm"].ToString().Trim();
                string detYearStr = row["detyy"].ToString().Trim();

                string collDayStr = row["colldd"].ToString().Trim();
                string collMonthStr = row["collmm"].ToString().Trim();
                string collYearStr = row["collyy"].ToString().Trim();

                //make them all full dates and compare
                int detYear = int.Parse(detYearStr);

                int detMonth;
                if (String.IsNullOrEmpty(detMonthStr))
                {
                    detMonth = 12; //for dets we need to assume the last month of the year if only the year. 
                }
                else
                {
                    detMonth = int.Parse(detMonthStr);
                }

                int detDay;
                if (String.IsNullOrEmpty(detDayStr))
                {
                    detDay = DateTime.DaysInMonth(detYear, detMonth); //for dets we need to assume the last day of the month if no day
                }
                else
                {
                    detDay = int.Parse(detDayStr);
                }

                DateTime detDate = new DateTime(detYear, detMonth, detDay);

                int collYear = int.Parse(collYearStr);

                int collMonth;
                if (String.IsNullOrEmpty(collMonthStr))
                {
                    collMonth = 1; // for collection dates we assume first month of the year if no month
                }
                else
                {
                    collMonth = int.Parse(collMonthStr);
                }

                int collDay;
                if (String.IsNullOrEmpty(collDayStr))
                {
                    collDay = 1; // for collection dates we assume first day of the month if no day
                }
                else
                {
                    collDay = int.Parse(collDayStr);
                }

                DateTime collDate = new DateTime(collYear, collMonth, collDay);

                return detDate >= collDate;

            }
            else
            {
                throw new Exception("coll date or det date invalid");
            }

        }

        //HELPERS, BUT SOME USED ELSEWHERE

        //Are coordinates and associated fields valid
        //TESTED
        public static bool coordsAreValid(DataRow row)
        {
            string errors = getCoordErrors(row);
            return String.IsNullOrEmpty(errors);
        }

        //find errors in lat, long and associated fields
        //TESTED VIA coordsAreValiD()
        public static string getCoordErrors(DataRow row)
        {

            string latStr = row["lat"].ToString().Trim();
            string ns = row["ns"].ToString().Trim();
            string lngStr = row["long"].ToString().Trim();
            string ew = row["ew"].ToString().Trim();
            string unit = row["llunit"].ToString().Trim();
            string qds = row["qds"].ToString().Trim();

            List<string> coordErrors = new List<string>();

            List<string> otherFieldErrors = new List<string>();

            //if we have one, we must have the other
            //I don't think this can happen in Brahms but just in case
            if (latStr != lngStr && (String.IsNullOrEmpty(latStr) || String.IsNullOrEmpty(lngStr)))
            {
                coordErrors.Add("lat and long must both contain values or both be empty");
            }

            //they might both be empty
            if(String.IsNullOrEmpty(latStr) && String.IsNullOrEmpty(lngStr))
            {
                if (!String.IsNullOrEmpty(ns) || !String.IsNullOrEmpty(ew) || !string.IsNullOrEmpty(unit))
                {
                    return "ns, ew, and llunit should be empty if no coords captured";
                }
                else
                {
                    return ""; //no further checking needed
                }
            }

            //one is empty and the other not
            if (String.IsNullOrEmpty(latStr) || String.IsNullOrEmpty(lngStr))
            {

                string msg = "One of lat or long has a value and the other not. It must be both or neither";
                if (!String.IsNullOrEmpty(ns) || !String.IsNullOrEmpty(ew) || !string.IsNullOrEmpty(unit))
                {
                    return $"{msg}{Environment.NewLine}ns, ew, and llunit should be empty if no coords captured";
                }
                else
                {
                    return msg;
                }

            }

            //silent else
            //lat and lng must be numbers represented as strings
            //this gets complicated because we can't carry on if this fails
            double lat;
            double lng;
            bool carryOn = true;
            try
            {
                lat = double.Parse(latStr);
            }
            catch
            {
                coordErrors.Add("lat is not a valid number");
                carryOn = false;
            }

            try
            {
                lng = double.Parse(lngStr);

            }
            catch
            {
                coordErrors.Add("long is not a valid number");
                carryOn = false;
            }

            //check the other fields while we're here
            //unit must be one of DD, DM or DMS
            string[] validUnits = { "DD", "DMS", "DM" };

            if (!validUnits.Contains(unit))
            {
                otherFieldErrors.Add("llunit not valid");
                carryOn = false;
            }

            string[] validNS = { "N", "S" };
            string[] validEW = { "E", "W" };

            //we can carry on despite invalid ns and we values so no need to change carryOn here
            if (!validNS.Contains(ns))
            {
                otherFieldErrors.Add("ns not valid");
            }

            if (!validEW.Contains(ew))
            {
                otherFieldErrors.Add("ew not valid");
            }

            //now we can check the actual coordinates
            if (carryOn)
            {
                //this will work now
                lat = double.Parse(latStr);
                lng = double.Parse(lngStr);


                //one can't be zero and the other a number
                if (lat == 0 || lng == 0)
                {
                    if (lat + lng == 0) //if they are both zeros, then not captured, check the other fields are not captured
                    {
                        if (!String.IsNullOrEmpty(ns) || !String.IsNullOrEmpty(ew) || !string.IsNullOrEmpty(unit))
                        {
                            return "ns, ew, and llunit should be empty if no coords captured";
                        }
                    }
                    else
                    {
                        coordErrors.Add("one of lat or long is missing");
                        coordErrors.AddRange(otherFieldErrors);
                        return String.Join("; ", coordErrors.ToArray());
                    }
                }

                //silent else

                //we can ingore DD here as we just need the degrees part
                char[] separators = { '.' };
                string[] latParts = latStr.Split(separators);
                string[] lngParts = lngStr.Split(separators);

                int latDeg = int.Parse(latParts[0]);
                int lngDeg = int.Parse(lngParts[0]);

                if (latDeg > 90 || latDeg < 0 )
                {
                    coordErrors.Add("lat not between 0 and 90");
                }

                if (lngDeg > 180 || lngDeg < 0)
                {
                    coordErrors.Add("long not between 0 and 180");
                }

                //the M or MS part
                if (unit == "DM") 
                {
                    //add the decimal points
                    string latMin = latParts[1].Insert(2, ".");
                    string lngMin = lngParts[1].Insert(2, ".");

                    double latMinNum = double.Parse(latMin);
                    double lngMinNum = double.Parse(lngMin);

                    if (latMinNum > 60)
                    {
                        coordErrors.Add("lat minutes greater than 60");
                    }

                    if (lngMinNum > 60)
                    {
                        coordErrors.Add("long minutes greater than 60");
                    }

                }
                else if (unit == "DMS")
                {
                    string latMin = latParts[1].Substring(0, 2);
                    string lngMin = lngParts[1].Substring(0, 2);
                    string latSec = latParts[1].Substring(2).Insert(2, ".");
                    string lngSec = latParts[1].Substring(2).Insert(2, ".");

                    int latMinNum = int.Parse(latMin);
                    int lngMinNum = int.Parse(lngMin);

                    double latSecNum = double.Parse(latSec);
                    double lngSecNum = double.Parse(lngSec);

                    if (latMinNum > 60)
                    {
                        coordErrors.Add("lat minutes greater than 60");
                    }

                    if (lngMinNum > 60)
                    {
                        coordErrors.Add("long minutes greater than 60");
                    }

                    if (latSecNum > 60)
                    {
                        coordErrors.Add("lat seconds greater than 60");
                    }

                    if (lngSecNum > 60)
                    {
                        coordErrors.Add("long seconds greater than 60");
                    }

                }

            }


            coordErrors.AddRange(otherFieldErrors);

            return String.Join("; ", coordErrors.ToArray());

        }

        // unit is valid
        //NOT TESTED
        //I think it's now deprecated - see getCoordsErrors()
        public static bool llunitIsValid(string unit)
        {
            //unit must be one of DD, DM or DMS
            string[] validUnits = { "DD", "DMS", "DM" };

            return validUnits.Contains(unit);
        }

        //get decimal coordinates from string
        //INITIAL TEST AND WORKING, WAITING FOR HESTER....
        public static string getDecimalCoords(DataRow row)
        {
            
            if (coordsAreValid(row))
            {

                string latStr = row["lat"].ToString().Trim();
                string ns = row["ns"].ToString().Trim();
                string lngStr = row["long"].ToString().Trim();
                string ew = row["ew"].ToString().Trim();
                string unit = row["llunit"].ToString().Trim();
                string qds = row["qds"].ToString().Trim();

                if (unit == "DD")
                {

                    double lat = double.Parse(latStr);
                    double lng = double.Parse(lngStr);

                    if (ns == "S")
                    {
                        lat = -1 * lat;
                    }

                    if (ew == "W")
                    {
                        lng = -1 * lng;
                    }

                    //we need to use the precision of the longer string
                    string longer = latStr.Length > lngStr.Length ? latStr : lngStr;
                    int precision = longer.Split('.')[1].Length;
                    string precStr = "N" + precision;

                    return $"{lat.ToString(precStr)}, {lng.ToString(precStr)}";

                }
                else //DM or DMS
                {
                    //there is always a decimal
                    char[] separators = { '.' };
                    string[] latParts = latStr.Split(separators);
                    string[] lngParts = lngStr.Split(separators);

                    int latDeg = int.Parse(latParts[0]);
                    int lngDeg = int.Parse(lngParts[0]);

                    //the M or MS part
                    if (unit == "DM") //its one number
                    {
                        //add the decimals
                        string latMin = latParts[1].Insert(2, ".");
                        string lngMin = lngParts[1].Insert(2, ".");

                        double latMinNum = double.Parse(latMin);
                        double lngMinNum = double.Parse(lngMin);

                        double lat = latDeg + latMinNum / 60;
                        double lng = lngDeg + lngMinNum / 60;

                        if (ns == "S")
                        {
                            lat = -1 * lat;
                        }

                        if (ew == "W")
                        {
                            lng = -1 * lng;
                        }

                        //we need to use the precision of the longer string
                        string longer = latMin.Length > lngMin.Length ? latMin : lngMin;
                        int precision = longer.Split('.')[1].Length + 2; //the +2 is the key
                        string precStr = "N" + precision;

                        return $"{lat.ToString(precStr)}, {lng.ToString(precStr)}";

                    }
                    else //it has to be DMS
                    {
                        string latMin = latParts[1].Substring(0, 2);
                        string lngMin = lngParts[1].Substring(0, 2);
                        string latSec = latParts[1].Substring(2).Insert(2, ".");
                        string lngSec = lngParts[1].Substring(2).Insert(2, ".");

                        int latMinNum = int.Parse(latMin);
                        int lngMinNum = int.Parse(lngMin);

                        double latSecNum = double.Parse(latSec);
                        double lngSecNum = double.Parse(lngSec);

                        double lat = latDeg + (latMinNum / 60.0) + (latSecNum / 3600);
                        double lng = lngDeg + (lngMinNum / 60.0) + (lngSecNum / 3600);

                        if (ns == "S")
                        {
                            lat = -1 * lat;
                        }

                        if (ew == "W")
                        {
                            lng = -1 * lng;
                        }

                        string longer = latSec.Length > lngSec.Length ? latSec : lngSec;
                        int precision = longer.Split('.')[1].Length + 4; //the +4 is the key
                        string precStr = "N" + precision;

                        return $"{lat.ToString(precStr)}, {lng.ToString(precStr)}";

                    }
                }
            }
            else
            {
                throw new Exception("Invalid coordinates");
            }

            
        }

        //get QDS from coordinates
        //TESTED IN coordsMatchQDS(), WORKING
        public static string getQDSFromCoords(string decimalCoords) //note this works for southern Africa only
        {
            char[] separators = { ',' };
            string[] coords = decimalCoords.Split(separators);
            string latStr = coords[0];
            string lngStr = coords[1];
            double lat = double.Parse(latStr);
            double lng = double.Parse(lngStr);

            int latWholePart = (int)Math.Truncate(lat);
            int longWholePart = (int)Math.Truncate(lng);

            string QDS = $"{latWholePart}{longWholePart}";

            double latDecPart = lat - latWholePart;
            double longDecPart = lng - longWholePart;

            //working out these letters. Let's do this in rows and columns...
            if (latDecPart < 0.25)
            {
                if (longDecPart < 0.25)
                {
                    QDS += "AA";
                }
                else if (longDecPart < 0.5)
                {
                    QDS += "AB";
                }
                else if (longDecPart < 0.75)
                {
                    QDS += "BA";
                }
                else //0.75 - 0.99
                {
                    QDS += "BB";
                }

            }
            else if (latDecPart < 0.5)
            {
                if (longDecPart < 0.25)
                {
                    QDS += "AC";
                }
                else if (longDecPart < 0.5)
                {
                    QDS += "AD";
                }
                else if (longDecPart < 0.75)
                {
                    QDS += "BC";
                }
                else //0.75 - 0.99
                {
                    QDS += "BD";
                }
            }
            else if (latDecPart < 0.75)
            {
                if (longDecPart < 0.25)
                {
                    QDS += "CA";
                }
                else if (longDecPart < 0.5)
                {
                    QDS += "CB";
                }
                else if (longDecPart < 0.75)
                {
                    QDS += "DA";
                }
                else //0.75 - 0.99
                {
                    QDS += "DB";
                }
            }
            else // 0.75 - .099
            {
                if (longDecPart < 0.25)
                {
                    QDS += "CC";
                }
                else if (longDecPart < 0.5)
                {
                    QDS += "CD";
                }
                else if (longDecPart < 0.75)
                {
                    QDS += "DC";
                }
                else //0.75 - 0.99
                {
                    QDS += "DD";
                }
            }

            return QDS;
        }

        //check if agents names are valid and return those that are not
        //TESTED and working
        public static string getAgentNamesNotInList(string agentsString, DataTable masterAgents)
        {

            if(agentsString == "") //sometimes we get this
            {
                return "";
            }

            var masterAgentsEnum = masterAgents.AsEnumerable();

            //split the agents and remove periods from initials
            char[] agentSeparator = { ';' };
            List<string> agentsStringList = agentsString.Split(agentSeparator).Select(a => a.Trim()).ToList();

            List<string> agentsNotMatched = new List<string>();
            //create a list of agents
            foreach(string agent in agentsStringList)
            {

                int matchingAgentCount = 0;
                string initials = "";

                //initials only
                if (StringIsAllUpper(agent))
                {

                    initials = agent.Replace(".", "").Replace(" ", "").Trim(); //replace periods and whitespace
                    matchingAgentCount = masterAgentsEnum.Where(row => row["surname"].ToString().Trim() == "" && row["initials"].ToString().Trim() == agent).Count();

                    if (matchingAgentCount < 1) //we must have at least one
                    {
                        agentsNotMatched.Add(agent);
                    }

                    continue;

                }
                
                //surname only (we assume a comma to separate
                if (!agent.Contains(","))
                {
                    matchingAgentCount = masterAgentsEnum.Where(row => row["surname"].ToString().Trim() == agent && row["initials"].ToString().Trim() == "").Count();
                    if (matchingAgentCount < 1) //we must have at least one
                    {
                        agentsNotMatched.Add(agent);
                    }

                    continue;
                }

                //both

                char[] sep = { ',' };
                string[] parts = agent.Split(sep);
                string lastName = parts[0].Trim();

                initials = parts[1].Replace(".", "").Replace(" ", "").Trim(); //replace periods and whitespace

                matchingAgentCount = masterAgentsEnum.Where(row => row["surname"].ToString().Trim() == lastName && row["initials"].ToString().Trim() == initials).Count();

                if (matchingAgentCount < 1) //we must have at least one
                {
                    agentsNotMatched.Add(agent);
                }
            }


            return String.Join("; ", agentsNotMatched.ToArray());

        }

        public static bool yearIsValid(string yearStr)
        {

            yearStr = yearStr.Trim();

            if(String.IsNullOrEmpty(yearStr))
            {
                return false;
            }
            else
            {
                try
                {
                    int year = int.Parse(yearStr);
                    if (year > 1850 && year <= DateTime.Now.Year)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }
            }
            
        }

        public static bool monthIsValid(string monStr, string dayStr)
        {
            if (!String.IsNullOrEmpty(monStr) && monStr != "0")
            {
                try
                {
                    int mon = int.Parse(monStr);
                    if (mon > 0 && mon <= 12)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }
            }
            else //we need to check if we have a day an no month, in which case month is not valid
            {
                if (!String.IsNullOrEmpty(dayStr) && dayStr != "0")
                {
                    try
                    {
                        int.Parse(dayStr);
                        return false;
                    }
                    catch
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
        }

        public static bool dayIsValid(int year, int mon, int day)
        {
            return day > 0 && day <= DateTime.DaysInMonth(year, mon);
        }

        public static bool dayIsValid(int mon, int day)
        {
            List<int> ThirtyOneDayMonths = new List<int>(new int[] { 1, 3, 5, 7, 8, 10, 12 });
            int maxDays = 0;

            if (mon == 2)
            {
                maxDays = 29;
            }
            else if (ThirtyOneDayMonths.Contains(mon))
            {
                maxDays = 31;
            }
            else
            {
                maxDays = 30;
            }

            return day > 0 && day <= maxDays;
        }

        //helpers
        private static bool StringIsAllUpper(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (Char.IsLetter(input[i]) && !Char.IsUpper(input[i]))
                    return false;
            }
            return true;
        }
    }
}
