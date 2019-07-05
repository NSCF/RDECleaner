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

            this.viewRecordsBinding = new BindingSource();

            this.dgvRecordsView.DataSource = this.viewRecordsBinding;

            this.dupsSearched = false;

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

            this.viewRecordsBinding.DataSource = this.records;

            btnCheckDuplicates.Enabled = true;

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

        private void btnChoosePeople_Click(object sender, EventArgs e)
        {
            this.people = new DataTable();

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {

                openFileDialog.Filter = "dbf files (*.dbf)|*.dbf";
                openFileDialog.FilterIndex = 1;

                if (openFileDialog.ShowDialog() == DialogResult.OK && openFileDialog.FileName != null)
                {

                    string peopleFile = Path.GetFileName(openFileDialog.FileName);
                    string peopleDirectory = Path.GetDirectoryName(openFileDialog.FileName);

                    try
                    {
                        readDBFTable(peopleDirectory, peopleFile, this.taxa);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error reading people table: " + ex.Message);
                        return;
                    }

                    MessageBox.Show("People table successfully imported");
                    txtPeopleTable.Text = peopleFile;

                }
            }
        }

        private void btnFindMissingRecords_Click(object sender, EventArgs e)
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
                    MessageBox.Show("Images folder is not valid. Please select a valid images folder");
                    return;
                }

                if (Directory.Exists(fullPath))
                {
                    rtbReportErrors.Clear();

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

                    List<string>[] errors = RecordErrorFinder.checkAllImagesCaptured(this.records, imageFiles);

                    List<string> notCaptured = errors[0];
                    List<string> noImages = errors[1];

                    if (notCaptured.Count > 0)
                    {
                        notCaptured.Sort();
                        string codesNotCaptured = String.Join("; ", notCaptured.ToArray());
                        
                        rtbReportErrors.Text += $"{notCaptured.Count} image files that do not match captured barcodes. See the log.{Environment.NewLine}";
                        rtbReportErrors.Text += codesNotCaptured + Environment.NewLine + Environment.NewLine;
                        string logNotCapturedMsg = $"Image file names without matching barcode values in RDE files ({notCaptured.Count}): {codesNotCaptured}";
                        this.errorLogs.Add(logNotCapturedMsg);
                    }

                    if (noImages.Count > 0)
                    {

                        noImages.Sort();
                        string codesNoImages = String.Join("; ", noImages.ToArray());

                        rtbReportErrors.Text += $"{noImages.Count} captured barcodes that do not match image files. See the log.";
                        rtbReportErrors.Text += codesNoImages + Environment.NewLine + Environment.NewLine;
                        string noImagesMsg = $"No corresponding images for the following RDE barcodes ({noImages.Count}): {codesNoImages}";
                        this.errorLogs.Add(noImagesMsg);
                    }
                }
                else
                {
                    MessageBox.Show("Images folder does not exist. Please select a valid images folder");
                    return;
                }
            }
            else
            {
                MessageBox.Show("No images folder selected. Captured records will not be checked against the images used.");
                this.errorLogs.Insert(0, "IMAGES NOT CHECKED!!!!");
            }
        }

        private void btnCheckDuplicates_Click(object sender, EventArgs e)
        {
            RecordCleaner.removeBarcodeExclamations(this.records);

            int duplicatesRemoved = RecordCleaner.removeDuplicates(this.records);

            if (!this.dupsSearched)
            {
                this.findBotRecDuplicates();
            }

            MessageBox.Show($"{duplicatesRemoved} duplicate records removed");

            this.btnNextDuplicate.Enabled = true;

            this.btnCheckDuplicates.Enabled = false;

        }

        private void btnGetDuplicates_Click(object sender, EventArgs e)
        {
            if (!this.dupsSearched)
            {
                this.findBotRecDuplicates();
            }
        }

        private void btnFindNextDuplicate_Click(object sender, EventArgs e)
        {

            if (this.botRecDuplicatesIndex < this.botRecDuplicates.Count)
            {
                BotanicalRecordDuplicateTracker next = this.botRecDuplicates[this.botRecDuplicatesIndex];

                //make the search string
                string INSearch = "";
                foreach (string dupBarcode in next.dupBarcodes)
                {
                    string quoted = $"'{dupBarcode}'";
                    INSearch += quoted + ", ";
                }
                INSearch = INSearch.Substring(0, INSearch.Length - 2);

                string expression = $"barcode IN ({INSearch})";

                this.viewRecordsBinding.Filter = expression;

                this.botRecDuplicatesIndex++;

                this.lblDupIndexCount.Text = $"{this.botRecDuplicatesIndex} of {this.botRecDuplicates.Count} duplicates";
            }
            else
            {
                MessageBox.Show("All duplicates have been processed.");
                this.viewRecordsBinding.Filter = null;
                this.btnCheckDuplicates.Enabled = false;
                this.lblDupIndexCount.Text = "";
            }
        }

        private void btnTestSomething_Click(object sender, EventArgs e)
        {
            this.viewRecordsBinding.DataSource = this.records;
        }

        private void dgvRecordsView_SelectionChanged(object sender, EventArgs e)
        {

            if (this.viewRecordsBinding.DataSource != null && this.dgvRecordsView.SelectedRows.Count > 0)
            {
                this.rtbReportErrors.Text = this.dgvRecordsView.SelectedRows[0].Cells["locnotes"].Value.ToString();
            }

        }

        private void btnMergeDups_Click(object sender, EventArgs e)
        {
            if (this.dgvRecordsView.SelectedRows.Count != 1)
            {
                MessageBox.Show("One row must be selected as the master record to merge into...");
                return;
            }
            else
            {
                string masterBarcode = this.dgvRecordsView.SelectedRows[0].Cells["barcode"].Value.ToString().Trim();

                //we incremented the index of the duplicates tracker, so we need to go one back
                BotanicalRecordDuplicateTracker dupInfo = this.botRecDuplicates[this.botRecDuplicatesIndex - 1];

                //if they're the same then we can't merge
                if (dupInfo.dupBarcodes.Distinct().Count() < dupInfo.dupBarcodes.Count) //if any are not equal to the first
                {
                    MessageBox.Show("One row must be selected as the master record to merge into...");
                    return;
                }
                else
                {
                    //process each one
                    string masterRDEspec = this.dgvRecordsView.SelectedRows[0].Cells["rdespec"].Value.ToString().Trim();
                    if (!String.IsNullOrEmpty(masterRDEspec))
                    {
                        XMLSpecimenList rdespeclist = RecordCleaner.rdeSpecToList(masterRDEspec);
                        
                        //go through each duplicate record, check if it's already in the rdespec, if not, add it
                        foreach(string dupBarcode in dupInfo.dupBarcodes)
                        {
                            
                            int count = rdespeclist.Specimens.Where(s => s.barcode == dupBarcode).Count();
                            if (count == 0) //add it
                            {
                                XMLSpecimen spec = new XMLSpecimen();
                                spec.barcode = dupBarcode;
                                spec.ih = "Herbarium sheet";

                                //get the various parts
                                string numberString = Regex.Match(dupBarcode, @"\d+").Value;
                                int numberIndex = dupBarcode.IndexOf(numberString);
                                string prefix = dupBarcode.Substring(0, numberIndex);

                                spec.ccid = prefix;
                                spec.accession = numberString;

                            }
                        }

                        //write it back again
                        this.dgvRecordsView.SelectedRows[0].Cells["rdespec"].Value = rdespeclist.ToXMLString();
                    }
                    else
                    {
                        XMLSpecimenList rdespeclist = new XMLSpecimenList();
                        foreach (string dupBarcode in dupInfo.dupBarcodes)
                        {

                            XMLSpecimen spec = new XMLSpecimen();
                            spec.barcode = dupBarcode;
                            spec.ih = "Herbarium sheet";

                            //get the various parts
                            string numberString = Regex.Match(dupBarcode, @"\d+").Value;
                            int numberIndex = dupBarcode.IndexOf(numberString);
                            string prefix = dupBarcode.Substring(0, numberIndex);

                            spec.ccid = prefix;
                            spec.accession = numberString;
                        }

                        //write it back again
                        this.dgvRecordsView.SelectedRows[0].Cells["rdespec"].Value = rdespeclist.ToXMLString();
                    }
                }
            }
        }

        private void btnDeleteRows_Click(object sender, EventArgs e)
        {
            if (this.dgvRecordsView.SelectedRows.Count > 1) //many selected records
            {
                if (MessageBox.Show($"Are you sure you want to delete {this.dgvRecordsView.SelectedRows.Count} records?", 
                    "Confirm delete", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    foreach (DataGridViewRow item in this.dgvRecordsView.SelectedRows)
                    {
                        this.dgvRecordsView.Rows.RemoveAt(item.Index);
                        this.records.AcceptChanges();
                    }
                }

            }
            else //only one record
            {
                foreach (DataGridViewRow item in this.dgvRecordsView.SelectedRows)
                {
                    this.dgvRecordsView.Rows.RemoveAt(item.Index);
                    this.records.AcceptChanges();
                }
            }

        }

        private void btnCleanRecords_Click(object sender, EventArgs e)
        {
            int collNumUpdates = 0;
            if (rbNumberFromBarcode.Checked)
            {
                collNumUpdates = RecordCleaner.addCollNumberFromBarcode(this.records);
            }
            else if (rbNumberToSN.Checked)
            {
                collNumUpdates = RecordCleaner.changeEmptyCollectorNumbersToSN(this.records);
            }

            if (collNumUpdates > 0)
            {
                rtbReportErrors.Text += $"Collector number updated for {collNumUpdates} records{Environment.NewLine}";
            }

            int dupsUpdated = RecordCleaner.updateDups(this.records);

            if (dupsUpdated > 0)
            {
                rtbReportErrors.Text += $"Dups updated for {dupsUpdated} records{Environment.NewLine}";
            }

            RecordCleaner.updateWHO(this.records);

            RecordCleaner.addAccessionNumbers(this.records);

            RecordCleaner.clearZeroCoordinates(this.records);

            MessageBox.Show("Record cleaning complete");


        }

        private void btnAddQDSFromCoords_Click(object sender, EventArgs e)
        {
            rtbReportErrors.Clear();
            int qdsUpdates = RecordCleaner.addQDSFromCoordinates(this.records);
            if(qdsUpdates > 0)
            {
                rtbReportErrors.Text += $"QDS updated for {qdsUpdates} records";
            }

            btnCheckQDSCountry.Enabled = true;
            btnAddQDSFromCoords.Enabled = false;

        }

        private void btnCheckQDSCountry_Click(object sender, EventArgs e)
        {
            //TODO this is a row level check again
        }

        //handle delete button presses for datagrid rows
        private void dgvRecordsView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (char)Keys.Delete)
            {
                e.Handled = true;
                MessageBox.Show("Use the buttons to delete records...");
            }
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

        private void findBotRecDuplicates()
        {

            this.botRecDuplicates = new List<BotanicalRecordDuplicateTracker>();
            this.botRecDuplicatesIndex = 0;

            char[] seps = { '-', '–', '—' };

            foreach (DataRow row in this.records.Rows)
            {
                string barcode = row["barcode"].ToString().Trim();
                string root = barcode.Split(seps)[0];

                //sometimes there is no barcode
                if (String.IsNullOrEmpty(root))
                {
                    continue;
                }

                // check if we've already processed this root
                int cnt = this.botRecDuplicates.Where(x => x.root == root).Count();

                if (cnt == 0)
                {
                    DataRow[] dups = this.records.Select($"barcode like '{root}*'");

                    if (dups.Count() > 1)
                    {
                        List<string> dupBarcodes = new List<string>();

                        foreach (DataRow dr in dups)
                        {
                            dupBarcodes.Add(dr["barcode"].ToString().Trim());
                        }

                        this.botRecDuplicates.Add(new BotanicalRecordDuplicateTracker(root, dupBarcodes));

                        this.lblDupsCount.Text = "No. Dups: " + this.botRecDuplicates.Count;

                    }
                }

            }

            this.dupsSearched = true;
        }

        //ERROR CHECKING FUNCTIONS

        private void errorCheckingAll()
        {
            //preparation
            this.RDEUIErrors = new List<string>();
            this.errorsBinding = new BindingList<string>(RDEUIErrors);
            //this.lbRDEErrors.DataSource = errorsBinding;

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

            //checking countries
            HashSet<string> extCountriesOrErrors = new HashSet<string>();
            List<string> barcodesToCheck = new List<string>();
            if (extCountriesOrErrors.Count > 0)
            {

                String[] stringArray = new String[extCountriesOrErrors.Count];
                extCountriesOrErrors.CopyTo(stringArray);
                this.errorsBinding.Add($"Check these country names found in {barcodesToCheck.Count} records: {String.Join("; ", stringArray)}");
                this.errorLogs.Add($"The following {barcodesToCheck.Count} have country names that need to be checked: " +
                    $"{String.Join("; ", barcodesToCheck.ToArray())}.{Environment.NewLine}" +
                    $"Country names: {String.Join("; ", stringArray)}");
            }

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
            

        }

        //check captured names against the taxon backbone
        private void checkTaxonBackbone()
        {
            if (txtTaxonBackbone.Text != null && txtTaxonBackbone.Text != "" && this.taxa.Rows.Count > 0)
            {
                

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
        //To be honest I think adding the QDS needs to be an option for only after coordinates have been cleaned...
        private void checkCoordinatesAndQDSs()
        {

            List<string> coordErrors = new List<string>();
            List<string> QDSCoordsMisMatchErrors = new List<string>();

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
            if (this.QDSCountries.Count > 0)
            {
                List<string> QDSErrors = new List<string>();

                //lets build the list of qdss per country so we don't have to do it on every iteration
                Dictionary<string, List<string>> countryQDSs = new Dictionary<string, List<string>>();

                foreach (KeyValuePair<string, string> code in this.countryCodes)
                {
                    string countryCode = code.Value;
                    List<string> qdss = this.QDSCountries.Where(x => x.CountryCode == countryCode).Select(x => x.QDS.Trim()).ToList();
                    countryQDSs.Add(countryCode, qdss);
                }

                if (QDSErrors.Count > 0)
                {
                    this.errorsBinding.Add($"There were country / QDS mismatches for {QDSErrors.Count} records.");
                    this.errorLogs.Add($"There were country / QDS mismatches for the following {QDSErrors.Count} records: {String.Join("; ", QDSErrors.ToArray())}");
                }
            }
        }
        


        //PROPERTIES

        private string workingDir { get; set; }

        //data
        private DataTable records { get; set; }
        private DataTable taxa { get; set; }
        private DataTable people { get; set; }

        private List<BotanicalRecordDuplicateTracker> botRecDuplicates { get; set; }
        private bool dupsSearched { get; set; }
        private int botRecDuplicatesIndex { get; set; }

        private DataTable viewRecords { get; set; }
        private BindingSource viewRecordsBinding { get; set; }

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
