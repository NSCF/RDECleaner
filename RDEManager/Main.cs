using FileHelpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;


namespace RDEManager
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            
        }

        //UI FUNCTIONS

        private void btnChooseDBF_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "dbf files (*.dbf)|*.dbf";
                openFileDialog.FilterIndex = 1;
                openFileDialog.Multiselect = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    
                    this.workingDir = Path.GetDirectoryName(openFileDialog.FileNames[0]);

                    string[] fileParts = new string[openFileDialog.FileNames.Length]; //start empty
                    for (int i = 0; i < openFileDialog.FileNames.Length; i++)
                    {
                        fileParts[i] = Path.GetFileName(openFileDialog.FileNames[i]);
                    }
                    string allParts = string.Join(" | ", fileParts);

                    txtChooseDBF.Text = allParts;
                    btnAddDBFData.Enabled = true;

                }
            }
            
        }

        private void addFile_Click(object sender, EventArgs e)
        {
            this.records = new DataTable();

            string[] stringSeparators = new string[] { " | " };
            string[] fileNames = txtChooseDBF.Text.Split(stringSeparators, StringSplitOptions.None);          

            for (int i = 0; i < fileNames.Length; i++)
            {
                addDBFRecords(fileNames[i]);
            }

            btnCheckRDEFile.Enabled = true;

        }

        private void btnChooseImageFolder_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    txtImageFolder.Text = fbd.SelectedPath;

                }
            }
        }

        private void btnChooseTaxonBackbone_Click(object sender, EventArgs e)
        {

            this.taxa = new DataTable();

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {

                openFileDialog.Filter = "dbf files (*.dbf)|*.dbf";
                openFileDialog.FilterIndex = 1;

                if (openFileDialog.ShowDialog() == DialogResult.OK && openFileDialog.FileName != null)
                {

                    string taxonFile = Path.GetFileName(openFileDialog.FileName);
                    string taxonDirectory = Path.GetDirectoryName(openFileDialog.FileName);
                    
                    try
                    {
                        readDBFTable(taxonDirectory, taxonFile, this.taxa);                      
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("Error reading taxon backbone file: " + ex.Message);
                        return;
                    }

                    MessageBox.Show("Taxon file successfully read");
                    txtTaxonBackbone.Text = taxonFile;
                    lblNoTaxa.Text += $" {this.taxa.Rows.Count}";
                    return;

                }
            }
        }

        private void btnChooseQDSCountries_Click(object sender, EventArgs e)
        {

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {

                openFileDialog.Filter = "csv files (*.csv)|*.csv";
                openFileDialog.FilterIndex = 1;

                if (openFileDialog.ShowDialog() == DialogResult.OK && openFileDialog.FileName != null)
                {

                    string QDSCountriesFile = Path.GetFileName(openFileDialog.FileName);

                    this.QDSCountries = new List<QDSCountry>();
                    var engine = new FileHelperEngine<QDSCountry>();
                    
                    try
                    {
                        QDSCountries = engine.ReadFile("FileIn.txt").ToList();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error reading QDSCountries file: " + ex.Message);
                        return;
                    }

                    MessageBox.Show("QDSCountries file successfully read");
                    txtTaxonBackbone.Text = QDSCountriesFile;
                    return;

                }
            }
        }

        private void btnCheckRecords_Click(object sender, EventArgs e)
        {

        }

        //HELPER FUNCTIONS

        private void readDBFTable(string directory, string fileName, DataTable target)
        {
            string fileExtenstion = Path.GetExtension(fileName);
            string tableName = fileName.Replace(fileExtenstion, "");

            string connectionString = @"Provider=VFPOLEDB.1;Data Source=" + directory + "\\" + fileName;
            string selectSQL = "select * from [" + tableName + "]";
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            using (OleDbCommand command = connection.CreateCommand())
            {
                try
                {
                    connection.Open();
                }
                catch(Exception ex)
                {
                    throw new Exception("Error with database connection: " + ex.Message);
                }
                

                if (connection.State == ConnectionState.Open)
                {
                    OleDbCommand cmd = new OleDbCommand(selectSQL, connection);
                    OleDbDataAdapter DA = new OleDbDataAdapter(cmd);
                    try
                    {
                        DA.Fill(target);

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    return;

                }
                
            }
        }

        private void addDBFRecords(string fileName)
        {
            string folderName = this.workingDir;

            try
            {
                readDBFTable(folderName, fileName, this.records);
                lblNumberOfRecords.Text = "Number of records: " + this.records.Rows.Count;
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Error reading records for {fileName}: {ex.Message}");
            }

        }
        
        private List<RDETrackingRecord> getGoogleSheetData()
        {
            GoogleSheetReader gsr = new GoogleSheetReader();
            List<RDETrackingRecord> capturerData = gsr.readGoogleSheet("1nadDvsbuLl3Mj1swecU2Kup-q515kMIwW8YLQqH39P8", "Sheet1", "A", "E");
            string[] stringSeparators = new string[] { " | " };
            List<string> originalRDEFiles = txtChooseDBF.Text.Split(stringSeparators, StringSplitOptions.None).ToList();
            originalRDEFiles = originalRDEFiles.Select(fname => fname.ToLower().Replace(".dbf", "")).ToList();

            //get the ones which match the RDE files
            capturerData = capturerData.Where(rec => originalRDEFiles.Contains(rec.RDEFileName.Trim())).ToList();

            //check we have them all
            if (originalRDEFiles.Count > capturerData.Count)
            {
                List<string> notFound = originalRDEFiles.Where(file => !capturerData.Any(rec => rec.RDEFileName.Trim() == file.Trim())).ToList();
                MessageBox.Show($"Capturer details not found for {String.Join("; ", notFound.ToArray()).ToUpper()}");
            }

            return capturerData;
        }

        private string getQDSFromCoords(double decimalLat, double decimalLong) //note this works for southern Africa only
        {
            int latWholePart = (int)Math.Truncate(decimalLat);
            int longWholePart = (int)Math.Truncate(decimalLong);

            string QDS = $"{latWholePart}{longWholePart}";

            double latDecPart = decimalLat - latWholePart;
            double longDecPart = decimalLong - longWholePart;

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

        //DATA CLEANING/UPDATING FUNCTIONS

        //remove exclamations from barcode numbers - these are added if the barcode is found on the actual live database
        //these will be checked again by the person uploading the records into the source database
        private void removeBarcodeExclamations()
        {
            DataColumn col = this.records.Columns["barcode"];
            foreach (DataRow row in this.records.Rows)
            {
                string barcodeVal = row[col].ToString();
                if (barcodeVal.Contains("!"))
                {
                    row[col] = barcodeVal.Replace("!", "");
                }
            }
        }

        //Delete identical duplicates
        private void removeDuplicates()
        {
            
            int countBefore = this.records.Rows.Count;
            this.records = this.records.DefaultView.ToTable(true);
            int countAfter = this.records.Rows.Count;

            if (countBefore > countAfter)
            {
                errorsBinding.Add($"There were {countBefore - countAfter} identical duplicate records. These have been removed.");
            }
        }

        //dups must be empty
        private void emptyDups()
        {
            if (this.columnNames.Contains("dups"))
            {

                List<DataRow> dupsNotNull = this.records.Select("dups is not null and trim(dups) <> ''").ToList();

                if (dupsNotNull.Count > 0)
                {
                    string dupsmsg = $"{dupsNotNull.Count} records have a value for dups. These have been cleared.";
                    errorsBinding.Add(dupsmsg);
                    dupsNotNull.ForEach(row =>
                    {
                        row["dups"] = DBNull.Value;
                    });
                }
            }
        }

        //update [who] to include NSCF
        private void updateWHO()
        {
            for (int i = 0; i < this.records.Rows.Count; i++)
            {
                DataRow row = this.records.Rows[i];
                if (!row["who"].ToString().Contains("NSCF")) {
                    row["who"] = row["who"].ToString().Trim() + " (NSCF)";
                }
            }
        }

        //update accession numbers
        private void addAccessionNumbers()
        {
            foreach (DataRow row in this.records.Rows)
            {
                string barcode = row["barcode"].ToString().Trim();
                if (barcode.Length > 0)
                {
                    //strip off the -# at the end
                    char[] separators = { '-', '–', '—' };
                    barcode = barcode.Split(separators)[0];

                    //get the number part
                    string numberString = Regex.Match(barcode, @"\d+").Value;
                    if (numberString.Length > 0)
                    {
                        row["accession"] = numberString;
                    }
                }

            }
        }

        //TODO coordinates that are zero must be emptied. Remember llunit and llres also
        private void clearZeroCoordinates()
        {
            List<string> coordErrors = new List<string>();
            int coordsCounter = 0;

            foreach (DataRow row in this.records.Rows)
            {
                string latStr = row["lat"].ToString().Trim();
                string lngStr = row["long"].ToString().Trim();
                string unit = row["llunit"].ToString().Trim();

                //Do we have anything?
                if (latStr.Length > 0 && lngStr.Length > 0)
                {
                    coordsCounter++;

                    //first check that they parse and they're not just zeros
                    try
                    {
                        double parsedLat = double.Parse(latStr);
                        double parsedLong = double.Parse(lngStr);

                        if (parsedLat < 0.0000001 && parsedLong < 0.0000001) //&& because we can have 00.0000, 12.25475, for example. 
                        {
                            row["lat"] = "";
                            row["long"] = "";
                            row["llunit"] = "";
                            row["llres"] = "";
                        }

                    }
                    catch
                    {
                        MessageBox.Show($"There was an error with parsing coordinates for {row["barcode"].ToString().Trim()}. Please clean the coordinates and then continue");
                        return;
                    }
                }
            }
        }

        //ERROR CHECKING FUNCTIONS

        private void errorCheckingAll()
        {
            //preparation
            this.RDEUIErrors = new List<string>();
            this.errorsBinding = new BindingList<string>(RDEUIErrors);
            this.lbRDEErrors.DataSource = errorsBinding;

            this.errorLogs = new List<string>();

            this.columnNames = this.records.Columns.Cast<DataColumn>()
                                 .Select(x => x.ColumnName)
                                 .ToArray();


            this.countryCodes = new Dictionary<string, string>();

            countryCodes.Add("South Africa", "SOU");
            countryCodes.Add("Zimbabwe", "ZIM");
            countryCodes.Add("Zambia", "ZAM");
            countryCodes.Add("Tanzania", "TAN");
            countryCodes.Add("Swaziland", "SWA");
            countryCodes.Add("Namibia", "NAM");
            countryCodes.Add("Mozambique", "MOZ");
            countryCodes.Add("Malawi", "MAA");
            countryCodes.Add("Lesotho", "LES");
            countryCodes.Add("Botswana", "BOT");
            countryCodes.Add("Angola", "ANG");

            //the heavy lifting...
            this.checkLocalityData();
            this.findNonIdenticalDuplicates();

            //write the error log
            using (System.IO.StreamWriter logfile =
            new System.IO.StreamWriter($"{this.workingDir}\\errorlog.txt"))
            {
                string lastDir = this.workingDir.Substring(this.workingDir.LastIndexOf('\\') + 1);

                logfile.WriteLine($"Error log for {lastDir} \\ {txtChooseDBF.Text}");
                logfile.WriteLine($"{DateTime.Now}");
                logfile.Write(Environment.NewLine);

                foreach (string log in this.errorLogs)
                {
                    logfile.WriteLine(log);
                    logfile.Write(Environment.NewLine);
                }
            }
        }

        //check countries are in our list for Southern Africa and list those that are not
        private void checkCountries()
        {
            HashSet<string> extCountriesOrErrors = new HashSet<string>();
            List<string> barcodesToCheck = new List<string>();

            foreach (DataRow row in this.records.Rows)
            {
                string country = row["country"].ToString().Trim();

                if (!this.countryCodes.ContainsKey(country))
                {
                    extCountriesOrErrors.Add(country);
                    barcodesToCheck.Add(row["barcode"].ToString().Trim());
                }
            }
            if (extCountriesOrErrors.Count > 0)
            {

                String[] stringArray = new String[extCountriesOrErrors.Count];
                extCountriesOrErrors.CopyTo(stringArray);
                this.errorsBinding.Add($"Check these country names found in {barcodesToCheck.Count} records: {String.Join("; ", stringArray)}");
                this.errorLogs.Add($"The following {barcodesToCheck.Count} have country names that need to be checked: " +
                    $"{String.Join("; ", barcodesToCheck.ToArray())}.{Environment.NewLine}" +
                    $"Country names: {String.Join("; ", stringArray)}");
            }
            
        }

        //the major, minor and locality fields are empty
        private void checkLocalityData()
        {
            this.localityWithValues = new List<DataRow>();
            this.majorareaWithValues = new List<DataRow>();
            this.minorareaWithValues = new List<DataRow>();

            //locality must be empty
            if (columnNames.Contains("locality"))
            {
                localityWithValues = this.records.Select("locality is not null and trim(locality) <> ''").ToList();
                if (localityWithValues.Count > 0)
                {
                    string locmsg = $"Field 'locality' contains values for {localityWithValues.Count} records. All locality data must be recorded in 'locnotes'. See the log.";
                    errorsBinding.Add(locmsg);
                    string loclogmsg = $"{localityWithValues.Count } records with locality values: {String.Join("; ", localityWithValues.Select(row => row["barcode"].ToString().Trim()).ToArray())}.";
                    this.errorLogs.Add(loclogmsg);
                }
            }

            //majorarea must be empty
            if (columnNames.Contains("majorarea"))
            {
                majorareaWithValues = this.records.Select("majorarea is not null and trim(majorarea) <> ''").ToList();
                if (majorareaWithValues.Count > 0)
                {
                    string majormsg = $"Field 'majorarea' contains values for {majorareaWithValues.Count} records. All locality data must be recorded in 'locnotes'. See the log.";
                    errorsBinding.Add(majormsg);
                    string majorlogmsg = $"{majorareaWithValues.Count} records with values for majorarea: {String.Join("; ", majorareaWithValues.Select(row => row["barcode"].ToString().Trim()).ToArray())}.";
                    this.errorLogs.Add(majorlogmsg);
                }
            }

            //minorarea must be empty
            if (columnNames.Contains("minorarea"))
            {
                minorareaWithValues = this.records.Select("minorarea is not null and trim(minorarea) <> ''").ToList();
                if (minorareaWithValues.Count > 0)
                {
                    string minormsg = $"Field 'minorarea' contains values for {minorareaWithValues.Count} records. All locality data must be recorded in 'locnotes'";
                    errorsBinding.Add(minormsg);
                    string minorlogmsg = $"{minorareaWithValues.Count} records with values for majorarea: {String.Join("; ", minorareaWithValues.Select(row => row["barcode"].ToString().Trim()).ToArray())}.";
                    this.errorLogs.Add(minorlogmsg);
                }
            }
        }

        //non-identical duplicates
        private void findNonIdenticalDuplicates()
        {
            List<string> capturedBarcodes = this.records.AsEnumerable().Select(x => x["barcode"].ToString().ToUpper().Trim()).ToList();
            var duplicates = capturedBarcodes
                //.Select(b => b.Contains("-") ? b.Substring(0, b.LastIndexOf('-')).Trim() : b.Trim()) //comment this out if we don't want to look at the dash numbers (sheet numbers)
                .GroupBy(x => x.Trim())
                .Where(g => g.Count() > 1)
                .Select(y => new { Element = y.Key, Counter = y.Count() })
                .ToList();

            if (duplicates.Count > 0)
            {

                string dupMessage = $"The following {duplicates.Count} records have non-identical duplicates: ";
                duplicates.ForEach(dup =>
                {
                    dupMessage += $"{dup.Element} ({dup.Counter} duplicates); ";
                });

                dupMessage = dupMessage.Substring(0, dupMessage.Length - 2); //trim the last '; '

                errorsBinding.Add($"There are {duplicates.Count} non-identical duplicates. See the log.");
                this.errorLogs.Add(dupMessage);
            }
        }

        //check all images have been captured
        private void checkAllImagesCaptured()
        {
            if (txtImageFolder.Text != null && txtImageFolder.Text != "")
            {
                string fullPath = "";
                try
                {
                    fullPath = Path.GetFullPath(txtImageFolder.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Images folder is not valid. All images captured not confirmed. Error: " + ex.Message);
                }

                if (fullPath != "" && Directory.Exists(fullPath))
                {
                    //get all the file paths and convert to fileNames
                    this.imagePaths = new List<string>();
                    this.imagePaths = Directory.GetFiles(fullPath, "*", SearchOption.AllDirectories).ToList();
                    List<string> fileNames = new List<string>();
                    for (int i = 0; i < this.imagePaths.Count; i++)
                    {
                        fileNames.Add(Path.GetFileName(this.imagePaths[i]));
                    }

                    //get all the image file names
                    List<string> imageFiles = fileNames.Where(fileName => {
                        string ext = fileName.Substring(fileName.LastIndexOf('.') + 1);
                        return ext == "jpg" || ext == "tif";
                    }).ToList();

                    //get the barcode values from the image file names
                    List<string> barcodesFromFileNames = imageFiles.Select(fileName => {
                        return fileName.Substring(0, fileName.LastIndexOf('.')).Trim();
                    }).ToList();

                    barcodesFromFileNames = barcodesFromFileNames.Select(s => s.ToUpper()).Distinct().ToList(); //there may be cases where the same specimen has more than one image (eg. jpg and tif)

                    //get the captured barcodes
                    List<string> notCaptured = new List<string>();
                    List<string> capturedBarcodes = this.records.AsEnumerable().Select(x => x["barcode"].ToString().ToUpper().Trim()).ToList();
                    notCaptured = barcodesFromFileNames.Where(bf => !capturedBarcodes.Contains(bf)).ToList();

                    //get those in the list where we don't have images
                    List<string> noImages = capturedBarcodes.Where(bc => !barcodesFromFileNames.Contains(bc)).ToList();

                    if (notCaptured.Count > 0)
                    {
                        notCaptured.Sort();
                        errorsBinding.Add($"{notCaptured.Count} image files that do not match captured barcodes. See the log.");
                        string logNotCapturedMsg = $"Image file names without matching barcode values in RDE files ({notCaptured.Count}): {String.Join("; ", notCaptured.ToArray())}";
                        this.errorLogs.Add(logNotCapturedMsg);
                    }

                    if (noImages.Count > 0)
                    {
                        noImages.Sort();
                        errorsBinding.Add($"{noImages.Count} captured barcodes that do not match image files. See the log.");
                        string noImagesMsg = $"No corresponding images for the following RDE barcodes ({noImages.Count}): {String.Join("; ", noImages.ToArray())}";
                        this.errorLogs.Add(noImagesMsg);
                    }
                }
                else
                {
                    MessageBox.Show("Images folder does no exist");
                }
            }
            else
            {
                MessageBox.Show("No images folder selected. Captured records will not be checked against the images used.");
                this.errorLogs.Insert(0, "IMAGES NOT CHECKED!!!!");
            }

            //update the imagelist field to include the image paths
            int imagePathsUpdated = 0;
            foreach (DataRow row in this.records.Rows)
            {
                string bc = row["barcode"].ToString().Trim();
                List<string> paths = this.imagePaths.Where(s => s.Contains(bc)).ToList();
                if (paths.Count > 0)
                {
                    row["imagelist"] = String.Join(Environment.NewLine, paths);
                    imagePathsUpdated++;
                }
            }

            if (imagePathsUpdated > 0)
            {
                errorsBinding.Add($"'imagelist' field updated for {imagePathsUpdated} records");
            }
        }

        //check captured names against the taxon backbone
        private void checkTaxonBackbone()
        {
            if (txtTaxonBackbone.Text != null && txtTaxonBackbone.Text != "" && this.taxa.Rows.Count > 0)
            {
                List<DataRow> recordsEnum = this.records.AsEnumerable().ToList();

                //check that for every rank the required higher rank is present
                List<string> noFamily = recordsEnum.Where(row => String.IsNullOrEmpty(row["family"].ToString()) && !String.IsNullOrEmpty(row["genus"].ToString())).Select(row => row["barcode"].ToString().Trim()).ToList();
                List<string> noGenus = recordsEnum.Where(row => String.IsNullOrEmpty(row["genus"].ToString()) && !String.IsNullOrEmpty(row["sp1"].ToString())).Select(row => row["barcode"].ToString().Trim()).ToList();
                List<string> noSpecies = recordsEnum.Where(row => String.IsNullOrEmpty(row["sp1"].ToString()) && !String.IsNullOrEmpty(row["sp2"].ToString())).Select(row => row["barcode"].ToString().Trim()).ToList();
                List<string> noSubspecies = recordsEnum.Where(row => String.IsNullOrEmpty(row["sp2"].ToString()) && !String.IsNullOrEmpty(row["sp3"].ToString())).Select(row => row["barcode"].ToString().Trim()).ToList();

                if (noFamily.Count > 0 || noGenus.Count > 0 || noSpecies.Count > 0 || noSubspecies.Count > 0)
                {
                    errorsBinding.Add($"There are records missing higher taxa. See the log.");
                }

                if (noFamily.Count > 0)
                {
                    this.errorLogs.Add($"Family names are missing for: {String.Join("; ", noFamily.ToArray())}");
                }

                if (noGenus.Count > 0)
                {
                    this.errorLogs.Add($"Generus names are missing for: {String.Join("; ", noGenus.ToArray())}");
                }

                if (noSpecies.Count > 0)
                {
                    this.errorLogs.Add($"Species names are missing for: {String.Join("; ", noSpecies.ToArray())}");
                }

                if (noSubspecies.Count > 0)
                {
                    this.errorLogs.Add($"Subspecies names are missing for: {String.Join("; ", noSubspecies.ToArray())}");
                }

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

                var checkTaxonNames = this.taxa.AsEnumerable()
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

                if (invalidFamilies.Count > 0 || invalidGenera.Count > 0 || invalidSpecies.Count > 0 || invalidSubspecies.Count > 0 || invalidVars.Count > 0)
                {
                    errorsBinding.Add($"There are taxa that don't match the backbone. See the log.");
                }

                if (invalidFamilies.Count > 0)
                {
                    this.errorLogs.Add($"The following invalid family names were found: {String.Join("; ", invalidFamilies.ToArray())}");
                }

                if (invalidGenera.Count > 0)
                {
                    this.errorLogs.Add($"The following invalid genus names were found: {String.Join("; ", invalidGenera.ToArray())}");
                }

                if (invalidSpecies.Count > 0)
                {
                    List<string> fullSpeciesNames = new List<string>();
                    invalidSpecies.ForEach(sp => fullSpeciesNames.Add($"{sp.genus} {sp.species}"));
                    this.errorLogs.Add($"The following invalid species names were found: {String.Join("; ", fullSpeciesNames.ToArray())}");
                }

                if (invalidSubspecies.Count > 0)
                {
                    List<string> fullSubspeciesNames = new List<string>();
                    invalidSubspecies.ForEach(sp => fullSubspeciesNames.Add($"{sp.genus} {sp.species} {sp.subspecies}"));
                    this.errorLogs.Add($"The following invalid subspecies names were found: {String.Join("; ", fullSubspeciesNames.ToArray())}");
                }

                if (invalidVars.Count > 0)
                {
                    List<string> fullVarNames = new List<string>();
                    invalidVars.ForEach(var => fullVarNames.Add($"{var.genus} {var.species} {var.subspecies} {var.var}"));
                    this.errorLogs.Add($"The following invalid variety/form names were found: {String.Join("; ", fullVarNames.ToArray())}");
                }

            }
        }

        //check dates
        private void checkDates()
        {
       
            List<string> yearErrors = new List<string>();
            List<string> monthErrors = new List<string>();
            List<string> dayErrors = new List<string>();
            foreach (DataRow row in this.records.Rows)
            {
                //years
                int year = -9999; //we need this again later for the day check
                string yearStr = row["year"].ToString().Trim();
                if (!String.IsNullOrEmpty(yearStr))
                {
                    try
                    {
                        year = int.Parse(yearStr);
                        if (year < 1850 || year > DateTime.Now.Year)
                        {
                            yearErrors.Add(row["barcode"].ToString().Trim());
                            //set it back to -1 because we use it again later
                            year = -9999;
                        }
                    }
                    catch
                    {
                        yearErrors.Add(row["barcode"].ToString().Trim());
                    }
                }

                //months
                int mon = -9999; //we use this later
                string monStr = row["month"].ToString().Trim();
                if (!String.IsNullOrEmpty(monStr))
                {
                    
                    try
                    {
                        mon = int.Parse(monStr);
                        if (mon < 1 || mon > 12)
                        {
                            monthErrors.Add(row["barcode"].ToString().Trim());
                            mon = -9999;
                        }
                    }
                    catch
                    {
                        monthErrors.Add(row["barcode"].ToString().Trim());
                    }
                }

                //months
                string dayStr = row["day"].ToString().Trim();
                if (!String.IsNullOrEmpty(dayStr))
                {
                    int day = -9999;
                    try
                    {
                        day = int.Parse(dayStr);
                    }
                    catch
                    {
                        dayErrors.Add(row["barcode"].ToString().Trim());
                    }

                    //this only runs if day is an integer
                    if (day != -9999)
                    {
                        if (mon != -9999)
                        {
                            int maxDays;
                            if (year != -9999)
                            {
                                maxDays = DateTime.DaysInMonth(year, mon);
                            }
                            else 
                            {
                                List<int> ThirtyOneDayMonths = new List<int>(new int[] { 1, 3, 5, 7, 8, 10, 12 });

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
                            }

                            if (day < 1 || day > maxDays)
                            {
                                dayErrors.Add(row["barcode"].ToString().Trim());
                            }
                        }
                        else //we have no month
                        {
                            if (day < 1 || day > 31)
                            {
                                dayErrors.Add(row["barcode"].ToString().Trim());
                            }
                        }
                    }
                }
            }

            if (yearErrors.Count > 0 || monthErrors.Count > 0 || dayErrors.Count > 0)
            {
                int totalCount = yearErrors.Count + monthErrors.Count + dayErrors.Count;
                errorsBinding.Add($"There were collection date errors with {totalCount} records. See the error log.");

                if (yearErrors.Count > 0)
                {
                    this.errorLogs.Add($"Records with year errors ({yearErrors.Count}): {String.Join("; ", yearErrors.ToArray())}");
                }

                if (monthErrors.Count > 0)
                {
                    this.errorLogs.Add($"Records with month errors ({monthErrors.Count}): {String.Join("; ", monthErrors.ToArray())}");
                }

                if (dayErrors.Count > 0)
                {
                    this.errorLogs.Add($"Records with day errors ({dayErrors.Count}): {String.Join("; ", dayErrors.ToArray())}");
                }
            }
        }

        //check coordinates
        //note that this does the QDS check also 
        //Its a horrible dependency but it's there...
        private void checkCoordinatesAndQDSs()
        {

            List<string> coordErrors = new List<string>();
            List<string> QDSCoordsMisMatchErrors = new List<string>();

            int coordsCounter = 0;

            foreach (DataRow row in this.records.Rows)
            {
                string barcode = row["barcode"].ToString().Trim();
                string latStr = row["lat"].ToString().Trim();
                string lngStr = row["long"].ToString().Trim();
                string unit = row["llunit"].ToString().Trim();
                string qds = row["qds"].ToString().Trim();

                //Do we have anything?
                if (latStr.Length > 0 && lngStr.Length > 0)
                {
                    coordsCounter++;

                    //first check that they parse and they're not just zeros
                    try
                    {
                        double parsedLat = double.Parse(latStr);
                        double parsedLong = double.Parse(lngStr);

                        if (parsedLat < 0.0000001 && parsedLong < 0.0000001) //&& because we can have 00.0000, 12.25475, for example. 
                        {
                            //we don't clear them here, that is cleaning
                            continue;
                        }

                    }
                    catch
                    {
                        coordErrors.Add(barcode);
                        continue;
                    }

                    //if we have one, we must have the other
                    if ((latStr.Length > 0 && lngStr.Length == 0) || (latStr.Length == 0 && lngStr.Length > 0))
                    {
                        coordErrors.Add(barcode);
                        continue;

                    }

                    //look for other kinds of problems
                    if (unit == "DD")
                    {

                        double lat = double.Parse(latStr);
                        double lng = double.Parse(lngStr);

                        if (lat > 90 || lat < -90 || lng > 180 || lng < -180)
                        {
                            coordErrors.Add(barcode);
                            continue; 
                        }

                        if (qds.Length > 0)
                        {
                            if (qds != getQDSFromCoords(lat, lng))
                            {
                                QDSCoordsMisMatchErrors.Add(barcode);
                            }
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
                            coordErrors.Add(barcode);
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
                                coordErrors.Add(barcode);
                                continue;
                            }

                            if (qds.Length > 0)
                            {
                                double lat = latDeg + latMinNum / 60;
                                double lng = lngDeg + lngMinNum / 60;

                                if (qds != getQDSFromCoords(lat, lng))
                                {
                                    QDSCoordsMisMatchErrors.Add(barcode);
                                }
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

                            if (latMinNum > 60 || lngMinNum > 60)
                            {
                                coordErrors.Add(barcode);
                                continue;
                            }

                            if (latSecNum > 60 || lngSecNum > 60)
                            {
                                coordErrors.Add(barcode);
                                continue;
                            }

                            if (qds.Length > 0)
                            {
                                double lat = latDeg + latMinNum / 60 + latSecNum / 3600;
                                double lng = lngDeg + lngMinNum / 60 + lngSecNum / 3600;

                                if (qds != getQDSFromCoords(lat, lng))
                                {
                                    QDSCoordsMisMatchErrors.Add(barcode);
                                }
                            }

                        }
                        else //there is no unit value
                        {
                            coordErrors.Add(barcode);
                        }
                    }

                } 
            }

            //report on what we got
            if (coordErrors.Count > 0)
            {
                errorsBinding.Add($"There were coordinate errors for {coordErrors.Count} records. See the error log.");
                errorLogs.Add($"There were coordinate errors in the following {coordErrors.Count} records: {String.Join("; ", coordErrors.ToArray())}");
            }

            if(QDSCoordsMisMatchErrors.Count > 0)
            {
                errorsBinding.Add($"There were coordinate / QDS mismatches for {QDSCoordsMisMatchErrors.Count} records. See the error log.");
                errorLogs.Add($"There were coordinate / QDS mismatches for the following {QDSCoordsMisMatchErrors.Count} records: {String.Join("; ", QDSCoordsMisMatchErrors.ToArray())}");

            }
        }

        //Check QDSs are valid for country
        private void checkQDSValidForCountry()
        {
            



        }
        

        //TESTING FUNCTIONS


        //PROPERTIES

        private string workingDir { get; set; }

        //data
        private DataTable records { get; set; }
        private DataTable taxa { get; set; }
        private List<QDSCountry> QDSCountries { get; set; }
        private string[] columnNames { get; set; }
        private List<string> imagePaths { get; set; }

        Dictionary<string, string> countryCodes { get; set; }

        //reporting
        private BindingList<string> fileNamesBinding { get; set; }
        

        private List<string> RDEUIErrors { get; set; }
        private BindingList<string> errorsBinding { get; set; }
        private List<string> errorLogs { get; set; }

        private List<string> RDEcleaning { get; set; }
        private BindingList<string> cleaningBinding { get; set; }
        private List<string> cleaningLogs { get; set; }

        //errors
        private List<DataRow> localityWithValues { get; set; }
        private List<DataRow> majorareaWithValues { get; set; }
        private List<DataRow> minorareaWithValues { get; set; }

        
    }
}
