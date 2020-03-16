using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace RDEManager
{
    public static class RecordCleaner
    {
        /// <summary>
        ///remove exclamations from barcode numbers - these are added if the barcode is found on the actual live database
        ///these will be checked again by the person uploading the records into the source database
        /// </summary>
        /// <param name="records">The records to clean</param>
        public static void removeBarcodeExclamations(DataTable records)
        {
            DataColumn col = records.Columns["barcode"];
            foreach (DataRow row in records.Rows)
            {
                string barcodeVal = row[col].ToString().Trim();
                if (barcodeVal.Contains("!"))
                {
                    row[col] = barcodeVal.Replace("!", "");
                }
            }
        }

        /// <summary>
        /// Remove identical duplicate records and return the number of records removed
        /// </summary>
        public static int removeDuplicates(DataTable records)
        {

            int countBefore = records.Rows.Count;

            try
            {
                records = records.DefaultView.ToTable(true); //this removes any duplicates
            }
            catch(Exception ex)
            {
                throw ex;
            }

            
            int countAfter = records.Rows.Count;

            int duplicates = countBefore - countAfter;

            return duplicates;
        }

        //find specimens, remove duplicated botanical records, and add the specimens in rdespec
        //THIS IS NOW REDUNDANT, SEE btnCheckDuplicates_Click()
        public static int moveBotanicalRecordDuplicatesToRDESpec(DataTable records)
        {
            int affectedRows = 0;

            var recordsEnum = records.AsEnumerable();

            List<string> dupsAlreadyChecked = new List<string>();

            List<DataRow> rowsToRemove = new List<DataRow>();

            foreach (DataRow row in records.Rows)
            {
                // find others with the same barcode root
                string barcode = row["barcode"].ToString().Trim();
                char[] sep = { '-', '–', '—' }; //this assumes that dashes are used to separate suffixes for dups

                string root = barcode.Split(sep)[0];

                //check if we've already processed duplicates for this root
                if (!dupsAlreadyChecked.Contains(root))
                {
                    //get the other rows like this - remember this will include the same row
                    var dups = recordsEnum.Where(r =>
                    {
                        string rbc = r["barcode"].ToString().Trim();
                        string rbcRoot = rbc.Split(sep)[0];
                        return rbcRoot == root;
                    }).ToList();

                    if (dups.Count > 1)
                    {
                        //the one with the lowest integer suffix in the barcode is the master record
                        DataRow masterRecord = dups[0];
                        string masterBarcode = masterRecord["barcode"].ToString().Trim();
                        int masterSuffix = int.Parse(masterBarcode.Split(sep)[1]);

                        //find the master record and add the others to rows for later deletion
                        foreach (DataRow dup in dups)
                        {
                            //get the integer suffix
                            string dbc = row["barcode"].ToString().Trim();
                            int dupSuffix = int.Parse(dbc.Split(sep)[1]);

                            if (dupSuffix < masterSuffix)
                            {
                                rowsToRemove.Add(masterRecord);
                                masterRecord = dup;
                            }
                            else
                            {
                                rowsToRemove.Add(dup);
                            }
                        }

                        //we're all done
                        dupsAlreadyChecked.Add(root);

                    }
                }

            }

            return affectedRows;
        }

        //add collector number from barcode number - this is an option
        public static int addCollNumberFromBarcode(DataTable records)
        {
            int updates = 0;
            foreach (DataRow row in records.Rows)
            {
                string collNum = row["number"].ToString().Trim();
                if (!String.IsNullOrEmpty(collNum))
                {
                    string barcode = row["barcode"].ToString().Trim();
                    string numberString = Regex.Match(barcode, @"\d+").Value; //assumed the data are clean and this will work
                    row["number"] = numberString;
                    updates++;
                }
            }

            return updates;
        }

        //add s.n. to empty collector numbers
        public static int changeEmptyCollectorNumbersToSN(DataTable records)
        {
            int counter = 0;

            foreach(DataRow row in records.Rows)
            {
                string collNum = row["number"].ToString().Trim();
                if (String.IsNullOrEmpty(collNum))
                {
                    row["number"] = "s.n.";
                    counter++;
                }
            }

            return counter;
        }

        //dups must have the collection code
        public static int updateDups(DataTable records)
        {
            int updates = 0;
            foreach(DataRow row in records.Rows)
            {

                string rdespec = row["rdespec"].ToString().Trim();
                string dups = row["dups"].ToString().Trim();
                string barcode = row["barcode"].ToString().Trim();
                string collCode = "";

                if (String.IsNullOrEmpty(rdespec)) //no rdespec
                {

                    //get the collection code
                    collCode = getCollCodeFromBarcode(barcode);

                    if(dups != collCode)
                    {
                        updates++;
                        row["dups"] = collCode;
                    }
                }
                else //there is rdespec
                {
                    //deserialize it
                    XMLSpecimenList speclist = rdeSpecToList(rdespec);

                    string allDups = "";

                    bool updated = false;

                    foreach (XMLSpecimen spec in speclist.Specimens)
                    {
                        string code = getCollCodeFromBarcode(spec.barcode);
                        if(spec.ccid.Trim() != code)
                        {
                            spec.ccid = code;
                            updated = true;
                        }

                        allDups += code + ", ";
                    }

                    if (updated)
                    {
                        //'save' it again
                        row["rdespec"] = rdespecListToXML(speclist);
                        row["dups"] = updates;
                        updates++;
                    }

                }

            }

            return updates;

        }

        //update [who] to include NSCF
        public static void updateWHO(DataTable records, string updateString)
        {
            List<string> columnNames = getColumnNames(records);

            if (columnNames.Contains("who"))
            {
                foreach (DataRow row in records.Rows)
                {
                    if (!row["who"].ToString().Contains(updateString))
                    {
                        row["who"] = row["who"].ToString().Trim() + " (" + updateString + ")";
                    }
                }
            }
        }

        //add accession numbers
        //also moves old accession numbers to a oldbarcode if they exist and are different
        public static int addAccessionNumbers(DataTable records)
        {
            int updateCounter = 0; 

            foreach (DataRow row in records.Rows)
            {
                string barcode = row["barcode"].ToString().Trim();
                string rdeSpec = row["rdespec"].ToString().Trim();
                string accession = row["accession"].ToString().Trim();

                if (barcode.Length > 0)
                {
                    //strip off the -# at the end
                    char[] separators = { '-', '–', '—' };
                    barcode = barcode.Split(separators)[0];

                    //get the number part
                    string numberString = Regex.Match(barcode, @"\d+").Value;
                    if (numberString.Length > 0)
                    {
                        if(String.IsNullOrEmpty(accession)) { //just put it in
                            row["accession"] = numberString;
                            updateCounter++;
                        }
                        else //more complicatd
                        {
                            if (accession != numberString) //its a different accession and needs to be moved to rdespec
                            {
                                if (String.IsNullOrEmpty(rdeSpec)) //no rdespec 
                                {
                                    XMLSpecimen spec = new XMLSpecimen();
                                    spec.accession = numberString;
                                    spec.barcode = barcode;
                                    spec.oldbarcode = accession; //this is why we have to do all this

                                    XMLSpecimenList XMLList = new XMLSpecimenList();
                                    XMLList.Specimens.Add(spec);
                                   
                                    string xml = rdespecListToXML(XMLList);

                                    row["rdespec"] = xml;
                                    updateCounter++;


                                }
                                else //we already have RDEspec
                                {

                                    XMLSpecimenList speclist = rdeSpecToList(rdeSpec);
                                    
                                    foreach(XMLSpecimen spec in speclist.Specimens)
                                    {
                                        if (String.IsNullOrEmpty(spec.accession))
                                        {
                                            spec.accession = numberString;
                                            spec.oldbarcode = accession;
                                            updateCounter++;
                                        }
                                        else
                                        {
                                            if (spec.accession != numberString)
                                            {
                                                spec.oldbarcode = spec.accession;
                                                spec.accession = numberString;
                                                updateCounter++;
                                            }
                                        }
                                    }

                                }
                            }

                            //finally update the RDE row accession field
                            row["accession"] = numberString;
                            updateCounter++;

                        }
                        
                    }
                }

            }

            return updateCounter;
        }

        //coordinates that are zero must be emptied. Remember llunit and llres also
        public static int clearZeroCoordinates(DataTable records)
        {
            int updateCount = 0;

            foreach (DataRow row in records.Rows)
            {
                string latStr = row["lat"].ToString().Trim();
                string lngStr = row["long"].ToString().Trim();
                string unit = row["llunit"].ToString().Trim();

                //Do we have anything?
                if (latStr.Length > 0 && lngStr.Length > 0)
                {

                    //first check that they parse and they're not just zeros
                    try
                    {
                        double parsedLat = double.Parse(latStr);
                        double parsedLong = double.Parse(lngStr);

                        if (parsedLat < 0.0000001 && parsedLong < 0.0000001) //&& because we can have 00.0000, 12.25475, for example. 
                        {
                            row["lat"] = DBNull.Value;
                            row["long"] = DBNull.Value;
                            row["llunit"] = DBNull.Value;
                            row["llres"] = DBNull.Value;
                            updateCount++;
                        }

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }

            return updateCount;
        }

        public static int updateImageList(DataTable records, List<string> imagePaths)
        {
            if (imagePaths == null)
            {
                MessageBox.Show("Image paths not available. imagelist field will not be updated.");
                return 0;
            }

            int updateCount = 0;

            //update the imagelist field to include the image paths
            foreach (DataRow row in records.Rows)
            {
                string barcode = row["barcode"].ToString().Trim();

                //this should never happen. We assume all records have barcodes.
                if (string.IsNullOrEmpty(barcode))
                {
                    continue;
                }

                //silent else

                BarcodeParts bcParts = new BarcodeParts(row["barcode"].ToString().Trim());

                string root = bcParts.collectionCode + bcParts.number;
                List<string> paths = imagePaths.Where(s => s.Contains(root) && s.Contains("jpg")).ToList();
                if (paths.Count > 0)
                {
                    row["imagelist"] = String.Join(Environment.NewLine, paths);
                    updateCount++;

                    //check in rdespec
                    string rdespec = row["rdespec"].ToString().Trim();
                    if (!String.IsNullOrEmpty(rdespec))
                    {
                        XMLSpecimenList specs = new XMLSpecimenList(rdespec);

                        foreach(XMLSpecimen spec in specs.Specimens)
                        {
                            spec.imagelink = paths.Where(p => p.Contains(spec.barcode.Trim())).First();
                        }

                        row["rdespec"] = specs.ToXMLString();
                    }
                }
            }
            return updateCount;
        }

        //add quarter degree squares from coords - should only happen after coordinates have been cleaned. 
        public static int addQDSFromCoordinates(DataTable records)
        {

            int updates = 0;
            foreach (DataRow row in records.Rows)
            {
                string qds = row["qds"].ToString().Trim();
                if (String.IsNullOrEmpty(qds))
                {
                    

                    try
                    {
                        string coords = RecordErrorFinder.getDecimalCoords(row);
                        row["qds"] = RecordErrorFinder.getQDSFromCoords(coords);
                        updates++;
                    }
                    catch
                    {
                        continue;
                    }

                }
            }
            return updates;
        }

        //add country from locality notes if missing (search for countrynames)
        public static int addCountries(DataTable records)
        {
            int counter = 0;

            string[] countryNamesArr = {
                "South Africa",
                "Lesotho",
                "Swaziland",
                "Namibia",
                "Botswana",
                "Zimbabwe",
                "Mozambique",
                "Angola",
                "Zambia",
                "Malawi",
                "Tanzania",
                "Kenya"
            };

            List<string> countryNames = countryNamesArr.ToList();

            foreach (DataRow row in records.Rows)
            {
                string country = row["country"].ToString();
                if (!String.IsNullOrEmpty(country))
                {
                    string locality = row["locnotes"].ToString();
                    for (int i = 0; i < countryNames.Count; i++)
                    {
                        if (locality.ToLower().Contains(countryNames[i].ToLower()))
                        {
                            row["country"] = countryNames[i];
                            counter++;
                            break;
                        }
                    }
                }
            }

            return counter;
        }

        //HELPERS
        private static List<string> getColumnNames(DataTable records)
        {
            return records.Columns.Cast<DataColumn>()
                                 .Select(x => x.ColumnName)
                                 .ToList();
        }

        public static string rdespecListToXML(XMLSpecimenList XMLList)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(XMLSpecimenList));
            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    serializer.Serialize(writer, XMLList);
                    string xml = sww.ToString(); // Your XML

                    xml = xml.Replace("<SpecimenList>", "").Replace("</SpecimenList>", "").Replace("&amp;", "&");

                    return xml;

                }
            }
        }

        public static XMLSpecimenList rdeSpecToList(string rdespec)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(XMLSpecimenList));

            //we need to add the list wrapper
            rdespec = $"<SpecimenList>{rdespec}</SpecimenList>".Replace("&", "&amp;"); //we need the replace because ampersands mean something in XML!!

            try
            {
                using (TextReader reader = new StringReader(rdespec))
                {
                    return (XMLSpecimenList)serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error with speclist XML");
                throw ex;
            }

        }

        private static string getCollCodeFromBarcode(string barcode)
        {

            string collCode = "";

            foreach (char c in barcode)
            {
                if (Char.IsNumber(c))
                {
                    break;
                }
                else
                {
                    collCode += c;
                }
            }

            return collCode;
        }

    }
}
