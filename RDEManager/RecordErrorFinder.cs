using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDEManager
{
    public static class RecordErrorFinder
    {

        //RECORDSET LEVEL CHECKS

        //check all images captured and all records have images
        //returns [imageNotCaptured, recordsWithNoImages]
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

        //collector number should only be a number or s.n.
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
        public static bool countryInList(DataRow row, Dictionary<string, string> countryCodes)
        {

            string country = row["country"].ToString().Trim();
            return countryCodes.ContainsKey(country);

        }

        public static bool majorAreaIsEmpty(DataRow row)
        {
            string majorarea = row["majorarea"].ToString().Trim();
            return String.IsNullOrEmpty(majorarea);
        }

        public static bool minorAreaIsEmpty(DataRow row)
        {
            string minorarea = row["minorarea"].ToString().Trim();
            return String.IsNullOrEmpty(minorarea);
        }

        public static bool localityIsEmpty(DataRow row)
        {
            string locality = row["locality"].ToString().Trim();
            return String.IsNullOrEmpty(locality);
        }

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
                if (family.Length == 0)
                {
                    return false;
                }
            }

            if (species.Length > 0)
            {
                if (genus.Length == 0)
                {
                    return false;
                }
            }

            if (infrataxon1.Length > 0)
            {
                if (species.Length == 0)
                {
                    return false;
                }
            }

            if (infrataxon2.Length > 0)
            {
                if (infrataxon1.Length == 0)
                {
                    return false;
                }
            }

            //else
            return true;

        }

        //get the taxon ranks for this record not in the backbone
        public static string taxaInBackbone (DataRow row, DataTable taxa)
        {
            var taxonBackbone = taxa.AsEnumerable();

            List<string> ranksNotIn = new List<string>();

            var familyIn = taxonBackbone.Where(bbr => bbr["family"].ToString().Trim() == row["family"].ToString().Trim()).FirstOrDefault();
            if (familyIn != null)
            {
                ranksNotIn.Add("family");
            }

            var genusIn = taxonBackbone.Where(bbr => bbr["genus"].ToString().Trim() == row["genus"].ToString().Trim()).FirstOrDefault();
            if (genusIn != null)
            {
                ranksNotIn.Add("genus");
            }

            string species = row["sp1"].ToString().Trim();
            if (!String.IsNullOrEmpty(species))
            {
                var speciesIn = taxonBackbone.Where(bbr =>
                    bbr["genus"].ToString().Trim() == row["genus"].ToString().Trim() &&
                    bbr["sp1"].ToString().Trim() == row["sp1"].ToString().Trim()
                ).FirstOrDefault();

                if (speciesIn != null)
                {
                    ranksNotIn.Add("species");
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

                if (subspeciesIn != null)
                {
                    ranksNotIn.Add("infraspecific1");
                }
            }

            string variety = row["sp3"].ToString().Trim();
            if (!String.IsNullOrEmpty(variety))
            {
                var varietyIn = taxonBackbone.Where(bbr =>
                    bbr["genus"].ToString().Trim() == row["genus"].ToString().Trim() &&
                    bbr["sp1"].ToString().Trim() == row["sp1"].ToString().Trim() &&
                    bbr["sp2"].ToString().Trim() == row["sp2"].ToString().Trim() &&
                    bbr["sp3"].ToString().Trim() == row["sp"].ToString().Trim()
                ).FirstOrDefault();

                if (varietyIn != null)
                {
                    ranksNotIn.Add("infraspecific2");
                }
            }

            return String.Join("; ", ranksNotIn.ToArray());

        }

        //for finding duplicate records using accession number
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

        //check QDS is valid for country
        public static bool QDSValidForCountry(DataRow row, Dictionary<string, string> countryCodes, Dictionary<string, List<string>> countryQDSs)
        {
            string qds = row["qds"].ToString().Trim();
            string country = row["country"].ToString().Trim();

            if (qds.Length > 0)
            {
                string countryCode = "";
                if (countryCodes.TryGetValue(country, out countryCode)) //check this country is in our codes dictionary
                {
                    if (countryQDSs[countryCode].Contains(qds))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                } //else
                return true; //no test
            } //else
            return true; //no test
        }

        //check coords match QDS
        public static bool coordsMatchQDS(DataRow row)
        {
            string latStr = row["lat"].ToString().Trim();
            string lngStr = row["long"].ToString().Trim();
            string unit = row["llunit"].ToString().Trim();
            string qds = row["qds"].ToString().Trim();

            if (latStr.Length > 0 && lngStr.Length > 0 && qds.Length > 0)
            {
                //first check that they parse and they're not just zeros
                try
                {
                    double parsedLat = double.Parse(latStr);
                    double parsedLong = double.Parse(lngStr);

                    if (parsedLat < 0.0000001 && parsedLong < 0.0000001) //&& because we can have 00.0000, 12.25475, for example. 
                    {
                        return true; //no test
                    }
                    else
                    {
                        //if we have one, we must have the other
                        if ((latStr.Length > 0 && lngStr.Length == 0) || (latStr.Length == 0 && lngStr.Length > 0))
                        {
                            throw new Exception(); //missing coordinates

                        }

                        //See if they convert
                        try
                        {
                            string decimalCoords = getDecimalCoords(latStr, lngStr, unit); //this might throw
                            if (qds == getQDSFromCoords(decimalCoords))
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
                            throw;
                        }
                    }

                }
                catch
                {
                    throw;
                }
            }
            else
            {
                return true; //no test
            }
        }

        //check all collectors in master table
        public static string getCollectorsNotInList(DataRow row, DataTable masterAgents)
        {
            string collectors = row["collector"].ToString().Trim();
            string additional = row["addcoll"].ToString().Trim();


            if (additional != "")
            {
                collectors += "; " + additional;
            }

            return getAgentNamesNotInList(collectors, masterAgents);
        }

        //check determiner in master table
        public static bool isDeterminerInList(DataRow row, DataTable masterAgents)
        {
            string detby = row["detby"].ToString().Trim();

            string result = getAgentNamesNotInList(detby, masterAgents);

            return String.IsNullOrEmpty(result);

        }

        //HELPERS, BUT SOME USED ELSEWHERE
        public static string getDecimalCoords(string latStr, string lngStr, string unit) //expects non-empty numeric only strings
        {
            //look for other kinds of problems
            if (unit == "DD")
            {

                double lat = double.Parse(latStr);
                double lng = double.Parse(lngStr);

                if (lat > 90 || lat < -90 || lng > 180 || lng < -180)
                {
                    throw new Exception();
                }
                else
                {
                    return $"{lat}, {lng}";
                }

            }
            else //DM or DMS
            {
                //there is always a decimal
                char[] separators = { '.' };
                string[] latParts = latStr.Split(separators);
                string[] lngParts = lngStr.Split(separators);

                int latDeg = int.Parse(latParts[0]);
                int lngDeg = int.Parse(lngParts[0]);

                if (latDeg > 90 || latDeg < -90 || lngDeg > 180 || lngDeg < -180)
                {
                    throw new Exception();
                }

                //the M or MS part
                if (unit == "DM") //its one number
                {
                    //add the decimals
                    string latMin = latParts[1].Insert(2, ".");
                    string lngMin = lngParts[1].Insert(2, ".");

                    double latMinNum = double.Parse(latMin);
                    double lngMinNum = double.Parse(lngMin);

                    if (latMinNum > 60 || lngMinNum > 60)
                    {
                        throw new Exception();
                    }

                    double lat = latDeg + latMinNum / 60;
                    double lng = lngDeg + lngMinNum / 60;

                    return $"{lat}, {lng}";

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

                    if (latMinNum > 60 || lngMinNum > 60)
                    {
                        throw new Exception();
                    }

                    if (latSecNum > 60 || lngSecNum > 60)
                    {
                        throw new Exception();
                    }

                    double lat = latDeg + latMinNum / 60 + latSecNum / 3600;
                    double lng = lngDeg + lngMinNum / 60 + lngSecNum / 3600;

                    return $"{lat}, {lng}";

                }
                else //there is no valid unit value
                {
                    throw new Exception();
                }
            }
        }

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

        private static string getAgentNamesNotInList(string agentsString, DataTable masterAgents)
        {

            var masterAgentsEnum = masterAgents.AsEnumerable();

            //split the agents and remove periods from initials
            char[] agentSeparator = { ';' };
            List<string> agentsStringList = agentsString.Split(agentSeparator).ToList();

            string agentsNotMatched = "";
            //create a list of agents
            foreach(string agentString in agentsStringList)
            {
                char[] sep = { ',' };
                string[] parts = agentsString.Split(sep);
                string lastName = parts[0].Trim();
                string initials = parts[1].Replace(".", "").Replace(" ", "").Trim(); //replace periods and whitespace

                int matchingAgentCount = masterAgentsEnum.Where(row => row["surname"].ToString().Trim() == lastName && row["initials"].ToString().Trim() == initials).Count();

                if (matchingAgentCount < 1) //we must have at least one
                {
                    agentsNotMatched += agentString + "; ";
                }
            }

            if (agentsNotMatched != "")
            {
                //trim the last '; '
                agentsNotMatched = agentsNotMatched.Substring(0, agentsNotMatched.Length - 2);

            }

            return agentsNotMatched;

        }
    }
}
