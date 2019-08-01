using FileHelpers;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
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

            this.CountryCodes = new CountryCodes();

            this.rowErrors = new List<string>();

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

            btnChooseImageFolder.Enabled = true;
            btnChooseTaxonBackbone.Enabled = true;
            btnChooseQDSCountries.Enabled = true;
            btnChoosePeople.Enabled = true;
            btnCheckDuplicates.Enabled = true;

        }

        private void btnChooseImageFolder_Click(object sender, EventArgs e)
        {

            FolderBrowserDialog fbd = new FolderBrowserDialog();
            using (fbd)
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    txtImageFolder.Text = fbd.SelectedPath;
                    btnFindMissingRecords.Enabled = true;
                }
            }
        }

        private void btnChooseTaxonBackbone_Click(object sender, EventArgs e)
        {

            this.taxa = new DataTable();

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Choose the taxon backbone file";

            using (openFileDialog)
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
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Choose the QDSCountries list";

            using (openFileDialog)
            {

                openFileDialog.Filter = "csv files (*.csv)|*.csv";
                openFileDialog.FilterIndex = 1;

                if (openFileDialog.ShowDialog() == DialogResult.OK && openFileDialog.FileName != null)
                {

                    string QDSCountriesFile = Path.GetFileName(openFileDialog.FileName);

                    List<CountryQDS> QDSPerCountryCode = new List<CountryQDS>();
                    var engine = new FileHelperEngine<CountryQDS>();
                    
                    //read it all out of the CSV and then convert to the dictionary
                    try
                    {
                        using (TextFieldParser parser = new TextFieldParser(openFileDialog.FileName))
                        {
                            parser.TextFieldType = FieldType.Delimited;
                            parser.SetDelimiters(",");

                            int counter = 0; //so we can skip row 1
                            while (!parser.EndOfData)
                            {
                                string[] values = parser.ReadFields();
                                if (counter > 0) //skip row 1
                                {
                                    QDSPerCountryCode.Add(new CountryQDS(values[0], values[1]));
                                }
                                counter++;
                            }

                            if (QDSPerCountryCode.Count == 0)
                            {
                                throw new Exception("no QDS per country code returned");
                            }

                            //convert to the local dictionary
                            this.CountryQDSs = new Dictionary<string, List<string>>();
                            foreach (CountryQDS countryQDS in QDSPerCountryCode)
                            {
                                string country = "";
                                bool success = this.CountryCodes.Codes.TryGetValue(countryQDS.CountryCode, out country);
                                if (success)
                                {
                                    //is the country in the dictionary?
                                    if (this.CountryQDSs.Keys.Contains(country))
                                    {
                                        this.CountryQDSs[country].Add(countryQDS.QDS);
                                    }
                                    else
                                    {
                                        List<string> qdsList = new List<string>();
                                        qdsList.Add(countryQDS.QDS);
                                        this.CountryQDSs.Add(country, qdsList);
                                    }
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error reading QDSCountries file: " + ex.Message);
                        return;
                    }

                    MessageBox.Show("QDSCountries file successfully read");
                    txtQDSCountriesFile.Text = QDSCountriesFile;
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
                    this.btnCleanRecords.Enabled = true;

                }
            }
        }

        private void btnFindMissingRecords_Click(object sender, EventArgs e)
        {
            RecordCleaner.removeBarcodeExclamations(this.records);

            if (txtImageFolder.Text != null && txtImageFolder.Text != "")
            {
                string fullPath = "";
                try
                {
                    fullPath = Path.GetFullPath(txtImageFolder.Text);
                }
                catch
                {
                    MessageBox.Show("Images folder is not valid. Please select a valid images folder");
                    return;
                }

                if (Directory.Exists(fullPath))
                {
                    this.missingRecordLogs = new List<string>();

                    rtbReportErrors.Clear();

                    //get all the file paths and convert to fileNames
                    this.imagePaths = new List<string>();
                    this.imagePaths = Directory.GetFiles(fullPath, "*", System.IO.SearchOption.AllDirectories).ToList();
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
                        this.missingRecordLogs.Add(logNotCapturedMsg);
                    }

                    if (noImages.Count > 0)
                    {

                        noImages.Sort();
                        string codesNoImages = String.Join("; ", noImages.ToArray());

                        rtbReportErrors.Text += $"{noImages.Count} captured barcodes that do not match image files. See the log.";
                        rtbReportErrors.Text += codesNoImages + Environment.NewLine + Environment.NewLine;
                        string noImagesMsg = $"No corresponding images for the following RDE barcodes ({noImages.Count}): {codesNoImages}";
                        this.missingRecordLogs.Add(noImagesMsg);
                    }

                    //write the error log file
                    string logFileName = "missingRecords.log";
                    System.IO.File.WriteAllLines(Path.Combine(this.workingDir, logFileName), this.missingRecordLogs.ToArray());

                }
                else
                {
                    MessageBox.Show("Images folder does not exist. Please select a valid images folder");
                    return;
                }
            }
            else
            {
                MessageBox.Show("No images folder selected. Captured records cannot be checked against images.");
            }
        }

        private void btnCheckDuplicates_Click(object sender, EventArgs e)
        {
            RecordCleaner.removeBarcodeExclamations(this.records);

            int duplicatesRemoved = RecordCleaner.removeDuplicates(this.records);

            if (!this.dupsSearched)
            {
                this.findBotRecDuplicates(); //generates the list of duplicates. Next step is to process them
            }

            MessageBox.Show($"{duplicatesRemoved} identical duplicate records removed");

            this.btnNextDuplicate.Enabled = true;

            this.btnCheckDuplicates.Enabled = false;

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
                INSearch = INSearch.Substring(0, INSearch.Length - 2); //remove the last comma

                string expression = $"barcode IN ({INSearch})";

                this.viewRecordsBinding.Filter = expression;

                if(this.dgvRecordsView.DataSource == null)
                {
                    this.dgvRecordsView.DataSource = viewRecordsBinding;
                }

                this.botRecDuplicatesIndex++;

                this.lblDupIndexCount.Text = $"{this.botRecDuplicatesIndex} of {this.botRecDuplicates.Count} duplicates";
            }
            else
            {
                this.dgvRecordsView.DataSource = null;
                this.viewRecordsBinding.Filter = null;
                this.btnCheckDuplicates.Enabled = true;
                this.btnNextDuplicate.Enabled = false;
                this.btnMergeDups.Enabled = false;
                this.btnDeleteRows.Enabled = false;
                this.lblDupIndexCount.Text = "";

                this.btnCleanRecords.Enabled = true;

                MessageBox.Show("All duplicates have been processed.");
                
            }
        }

        private void btnFindErrors_Click(object sender, EventArgs e)
        {
            this.errorsBinding = new BindingList<string>(this.rowErrors);
            //search for the next error if false
            if (this.findNextRowWithErrors(0))
            {
                this.btnNextRowWithErrors.Enabled = true;
                this.viewRecordsBinding.DataSource = this.records.Rows[this.errorRecordIndex];
            }
            else
            {
                MessageBox.Show("No errors found");
                this.btnAddQDSFromCoords.Enabled = true;
            }
        }

        private void btnNextRowWithErrors_Click(object sender, EventArgs e)
        {
            if (this.checkingQDSErrorsOnly)
            {
                bool errorFound = this.findNextCountryQDSError(this.errorRecordIndex + 1);
                if (!errorFound)
                {
                    MessageBox.Show("no more errors found");
                }
            }
            else
            {
                bool errorFound = this.findNextRowWithErrors(this.errorRecordIndex + 1);
                if (!errorFound)
                {
                    MessageBox.Show("no more errors found");
                    this.btnAddQDSFromCoords.Enabled = true;
                }
            }
        }

        private void btnTestSomething_Click(object sender, EventArgs e)
        {
            this.btnChooseQDSCountries_Click(sender, e);
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
            this.btnFindErrors.Enabled = true;

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

            this.checkingQDSErrorsOnly = true;
            //search for the next error if false
            if (findNextCountryQDSError(0))
            {
                this.viewRecordsBinding.DataSource = this.records.Rows[this.errorRecordIndex];
                this.errorsBinding = new BindingList<string>(this.rowErrors);
            }
            else
            {
                this.checkingQDSErrorsOnly = false;
                MessageBox.Show("No QDS errors found");
            }

        }

        //save the corrected dataset
        private void btnSaveChanges_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "dbf files (*.dbf)|*.dbf";
            saveFileDialog1.Title = "Save your changes";
            saveFileDialog1.OverwritePrompt = true;
            saveFileDialog1.ShowDialog();

            // If the file name is not an empty string open it for saving.  
            if (saveFileDialog1.FileName != "")
            {
                //copy a file, clear it, and write all the new records to it. 
                string sourceFile = this.txtChooseDBF.Text.Split(';')[0].Trim();

                try
                {
                    // Will not overwrite if the destination file already exists.
                    File.Copy(Path.Combine(this.workingDir, sourceFile), saveFileDialog1.FileName, true);

                    //we also need to copy the fpt file
                    string sourceFPT = sourceFile.ToLower().Replace(".dbf", ".fpt");
                    string destFPT = saveFileDialog1.FileName.ToLower().Replace(".dbf", ".fpt");
                    File.Copy(Path.Combine(this.workingDir, sourceFPT), destFPT, true);
                }

                // Catch exception if the file was already copied.
                catch (IOException copyError)
                {
                    MessageBox.Show($"Error with saving file. Copy template failed with message: {copyError.Message}");
                    return;
                }

                //connect to the new file
                string fileNameOnly = Path.GetFileName(saveFileDialog1.FileName);
                string ext = Path.GetExtension(saveFileDialog1.FileName);
                string tableName = fileNameOnly.Replace(ext, "");

                string connectionString = @"Provider=VFPOLEDB.1;Data Source=" + saveFileDialog1.FileName;
                string deleteSQL = $"DELETE FROM {tableName}";

                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {

                    try
                    {
                        connection.Open();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error with database connection: " + ex.Message);
                        return;
                    }

                    //get the file schema
                    OleDbCommand cm = new OleDbCommand($"select top 5 * from {tableName} order by barcode", connection);
                    OleDbDataReader myReader = cm.ExecuteReader();
                    DataTable schemaTable = myReader.GetSchemaTable(); ;
                    

                    //clear all records
                    OleDbCommand deleteCmd = new OleDbCommand(deleteSQL, connection);
                    try
                    {
                        int deletedRecordCount = deleteCmd.ExecuteNonQuery();
                        //MessageBox.Show($"Records deleted: {deletedRecordCount}");
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show($"Error with saving file. Empty template failed with message: {ex.Message}");
                        return;
                    }

                    //add the new records
                    OleDbCommand addRecordCmd = new OleDbCommand();
                    addRecordCmd.Connection = connection;

                    List<string> columnNames = new List<string>();
                    foreach(DataColumn col in this.records.Columns)
                    {
                        columnNames.Add(col.ColumnName.Trim());
                    }

                    string joinedColNames = String.Join(", ", columnNames.ToArray());

                    //we need the list of parameter spaceholders for the sql statement
                    List<string> placeholders = new List<string>();
                    for (int i = 0; i < columnNames.Count; i++)
                    {
                        placeholders.Add("?");
                    }

                    string joinedPlaceholders = String.Join(", ", placeholders.ToArray());

                    string addSQL = $"INSERT INTO {tableName} ({joinedColNames}) VALUES ({joinedPlaceholders})";

                    OleDbCommand insertCommand = new OleDbCommand(addSQL, connection);
                    int counter = 0; // this is just for keeping track during debugging
                    foreach (DataRow row in this.records.Rows)
                    {
                        counter++;
                        insertCommand.Parameters.Clear(); // clean everything out
                        
                        foreach (string colName in columnNames)
                        {
                            //get the type for this column
                            DataRow schema = schemaTable.Select($"ColumnName = '{colName}'")[0];
                            string coltype = schema["DataType"].ToString();
                            bool isLong = (bool)schema["IsLong"];

                            OleDbType OleColType;
                            if (coltype == "System.String")
                            {
                                if (isLong)
                                {
                                    OleColType = OleDbType.LongVarChar;
                                    insertCommand.Parameters.Add(colName, OleColType).Value = row[colName].ToString().Trim();
                                }
                                else
                                {
                                    OleColType = OleDbType.Char;
                                    insertCommand.Parameters.Add(colName, OleColType).Value = row[colName].ToString().Trim();
                                }
                            }
                            else if(coltype == "System.Decimal")
                            {
                                OleColType = OleDbType.Decimal;
                                string val = row[colName].ToString().Trim();
                                if (String.IsNullOrEmpty(val))
                                {
                                    insertCommand.Parameters.Add(colName, OleColType).Value = OleDbType.Empty;
                                }
                                else
                                {
                                    insertCommand.Parameters.Add(colName, OleColType).Value = decimal.Parse(val);
                                }
                                
                            }
                            else if (coltype == "System.Boolean")
                            {
                                OleColType = OleDbType.Boolean;
                                string val = row[colName].ToString().Trim();
                                if (String.IsNullOrEmpty(val))
                                {
                                    insertCommand.Parameters.Add(colName, OleColType).Value = OleDbType.Empty;
                                }
                                else
                                {
                                    insertCommand.Parameters.Add(colName, OleColType).Value = bool.Parse(val);
                                }
                            }
                            else if (coltype == "System.DateTime")
                            {
                                OleColType = OleDbType.DBDate;
                                string val = row[colName].ToString().Trim();
                                if (String.IsNullOrEmpty(val))
                                {
                                    insertCommand.Parameters.Add(colName, OleColType).Value = null;
                                }
                                else
                                {
                                    insertCommand.Parameters.Add(colName, OleColType).Value = DateTime.Parse(val);
                                }
                            }
                            else
                            {
                                throw new Exception("unmatched datatype in dbf file");
                            }

                        }

                        try
                        {
                            insertCommand.ExecuteNonQuery();
                        }
                        catch (OleDbException ex)
                        {
                            MessageBox.Show($"Error saving file. Could not add row with message {ex.Message}");
                            return;
                        }
                    }

                    //if we get here, it worked!!
                    connection.Close();
                    MessageBox.Show("Records successfully saved");
                    
                }
            }
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

                //sometimes there is no barcode
                if (String.IsNullOrEmpty(barcode))
                {
                    continue;
                }

                string root = barcode.Split(seps)[0];
                //sometimes we have a, b, etc at the end
                if(char.IsLetter(root[root.Length - 1]))
                {
                    root = root.Substring(0, root.Length - 1);
                }

                // check if we've already processed this root
                int cnt = this.botRecDuplicates.Where(x => x.root == root).Count();

                if (cnt == 0)
                {
                    DataRow[] dups = this.records.Select($"TRIM(barcode) like '{root}*'");

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
                else
                {
                    continue;
                }

            }

            this.dupsSearched = true;
        }

        private bool findNextRowWithErrors(int startIndex)
        {
            this.rowErrors.Clear();

            if (startIndex < this.records.Rows.Count)
            {
                for (int i = startIndex; i < this.records.Rows.Count; i++)
                {

                    DataRow row = this.records.Rows[i];

                    if (!RecordErrorFinder.numberIsAnIntegerOrSN(row))
                    {
                        this.rowErrors.Add(RecordErrors.collNumberError);
                    }

                    if (!RecordErrorFinder.countryInList(row, this.CountryCodes))
                    {
                        this.rowErrors.Add(RecordErrors.countryInvalid);
                    }

                    if (!RecordErrorFinder.majorAreaIsEmpty(row))
                    {
                        this.rowErrors.Add(RecordErrors.majorareaEmpty);
                    }

                    if (!RecordErrorFinder.minorAreaIsEmpty(row))
                    {
                        this.rowErrors.Add(RecordErrors.minorareaEmpty);
                    }

                    if (!RecordErrorFinder.localityIsEmpty(row))
                    {
                        this.rowErrors.Add(RecordErrors.localityEmpty);
                    }

                    if (!RecordErrorFinder.higherTaxaAllPresent(row))
                    {
                        this.rowErrors.Add(RecordErrors.higherTaxaMissing);
                    }

                    string ranksNotInBackbone = RecordErrorFinder.getRanksNotInBackbone(row, this.taxa);
                    if (!String.IsNullOrEmpty(ranksNotInBackbone))
                    {
                        this.rowErrors.Add($"{RecordErrors.ranksNotInBackbone}: {ranksNotInBackbone}");
                    }

                    string coordsErrors = RecordErrorFinder.getCoordErrors(row);
                    if (!String.IsNullOrEmpty(coordsErrors))
                    {
                        this.rowErrors.Add($"{RecordErrors.coordinateErrors}: {coordsErrors}");
                    }

                    if (!RecordErrorFinder.isQDSValid(row))
                    {
                        this.rowErrors.Add(RecordErrors.qdsInvalid);
                    }

                    if (!RecordErrorFinder.QDSValidForCountry(row, this.CountryQDSs))
                    {
                        this.rowErrors.Add(RecordErrors.qdsNotValidForCountry);
                    }
                    else //TEST FOR THE QDS - COORDINATES MATCH HERE
                    {
                        if (String.IsNullOrEmpty(coordsErrors))
                        {
                            if (!RecordErrorFinder.coordsMatchQDS(row))
                            {
                                this.rowErrors.Add(RecordErrors.qdsCoordsMismatch);
                            }
                        }
                    }

                    string collectorsNotIn = RecordErrorFinder.getCollectorsNotInList(row, this.people);
                    if (!String.IsNullOrEmpty(collectorsNotIn))
                    {
                        this.rowErrors.Add("The following collectors are not in the list: " + collectorsNotIn);
                    }

                    if (!RecordErrorFinder.isDeterminerInList(row, this.people))
                    {
                        this.rowErrors.Add("The determiner is not in the list");
                    }


                    //check if we found errors and break if we did
                    if (this.rowErrors.Count > 0)
                    {
                        this.errorRecordIndex = i;
                        return true;
                    }
                }

                //if we reach the end without finding errors
                return false;
            }
            else
            {
                return false;
            }

        }

        private bool findNextCountryQDSError(int startIndex)
        {
            this.rowErrors.Clear();

            //loop through till we find an error
            for (int i = startIndex; i < this.records.Rows.Count; i++)
            {
                DataRow row = this.records.Rows[i];
                if (!RecordErrorFinder.QDSValidForCountry(row, this.CountryQDSs))
                {
                    this.errorRecordIndex = i;
                    this.rowErrors.Add(RecordErrors.qdsNotValidForCountry);
                    break;
                }
            }

            //return whether or not there was an error found
            if (this.rowErrors.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
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

        private Dictionary<string, List<string>> CountryQDSs { get; set; }
        public CountryCodes CountryCodes { get; set; }
        private string[] columnNames { get; set; }
        private List<string> imagePaths { get; set; }


        //error checking
        bool checkingQDSErrorsOnly { get; set; }
        int errorRecordIndex { get; set; }
        List<string> rowErrors { get; set; }
        private BindingList<string> errorsBinding { get; set; }

        //reporting
        private BindingList<string> fileNamesBinding { get; set; }
        

        private List<string> RDEUIErrors { get; set; }
        
        private List<string> missingRecordLogs { get; set; }


        private List<string> RDEcleaning { get; set; }
        private BindingList<string> cleaningBinding { get; set; }
        private List<string> cleaningLogs { get; set; }

        //errors
        private List<DataRow> localityWithValues { get; set; }
        private List<DataRow> majorareaWithValues { get; set; }
        private List<DataRow> minorareaWithValues { get; set; }

        
    }
}
