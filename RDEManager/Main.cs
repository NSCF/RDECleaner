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
using GenericParsing;


namespace RDEManager
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();

            this.dupsSearched = false;

            this.CountryCodes = new CountryCodes();

            this.rowErrors = new List<string>();

            this.checkingQDSErrorsOnly = false;

            this.peopleChecked = new List<string>();

            this.records = new DataTable();
            this.schemasAccepted = new List<DataTable>();

            this.existingRecords = new DataTable();
            this.existingBarcodes = new HashSet<string>();
            this.existingBarcodeIndices = new List<int>();

        }

        //UI FUNCTIONS

        private void btnChooseSchema_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "dbf files (*.dbf)|*.dbf";
                openFileDialog.FilterIndex = 1;
                openFileDialog.Multiselect = false;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string fileName = openFileDialog.FileName;
                    string directory = Path.GetDirectoryName(fileName);
                    this.templateDir = directory;
                    fileName = Path.GetFileName(fileName);
                    string fileExtenstion = Path.GetExtension(fileName);
                    string tableName = fileName.Replace(fileExtenstion, "");


                    //get the schema
                    string connectionString = @"Provider=VFPOLEDB.1;Data Source=" + directory + "\\" + fileName;
                    string selectSQL = "select * from [" + tableName + "]";
                    string deleteSQL = "delete from [" + tableName + "]";
                    using (OleDbConnection connection = new OleDbConnection(connectionString))
                    using (OleDbCommand command = new OleDbCommand(selectSQL, connection))
                    using (OleDbCommand deleteCommand = new OleDbCommand(deleteSQL, connection))
                    {
                        try
                        {
                            connection.Open();

                            //create the reader
                            OleDbDataReader reader = command.ExecuteReader();

                            DataTable schemaTable = reader.GetSchemaTable();
                            this.templateSchema = schemaTable;
                            this.templateColNames = new List<string>();
                            this.templateColTypes = new List<string>();
                            foreach (DataRow row in schemaTable.Rows)
                            {
                                this.templateColNames.Add(row["ColumnName"].ToString());
                                this.templateColTypes.Add(row["DataType"].ToString());
                            }

                            //load the schema to create an initial empty dataset
                            try
                            {
                                this.records.Load(reader);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Error loading the schema: " + ex.Message);
                                return;
                            }

                            //delete any records that might be lurking in the template schema
                            try
                            {
                                deleteCommand.ExecuteNonQuery();
                            }
                            catch(Exception ex)
                            {
                                MessageBox.Show("Error clearning records from template: " + ex.Message);
                                return;
                            }

                            txtSchemaFile.Text = fileName;
                            lblSchemaFields.Text = $"{templateColNames.Count} fields";
                            btnChooseDBF.Enabled = true;

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error reading the template file: " + ex.Message);
                        }
                    }
                }
            }
        }

        private void btnChooseDBF_Click(object sender, EventArgs e)
        {

            if (this.filesToImport == null)
            {
                this.filesToImport = new List<string>();
            }
            else
            {
                this.filesToImport.Clear();
            }

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "dbf files (*.dbf)|*.dbf";
                openFileDialog.FilterIndex = 1;
                openFileDialog.Multiselect = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {

                    this.workingDir = Path.GetDirectoryName(openFileDialog.FileNames[0]);

                    for (int i = 0; i < openFileDialog.FileNames.Length; i++)
                    {
                        this.filesToImport.Add(Path.GetFileName(openFileDialog.FileNames[i]));
                    }

                    if(this.filesToImport.Count < 5)
                    {
                        string allParts = string.Join(" | ", this.filesToImport.ToArray());
                        txtChooseDBF.Text = allParts;
                    }
                    else
                    {
                        txtChooseDBF.Text = $"{this.filesToImport.Count} RDE files selected";
                    }

                    btnAddDBFData.Enabled = true;

                }
            }

        }

        private void addFile_Click(object sender, EventArgs e)
        {
            
            this.tablesNotAdded = new List<string>();


            for (int i = 0; i < this.filesToImport.Count; i++)
            {
                addRDERecords(this.filesToImport[i]);
            }

            if (this.tablesNotAdded.Count > 0)
            {
                string joinedTableNames = String.Join("; ", this.tablesNotAdded.ToArray());
                MessageBox.Show($"The following tables could not be added due to errors or schema violations: {joinedTableNames}");
            }

            MessageBox.Show("Finished importing records");

            btnClearRecords.Enabled = true;
            btnChooseImageFolder.Enabled = true;
            btnChooseTaxonBackbone.Enabled = true;
            btnChooseQDSCountries.Enabled = true;
            btnChoosePeople.Enabled = true;
            btnCheckDuplicates.Enabled = true;
            btnFindCaptureErrors.Enabled = true;

        }

        private void btnClearRecords_Click(object sender, EventArgs e)
        {

            string confirm = "Are you sure you want to clear all records? Any unsaved changes will be lost.";
            var confirmResult = MessageBox.Show(confirm,
                                     "Confirm Clear Records!!",
                                     MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                this.records.Clear();
                this.txtChooseDBF.Clear();
                lblNumberOfRecords.Text = "Number of records: ";
                btnClearRecords.Enabled = false;
                btnFindCaptureErrors.Enabled = false;
            }
            else
            {
                return;
            }
        }

        private void btnAddExistingBarcodes_Click(object sender, EventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Choose the existing barcodes file (csv)";

            using (openFileDialog)
            {

                openFileDialog.Filter = "csv files (*.csv)|*.csv";
                openFileDialog.FilterIndex = 1;

                if (openFileDialog.ShowDialog() == DialogResult.OK && openFileDialog.FileName != null)
                {

                    this.cbExistingBarcodeField.Items.Clear();

                    try
                    {
                        using (GenericParserAdapter parser = new GenericParserAdapter(openFileDialog.FileName))
                        {
                            parser.FirstRowHasHeader = true;
                            this.existingRecords = parser.GetDataTable();

                        }
                    }
                    catch(IOException ex)
                    {
                        MessageBox.Show("Please make sure the existing barcodes file is closed before adding it here");
                    }                    

                    this.cbExistingBarcodeField.Items.Add("--select--");
                    this.cbExistingBarcodeField.SelectedIndex = 0;

                    foreach (DataColumn col in this.existingRecords.Columns)
                    {
                        this.cbExistingBarcodeField.Items.Add(col.ColumnName);
                    }

                    cbExistingBarcodeField.Enabled = true;
                }
            }
        }

        private void cbExistingBarcodeField_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cbExistingBarcodeField.SelectedIndex > 0)
            {
                string barcodeField = cbExistingBarcodeField.SelectedItem.ToString();
                foreach (DataRow row in this.existingRecords.Rows)
                {
                    this.existingBarcodes.Add(row[barcodeField].ToString().Trim());
                }

                foreach (string barcode in this.existingBarcodes)
                {
                    for (int i = 0; i < this.records.Rows.Count; i++)
                    {
                        if (this.records.Rows[i]["barcode"].ToString().Trim().ToLower() == barcode.Trim().ToLower())
                        {
                            this.existingBarcodeIndices.Add(i);
                        }
                    }
                }

                lblExistingBarcodeCount.Text = "Existing barcodes recaptured: " + this.existingBarcodeIndices.Count;

                btnRemoveExistingBarcodes.Enabled = true;

            }            
        }

        private void btnChooseImageFolder_Click(object sender, EventArgs e)
        {

            if (this.records != null && this.records.Rows.Count > 0)
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
            else
            {
                MessageBox.Show("Make sure to import the RDE files before continuing");
            }

        }

        private void btnChooseTaxonBackbone_Click(object sender, EventArgs e)
        {

            this.taxa = new DataTable();

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Choose the taxon backbone file";

            using (openFileDialog)
            {

                this.modalMessage = "Please wait while taxa are imported";
                Form wait = new PleaseWait(this);
                wait.Show();

                openFileDialog.Filter = "dbf files (*.dbf)|*.dbf";
                openFileDialog.FilterIndex = 1;

                if (openFileDialog.ShowDialog() == DialogResult.OK && openFileDialog.FileName != null)
                {

                    string taxonFile = Path.GetFileName(openFileDialog.FileName);
                    string taxonDirectory = Path.GetDirectoryName(openFileDialog.FileName);

                    try
                    {
                        readDBFTable(taxonDirectory, taxonFile, this.taxa, false);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error reading taxon backbone file: " + ex.Message);
                        return;
                    }

                    MessageBox.Show("Taxon file successfully read");
                    btnCheckTaxa.Enabled = true;
                    txtTaxonBackbone.Text = taxonFile;
                    lblNoTaxa.Text = $"No. of taxon records:  {this.taxa.Rows.Count}";

                }

                wait.Close();
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
                            parser.TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.Delimited;
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
                    this.lblQDSCountries.Text = $"No. of countries with QDS lists: {this.CountryQDSs.Count}";
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

                this.modalMessage = "Please wait while people are imported";
                Form wait = new PleaseWait(this);
                wait.Show();

                openFileDialog.Filter = "dbf files (*.dbf)|*.dbf";
                openFileDialog.FilterIndex = 1;

                if (openFileDialog.ShowDialog() == DialogResult.OK && openFileDialog.FileName != null)
                {

                    string peopleFile = Path.GetFileName(openFileDialog.FileName);
                    string peopleDirectory = Path.GetDirectoryName(openFileDialog.FileName);

                    try
                    {
                        readDBFTable(peopleDirectory, peopleFile, this.people, false);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error reading people table: " + ex.Message);
                        return;
                    }

                    txtPeopleTable.Text = peopleFile;
                    this.lblPeople.Text = $"No. of agent records: {this.people.Rows.Count}";

                    MessageBox.Show("People table successfully imported");


                }

                wait.Close();
            }
        }

        private void btnRemoveExistingBarcodes_Click(object sender, EventArgs e)
        {

            this.existingBarcodeIndices.Sort();

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "dbf files (*.dbf)|*.dbf";
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName != "")
            {

                DataTable copyOfExistingRecords = this.records.Clone();
                copyOfExistingRecords.Columns["dcnotes"].MaxLength = 500;

                for (int i = this.existingBarcodeIndices.Count - 1; i >= 0; i--)
                {
                    //trim all those nasty padded Brahms strings so we don't get schema violations
                    int recordIndex = existingBarcodeIndices[i];
                    object[] recordData = this.records.Rows[recordIndex].ItemArray;
                    for(int j = 0; j < recordData.Length; j++)
                    {
                        if(recordData[j] is System.String)
                        {
                            recordData[j] = recordData[j].ToString().Trim();
                        }
                    }
                    
                    copyOfExistingRecords.Rows.Add(recordData);
                    this.records.Rows.RemoveAt(recordIndex);
                }
                this.records.AcceptChanges();

                saveTableAsDBF(saveFileDialog.FileName, copyOfExistingRecords);
            }
            else {
                MessageBox.Show("Existing records must be saved before they can be removed from the captured dataset");
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
                        string ext = Path.GetExtension(fileName).ToLower();
                        return ext == ".jpg" || ext == ".tif";
                    }).ToList();

                    //remove any in the existingRecords list
                    if(this.existingBarcodes.Count > 0)
                    {
                        imageFiles = imageFiles.Where(fileName =>
                        {
                            string filePartLower = fileName.Replace(Path.GetExtension(fileName), "").ToLower();
                            return !existingBarcodes.Any(barcode => barcode.ToLower() == filePartLower);
                        }).ToList();
                    }

                    List<string>[] errors = RecordErrorFinder.checkAllImagesCaptured(this.records, imageFiles);

                    List<string> notCaptured = errors[0];
                    List<string> noImages = errors[1];

                    //for simplicity filter out any that might be in existingBarcodes
                    notCaptured = notCaptured.Where(barcode => !this.existingBarcodes.Contains(barcode)).ToList();
                    noImages = noImages.Where(barcode => !this.existingBarcodes.Contains(barcode)).ToList();

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

            /*
            try
            {
                int duplicatesRemoved = RecordCleaner.removeDuplicates(this.records);
                MessageBox.Show($"{duplicatesRemoved} identical duplicate records removed");
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Error removing identical duplicates. They will have to be removed manually");
            }
            */

            if (!this.dupsSearched)
            {
                this.modalMessage = "Please wait while non-identical duplicates are located";
                Form wait = new PleaseWait(this);
                wait.Show();
                this.findBotRecDuplicates(); //generates the list of duplicates. Next step is to process them
                wait.Close();
            }

            //set the bndings for the grid view
            if (this.botRecDuplicates.Count > 0)
            {
                MessageBox.Show($"{this.botRecDuplicates.Count} duplicate records need to be processed");
                btnShowImage.Enabled = true;
                this.showNextDuplicate();
                this.btnNextDuplicate.Enabled = true;
                this.btnMergeDups.Enabled = true;
                this.btnDeleteRows.Enabled = true;
                this.btnCheckDuplicates.Enabled = false;
            }
            else
            {
                MessageBox.Show($"No duplicate records need to be processed. Proceed to updates.");
                this.btnUpdateMissingData.Enabled = true;
            }

        }

        private void btnFindNextDuplicate_Click(object sender, EventArgs e)
        {

            if (this.botRecDuplicatesIndex < this.botRecDuplicates.Count)
            {
                this.showNextDuplicate();
            }
            else
            {
                this.dgvRecordsView.DataSource = null;
                this.viewRecordsBinding.Filter = null;
                this.btnCheckDuplicates.Enabled = true;
                this.btnNextDuplicate.Enabled = false;
                this.btnMergeDups.Enabled = false;
                this.btnDeleteRows.Enabled = false;
                this.lblDupsCount.Text = "";

                this.btnUpdateMissingData.Enabled = true;

                MessageBox.Show("All duplicates have been processed. Proceed to updates.");

            }
        }

        private void btnCheckTaxa_Click(object sender, EventArgs e)
        {
            if (this.taxa == null || this.taxa.Rows.Count == 0)
            {
                MessageBox.Show("No taxon table available to check taxa");
                return;
            }
            else
            {
                Dictionary<string, List<string>> taxaNotFound = RecordErrorFinder.getTaxonNamesNotInBackbone(this.records, this.taxa);

                if (taxaNotFound.Count > 0)
                {
                    string printErr = $"Taxa not found:{Environment.NewLine}";
                    foreach (string key in taxaNotFound.Keys)
                    {
                        if (taxaNotFound[key].Count > 0)
                        {
                            printErr += $"{key} ({taxaNotFound[key].Count}): {String.Join("; ", taxaNotFound[key].ToArray())} {Environment.NewLine}";
                        }
                    }
                    rtbReportErrors.Clear();
                    rtbReportErrors.Text = printErr;
                }
                else
                {
                    MessageBox.Show("all taxa are correct");
                }
            }
        }

        private void btnFindCaptureErrors_Click(object sender, EventArgs e)
        {

            if (this.people != null && this.people.Rows.Count > 0)
            {
                this.errorsBinding = new BindingList<string>(this.rowErrors);
                //search for the next error if false
                if (this.findNextRowWithCaptureErrors(0))
                {
                    this.btnNextRowWithErrors.Enabled = true;
                    btnDeleteRows.Enabled = true;
                    btnShowImage.Enabled = true;

                    this.showNextError();
                }
                else
                {
                    MessageBox.Show("No errors found");
                    this.btnUpdateMissingData.Enabled = true;
                    btnDeleteRows.Enabled = false;
                    btnShowImage.Enabled = false;
                    this.btnNextRowWithErrors.Enabled = false;
                }
            }
            else
            {
                MessageBox.Show("Checking errors requires a people table. Please import it and then proceed");
            }


        }

        private void btnFindQDSCoordErrors_Click(object sender, EventArgs e)
        {
            this.checkingQDSErrorsOnly = true;
            this.errorsBinding = new BindingList<string>(this.rowErrors);
            //search for the next error if false
            if (this.findNextRowWithQDSCoordErrors(0))
            {
                this.btnNextRowWithErrors.Enabled = true;
                btnDeleteRows.Enabled = true;
                btnShowImage.Enabled = true;

                this.showNextError();
            }
            else
            {
                MessageBox.Show("No errors found");
            }
        }

        private void btnShowImage_Click(object sender, EventArgs e)
        {

            if (!String.IsNullOrEmpty(this.txtImageFolder.Text.Trim()))
            {

                if (dgvRecordsView.RowCount == 1 || dgvRecordsView.SelectedRows.Count > 0)
                {
                    List<string> barcodes = new List<string>();
                    bool allRowsWithBarcodes = true;
                    if (dgvRecordsView.SelectedRows.Count > 0)
                    {
                        foreach (DataGridViewRow row in dgvRecordsView.SelectedRows)
                        {
                            string barcode = row.Cells["barcode"].Value.ToString().Trim();
                            if (!string.IsNullOrEmpty(barcode))
                            {
                                barcodes.Add(barcode);
                            }
                            else
                            {
                                allRowsWithBarcodes = false;
                            }
                        }

                        if (!allRowsWithBarcodes)
                        {
                            MessageBox.Show("Some selected rows are missing valid barcodes");
                        }
                    }
                    else
                    {
                        string barcode = dgvRecordsView.Rows[0].Cells["barcode"].Value.ToString().Trim();
                        if (!string.IsNullOrEmpty(barcode))
                        {
                            barcodes.Add(barcode);
                        }
                        else
                        {
                            MessageBox.Show("Record is missing a valid barcode");
                        }
                    }

                    if (barcodes.Count > 0)
                    {
                        //get the files if we don't have them already
                        if (this.imagePaths == null)
                        {
                            this.imagePaths = new List<string>();
                            GetFilesRecursive(this.txtImageFolder.Text, imagePaths); //get a list of all the file names
                        }

                        List<string> filePathsToShow = new List<string>();
                        List<string> barcodesWithNoFiles = new List<string>();
                        foreach (string barcode in barcodes)
                        {
                            List<string> foundFilePaths = imagePaths.Where(x => x.Contains($"{barcode}.jpg")).ToList();
                            if (foundFilePaths.Count > 0)
                            {
                                filePathsToShow.AddRange(foundFilePaths);
                            }
                            else
                            {
                                barcodesWithNoFiles.Add(barcode);
                            }
                        }

                        //make sure filePaths are unique
                        filePathsToShow = filePathsToShow.Select(x => x).Distinct().ToList();

                        foreach (string filePath in filePathsToShow)
                        {
                            System.Diagnostics.Process.Start(filePath); //opens the file
                        }

                        if (barcodesWithNoFiles.Count > 0)
                        {
                            MessageBox.Show("Image files not found for the following images: " + String.Join(";", barcodesWithNoFiles.ToArray()));
                        }
                    }
                }
                else
                {
                    MessageBox.Show("No records selected");
                    return;
                }
            }
            else
            {
                MessageBox.Show("The images folder has not been selected.");
            }
        }

        private void btnNextRowWithErrors_Click(object sender, EventArgs e)
        {
            btnNextRowWithErrors.Enabled = false;
            btnConfirmPeople.Enabled = false;

            if (this.checkingQDSErrorsOnly)
            {
                bool errorFound = this.findNextRowWithQDSCoordErrors(this.errorRecordIndex + 1);
                if (!errorFound)
                {
                    MessageBox.Show("no more errors found");
                }
                else
                {
                    this.btnNextRowWithErrors.Enabled = true;
                    btnDeleteRows.Enabled = true;
                    btnShowImage.Enabled = true;
                    btnNextRowWithErrors.Enabled = true;
                    this.showNextError();
                }
            }
            else
            {
                if (this.findNextRowWithCaptureErrors(this.errorRecordIndex + 1))
                {
                    btnNextRowWithErrors.Enabled = true;
                    this.showNextError();
                }
                else
                {
                    this.rtbReportErrors.Clear();
                    this.btnNextRowWithErrors.Enabled = false;
                    this.btnDeleteRows.Enabled = false;
                    this.btnShowImage.Enabled = false;
                    this.lblErrorRowIndex.Visible = false;
                    MessageBox.Show("no more errors found");
                }
            }
        }

        private void btnConfirmPeople_Click(object sender, EventArgs e)
        {
            //we need to parse the error list, get the names out again and put them in peopleChecked
            List<string> namesToAdd = new List<string>();
            string collectorErrors = this.rowErrors.Find(x => x.StartsWith("The following collectors are not in the master list: "));
            if (!String.IsNullOrEmpty(collectorErrors))
            {
                string collectorNamesJoined = collectorErrors.Replace("The following collectors are not in the master list: ", "").Trim();
                List<string> splitNames = collectorNamesJoined.Split(';').ToList().Select(x => x.Trim()).ToList(); //split and trim
                namesToAdd.AddRange(splitNames);
            }
           
            string detnameError = this.rowErrors.Find(x => x.StartsWith("The determiner is not in the master list: "));
            if (!String.IsNullOrEmpty(detnameError))
            {
                string detname = detnameError.Replace("The determiner is not in the master list: ", "").Trim();
                namesToAdd.Add(detname);
            }

            if(namesToAdd.Count > 0)
            {
                this.peopleChecked.AddRange(namesToAdd);
            }

            btnConfirmPeople.Enabled = false;

        }

        private void btnTestSomething_Click(object sender, EventArgs e)
        {
            this.btnChooseQDSCountries_Click(sender, e);
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
                        foreach (string dupBarcode in dupInfo.dupBarcodes)
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
                    foreach (DataGridViewRow row in this.dgvRecordsView.SelectedRows)
                    {
                        this.dgvRecordsView.Rows.RemoveAt(row.Index);
                    }
                    this.records.AcceptChanges();
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

        private void btnUpdateMissingData_Click(object sender, EventArgs e)
        {
            bool updateWho = false;
            if (chkAddOrganization.Checked)
            {
                if (String.IsNullOrEmpty(txtAddWho.Text))
                {
                    MessageBox.Show("A value is required to update Who");
                    return;
                }
                else
                {
                    updateWho = true;
                }
            }

            List<string> updateReport = new List<string>();

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
                updateReport.Add($"Collector number updated for {collNumUpdates} records{Environment.NewLine}");
            }

            int dupsUpdated = RecordCleaner.updateDups(this.records);

            if (dupsUpdated > 0)
            {
                updateReport.Add($"Dups updated for {dupsUpdated} records{Environment.NewLine}");
            }

            int countriesUpdated = RecordCleaner.addCountries(this.records);
            if (countriesUpdated > 0)
            {
                updateReport.Add($"Countries added for {countriesUpdated} records{Environment.NewLine}");
            }

            if (updateWho)
            {
                RecordCleaner.updateWHO(this.records, txtAddWho.Text);
            }

            int updatedAccessionNumbers = RecordCleaner.addAccessionNumbers(this.records);
            if (updatedAccessionNumbers > 0)
            {
                updateReport.Add($"Accession numbers updated for {updatedAccessionNumbers} records{Environment.NewLine}");
            }

            int updatedCoordinates = RecordCleaner.clearZeroCoordinates(this.records);
            if (updatedCoordinates > 0)
            {
                updateReport.Add($"Zero coordinates cleared for {updatedCoordinates} records{Environment.NewLine}");
            }

            int updatedImagePaths = RecordCleaner.updateImageList(this.records, this.imagePaths);
            if (updatedImagePaths > 0)
            {
                updateReport.Add($"Image list updated for {updatedImagePaths} records{Environment.NewLine}");
            }

            //TODO add the process to add locality details

            int qdsUpdates = RecordCleaner.addQDSFromCoordinates(this.records);
            if (qdsUpdates > 0)
            {
                updateReport.Add($"QDS updated for {qdsUpdates} records");
            }

            //some formatting and the last message
            updateReport.Add(Environment.NewLine);
            updateReport.Add(Environment.NewLine);
            updateReport.Add("Record updates complete.");

            string message = String.Join("", updateReport.ToArray());

            MessageBox.Show(message);

            this.btnUpdateMissingData.Enabled = false;
            this.btnFindQDSCoordErrors.Enabled = true;

        }

        //save the corrected dataset
        private void btnSaveChanges_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "dbf files (*.dbf)|*.dbf";
            saveFileDialog.Title = "Save your changes";
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.ShowDialog();

            saveTableAsDBF(saveFileDialog.FileName, this.records);
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

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit? Any unsaved changes will be lost.",
                               "RDE Data Cleaner",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Information) == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        //HELPER FUNCTIONS

        private void readDBFTable(string directory, string fileName, DataTable target, bool RDESchemaCheck)
        {
            string fileExtenstion = Path.GetExtension(fileName);
            string tableName = fileName.Replace(fileExtenstion, "");

            string connectionString = @"Provider=VFPOLEDB.1;Data Source=" + directory + "\\" + fileName;
            string selectSQL = "select * from [" + tableName + "]";
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            using (OleDbCommand command = new OleDbCommand(selectSQL, connection))
            {
                try
                {
                    connection.Open();
                }
                catch(Exception ex)
                {
                    MessageBox.Show($"Error opening database connections for {tableName}: {ex.Message}");
                    this.tablesNotAdded.Add($"{tableName}: database connection could not be opened with message '{ex.Message}'");
                    return;
                }

                //create the reader
                OleDbDataReader reader = command.ExecuteReader();

                if (RDESchemaCheck)
                {
                    DataTable currentSchema = reader.GetSchemaTable();

                    SchemaDifferences schemaDiffs = getSchemaDifferences(currentSchema, this.templateSchema);

                    if (schemasMatch(schemaDiffs))
                    {
                        try
                        {
                            target.Load(reader);
                        }
                        catch (Exception ex)
                        {
                            if(ex.Message.ToLower().Contains("failed to enable constraints"))
                            {
                                try
                                {
                                    DataTable sourceData = new DataTable();
                                    sourceData.Load(reader);
                                    addDataRowsManually(sourceData, target, this.templateColNames);
                                }
                                catch (Exception innerex)
                                {
                                    this.tablesNotAdded.Add($"{tableName}: error adding rows manually with message '{innerex.Message}'");
                                }
                            }
                            else
                            {
                                this.tablesNotAdded.Add($"{tableName} with error: {ex.Message}");
                            }
                        }
                    }
                    else //schemas don't match - check type mismatches first
                    {
                        DataTable sourceData = new DataTable();
                        sourceData.Load(reader);

                        //check type mismatches again to confirm if values can be converted
                        for (int i = schemaDiffs.typeMismatches.Count - 1; i >= 0; i--)
                        {
                            string colMismatch = schemaDiffs.typeMismatches[i].columnName;
                            string templateColType = schemaDiffs.typeMismatches[i].templateType;

                            foreach (DataRow sourceRow in sourceData.Rows)
                            {
                                try
                                {
                                    switch (templateColType)
                                    {
                                        case "System.Boolean":
                                            Convert.ToBoolean(sourceRow[colMismatch]);
                                            break;
                                        case "System.Byte":
                                            Convert.ToByte(sourceRow[colMismatch]);
                                            break;
                                        case "System.DateTime":
                                            Convert.ToDateTime(sourceRow[colMismatch]);
                                            break;
                                        case "System.Decimal":
                                            Convert.ToDecimal(sourceRow[colMismatch]);
                                            break;
                                        case "System.Double":
                                            Convert.ToDouble(sourceRow[colMismatch]);
                                            break;
                                        case "System.Int16":
                                            Convert.ToInt16(sourceRow[colMismatch]);
                                            break;
                                        case "System.Int32":
                                            Convert.ToInt32(sourceRow[colMismatch]);
                                            break;
                                        case "System.Int64":
                                            Convert.ToInt64(sourceRow[colMismatch]);
                                            break;
                                        case "System.Single":
                                            Convert.ToSingle(sourceRow[colMismatch]);
                                            break;
                                        default: //its a string, and anything can convert to a string
                                            break;
                                    }

                                    schemaDiffs.typeMismatches.RemoveAt(i);
                                }
                                catch
                                {
                                    //nothing needed here
                                }
                            }
                        }

                        //there may be no more schema differences, so let's test again
                        if (schemasMatch(schemaDiffs))
                        {
                            try
                            {
                                target.Load(reader); //this should only throw if there is an incompatible type difference which should never be the case because we checked already above
                            }
                            catch (Exception ex)
                            {
                                if (ex.Message.ToLower().Contains("failed to enable constraints"))
                                {
                                    try
                                    {
                                        addDataRowsManually(sourceData, target, this.templateColNames);
                                    }
                                    catch (Exception innerex)
                                    {
                                        this.tablesNotAdded.Add($"{tableName}: error adding rows manually with message '{innerex.Message}'");
                                    }
                                }
                                else
                                {
                                    this.tablesNotAdded.Add($"{tableName} with error: {ex.Message}");
                                }
                            }
                        }
                        else //schemas still don't match
                        {
                            //have we processed this different schema already
                            if (isSchemaAlreadyAccepted(currentSchema)) 
                            {

                                try
                                {
                                    addDataRowsManually(sourceData, target, this.templateColNames);
                                }
                                catch(Exception ex)
                                {
                                    this.tablesNotAdded.Add($"{tableName}: error adding rows manually with message '{ex.Message}'");
                                }
                            }
                            else
                            {
                                //ask if we can add the records manually
                                string missingColumnsString = String.Join(", ", schemaDiffs.missingColumns.Select(x => $"[{x}]"));
                                string extraColumnsString = String.Join(", ", schemaDiffs.extraColumns.Select(x => $"[{x}]"));
                                string typeMismatchesString = "";
                                foreach(SchemaTypeMismatch stm in schemaDiffs.typeMismatches)
                                {
                                    string stmString = $"[{stm.columnName}] - currently {stm.mismatchedType.Replace("System", "")} should be {stm.templateType.Replace("System", "")}{Environment.NewLine}";
                                    typeMismatchesString += stmString;
                                }

                                string tableAndCols = $"File: {tableName + Environment.NewLine + Environment.NewLine}";
                                if (schemaDiffs.missingColumns.Count > 0)
                                {
                                    tableAndCols += $"Missing: {missingColumnsString + Environment.NewLine + Environment.NewLine}";
                                }

                                if (schemaDiffs.extraColumns.Count > 0)
                                {
                                    tableAndCols += $"Additional {extraColumnsString + Environment.NewLine + Environment.NewLine}";
                                }

                                if (schemaDiffs.typeMismatches.Count > 0)
                                {
                                    tableAndCols += $"TYPE MISMATCHES: {typeMismatchesString + Environment.NewLine + Environment.NewLine}";
                                }

                                if(schemaDiffs.typeMismatches.Count == 0)
                                {
                                    string showMessage = $"{tableAndCols}Do you want to add this file anyway?";
                                    DialogResult result = MessageBox.Show(showMessage, $"Confirm schema for {tableName}", MessageBoxButtons.YesNo);

                                    if (result == DialogResult.Yes)
                                    {
                                        try
                                        {
                                            addDataRowsManually(sourceData, target, this.templateColNames);
                                            this.schemasAccepted.Add(currentSchema);
                                        }
                                        catch (Exception ex)
                                        {
                                            this.tablesNotAdded.Add($"{tableName}: error adding rows manually with message '{ex.Message}'");
                                        }
                                        
                                    }
                                    else if (result == DialogResult.No)
                                    {
                                        this.tablesNotAdded.Add($"{tableName}: user chose not to add table due to schema differences");
                                    }
                                }
                                else
                                {
                                    string showMessage = $"{tableAndCols}VALUES IN COLUMNS WITH TYPE MISMATCHES MUST BE FIXED BEFORE THIS FILE CAN BE IMPORTED?";
                                    MessageBox.Show(showMessage);
                                }                                  
                            }
                        }
                    }
                }
                else
                {
                    try
                    {
                        target.Load(reader);
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.ToLower().Contains("failed to enable constraints"))
                        {
                            try
                            {
                                DataTable sourceData = new DataTable();
                                sourceData.Load(reader);
                                addDataRowsManually(sourceData, target, this.templateColNames);
                            }
                            catch (Exception innerex)
                            {
                                this.tablesNotAdded.Add($"{tableName}: error adding rows manually with message '{innerex.Message}'");
                            }
                        }
                        else
                        {
                            this.tablesNotAdded.Add($"{tableName} with error: {ex.Message}");
                        }
                    }
                }

                try
                {
                    connection.Close();
                }
                catch(Exception ex)
                {
                    MessageBox.Show($"Connection could not be closed for {tableName} with error: {ex.Message}");
                }
                    
            }

        }

        private void addRDERecords(string fileName)
        {
            string folderName = this.workingDir;

            readDBFTable(folderName, fileName, this.records, true);
            lblNumberOfRecords.Text = "Number of records: " + this.records.Rows.Count;

        }

        private bool isSchemaAlreadyAccepted(DataTable currentSchema)
        {
            if (this.schemasAccepted.Any())
            {
                DataTable acceptedSchema;
                for (int i = 0; i < this.schemasAccepted.Count; i++)
                {
                    acceptedSchema = this.schemasAccepted[i];

                    if(schemasMatch(getSchemaDifferences(currentSchema, acceptedSchema)))
                    {
                        return true;
                    }
                }

                return false;
            }
            else
            {
                return false;
            }
        }

        private bool schemasMatch(SchemaDifferences sds)
        {

            return (sds.typeMismatches.Count == 0 && sds.extraColumns.Count == 0 && sds.missingColumns.Count == 0);
        }

        private SchemaDifferences getSchemaDifferences(DataTable currentSchema, DataTable checkSchema)
        {
            List<string> currentSchemaColumns = new List<string>();
            List<SchemaTypeMismatch> colTypeMismatches = new List<SchemaTypeMismatch>();

            foreach (DataRow row in currentSchema.Rows)
            {
                string colName = row["ColumnName"].ToString();
                currentSchemaColumns.Add(colName);

                string colType = row["DataType"].ToString();

                DataRow[] checkShemaRow = checkSchema.Select($"ColumnName = '{colName}'");

                if (checkShemaRow.Length > 0)
                {
                    string templateColType = checkShemaRow[0]["DataType"].ToString();
                    if (colType != templateColType)
                    {
                        SchemaTypeMismatch stm = new SchemaTypeMismatch();
                        stm.columnName = colName;
                        stm.templateType = templateColType;
                        stm.mismatchedType = colType;

                        colTypeMismatches.Add(stm);
                    }
                }
            }

            //get checkSchema colNames
            List<string> checkSchemaColNames = new List<string>();
            foreach(DataRow row in checkSchema.Rows)
            {
                checkSchemaColNames.Add(row["ColumnName"].ToString());
            }

            //get missing columns
            List<string> missingColumns = checkSchemaColNames.Except(currentSchemaColumns).ToList();

            //get additional columns
            List<string> extraColumns = currentSchemaColumns.Except(checkSchemaColNames).ToList();

            SchemaDifferences sds = new SchemaDifferences();
            sds.extraColumns = extraColumns;
            sds.missingColumns = missingColumns;
            sds.typeMismatches = colTypeMismatches;

            return sds;

        }

        private void addDataRowsManually(DataTable source, DataTable target, List<string> targetColNames)
        {
            //add the rows one by one, keep track and delete again if there is a problem
            int rowsAdded = 0;
            DataRow newTargetRow;
            DataRow sourceRow;
            for (int i = 0; i < source.Rows.Count; i++)
            {
                newTargetRow = target.NewRow();
                sourceRow = source.Rows[i];

                foreach (string col in targetColNames)
                {
                    try
                    {
                        var val = sourceRow[col];

                        //this is a hack. del should alway be empty, and it sometimes looks empty but is not
                        if(col == "del")
                        {
                            val = DBNull.Value;
                        }

                        //we need to trim strings because Brahms adds white space
                        if (val is string)
                        {
                            string stringval = val.ToString();
                            stringval = stringval.Trim();
                            val = stringval;
                        }

                        try
                        {
                            newTargetRow[col] = val;
                        }
                        catch (Exception ex) //this is a problem, and we need to reverse
                        {
                            try
                            {
                                int rowsDeleted = 0;
                                for (int j = target.Rows.Count - 1; rowsDeleted < rowsAdded; j--)
                                {
                                    target.Rows[j].Delete();
                                    rowsDeleted++;
                                }
                                target.AcceptChanges();
                            }
                            catch (Exception innerEx)
                            {
                                int testint = 5;
                                //I really hope this never happens!
                            }

                            throw ex;
                        }

                    }
                    catch
                    {
                        try
                        {
                            newTargetRow[col] = DBNull.Value;
                        }
                        catch (Exception ex) //this is a problem, and we need to reverse
                        {
                            try
                            {
                                int rowsDeleted = 0;
                                for (int j = target.Rows.Count - 1; rowsDeleted < rowsAdded; j--)
                                {
                                    target.Rows[j].Delete();
                                    rowsDeleted++;
                                }
                                target.AcceptChanges();
                            }
                            catch (Exception inex)
                            {
                                int testint = 5;
                            }

                            throw ex;
                        }
                    }
                }

                try
                {
                    target.Rows.Add(newTargetRow);
                    rowsAdded++;
                }
                catch (Exception ex)
                {
                    try
                    {
                        int rowsDeleted = 0;
                        for (int j = target.Rows.Count - 1; rowsDeleted < rowsAdded; j--)
                        {
                            target.Rows[j].Delete();
                            rowsDeleted++;
                        }
                        target.AcceptChanges();
                    }
                    catch (Exception inex)
                    {
                        int testint = 5;
                    }

                    throw ex;

                }

            }
        }

        private void findBotRecDuplicates()
        {

            this.botRecDuplicates = new List<BotanicalRecordDuplicateTracker>();
            this.botRecDuplicatesIndex = 0;

            char[] seps = { '-', '–', '—' };

            List<DataRow> recordsToDelete = new List<DataRow>(); //for identical duplicate records to delete

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
                if (char.IsLetter(root[root.Length - 1]))
                {
                    root = root.Substring(0, root.Length - 1);
                }

                // check if we've already processed this root
                int cnt = this.botRecDuplicates.Where(x => x.root == root).Count();

                if (cnt == 0)
                {
                    List<DataRow> dups = this.records.Select($"TRIM(barcode) like '{root}*'").ToList();

                    if (dups.Count > 1)
                    {

                        List<int> dupsToRemove = new List<int>();

                        //first if they are identical record the identical records to delete
                        for (int i = 0; i < dups.Count - 1; i++)
                        {
                            for (int j = i + 1; j < dups.Count; j++)
                            {
                                bool identical = true;
                                for (int k = 0; k < dups[i].ItemArray.Length; k++)//iterate the values
                                {
                                    if (dups[i].ItemArray[k].ToString().Trim() != dups[j].ItemArray[k].ToString().Trim())
                                    {
                                        identical = false;
                                        break;
                                    }
                                }
                                if (identical)
                                {
                                    DataRow rowToDelete = dups[i];
                                    if (!recordsToDelete.Contains(rowToDelete)) //just so we don't add it twice
                                    {
                                        recordsToDelete.Add(rowToDelete);
                                    }
                                    dupsToRemove.Add(i);
                                }
                            }
                        }

                        //clean up
                        if (dupsToRemove.Count > 0)
                        {
                            foreach (int index in dupsToRemove)
                            {
                                dups.RemoveAt(index);
                            }
                        }

                        if (dups.Count > 1)
                        {
                            //then record those that are not identical for manual processing
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
                else
                {
                    continue;
                }

            }

            //delete any identical duplicates
            if (recordsToDelete.Count > 0)
            {
                foreach (DataRow record in recordsToDelete)
                {
                    record.Delete();
                    identicalDupsDeletedCount++;
                }
                this.records.AcceptChanges();
                MessageBox.Show($"{identicalDupsDeletedCount} identical duplicates removed");
            }

            this.dupsSearched = true;
        }

        private bool findNextRowWithCaptureErrors(int startIndex)
        {
            this.rowErrors.Clear();

            if (startIndex < this.records.Rows.Count)
            {
                for (int i = startIndex; i < this.records.Rows.Count; i++)
                {

                    DataRow row = this.records.Rows[i];

                    if (!RecordErrorFinder.barcodeHasValue(row))
                    {
                        this.rowErrors.Add(RecordErrors.noBarcode);
                    }

                    if (!RecordErrorFinder.barcodeValid(row))
                    {
                        this.rowErrors.Add(RecordErrors.invalidBarcode);
                    }

                    /*
                    no longer used
                    if (!RecordErrorFinder.numberIsAnIntegerOrSN(row))
                    {
                        this.rowErrors.Add(RecordErrors.collNumberError);
                    }
                    */

                    if (!RecordErrorFinder.higherTaxaAllPresent(row))
                    {
                        this.rowErrors.Add(RecordErrors.higherTaxaMissing);
                    }

                    string ranksNotInBackbone = RecordErrorFinder.getRanksNotInBackbone(row, this.taxa);
                    if (!String.IsNullOrEmpty(ranksNotInBackbone))
                    {
                        this.rowErrors.Add($"{RecordErrors.ranksNotInBackbone}: {ranksNotInBackbone}");
                    }

                    if (!RecordErrorFinder.detDateIsValid(row))
                    {
                        this.rowErrors.Add(RecordErrors.detDateInvalid);
                    }

                    string determinerNotInList = RecordErrorFinder.isDeterminerInList(row, this.people, this.peopleChecked);
                    if (!String.IsNullOrEmpty(determinerNotInList))
                    {
                        this.rowErrors.Add("The determiner is not in the master list: " + determinerNotInList);
                        btnConfirmPeople.Enabled = true;
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

                    string coordsErrors = RecordErrorFinder.getCoordErrors(row);
                    if (!String.IsNullOrEmpty(coordsErrors))
                    {
                        this.rowErrors.Add($"{RecordErrors.coordinateErrors}: {coordsErrors}");
                    }

                    if (!RecordErrorFinder.isQDSValid(row))
                    {
                        this.rowErrors.Add(RecordErrors.qdsInvalid);
                    }

                    string collectorsNotIn = RecordErrorFinder.getCollectorsNotInList(row, this.people, this.peopleChecked);
                    if (!String.IsNullOrEmpty(collectorsNotIn))
                    {
                        this.rowErrors.Add("The following collectors are not in the master list: " + collectorsNotIn);
                        btnConfirmPeople.Enabled = true;
                    }

                    if (!RecordErrorFinder.collDateIsValid(row))
                    {
                        this.rowErrors.Add(RecordErrors.collectionDateInvalid);
                    }


                    //last one!
                    if (!RecordErrorFinder.detDateAfterCollDate(row))
                    {
                        this.rowErrors.Add(RecordErrors.detDateBeforeCollectionDate);
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

        private bool findNextRowWithQDSCoordErrors(int startIndex)
        {
            this.rowErrors.Clear();

            if (startIndex < this.records.Rows.Count)
            {
                for (int i = startIndex; i < this.records.Rows.Count; i++)
                {

                    DataRow row = this.records.Rows[i];

                    if (!RecordErrorFinder.QDSValidForCountry(row, this.CountryQDSs))
                    {
                        this.rowErrors.Add(RecordErrors.qdsNotValidForCountry);
                    }
                    else //TEST FOR THE QDS - COORDINATES MATCH HERE
                    {
                        if (!RecordErrorFinder.coordsMatchQDS(row))
                        {
                            this.rowErrors.Add(RecordErrors.qdsCoordsMismatch);
                        }
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

        private void showNextDuplicate()
        {
            BotanicalRecordDuplicateTracker next = this.botRecDuplicates[this.botRecDuplicatesIndex];

            lblDupsCount.Text = $"{this.botRecDuplicatesIndex} of {this.botRecDuplicates.Count}";

            //make the search string
            string INSearch = "";
            foreach (string dupBarcode in next.dupBarcodes)
            {
                string quoted = $"'{dupBarcode}'";
                INSearch += quoted + ", ";
            }
            INSearch = INSearch.Substring(0, INSearch.Length - 2); //remove the last comma

            string expression = $"barcode IN ({INSearch})";

            if (this.viewRecordsBinding == null)
            {
                this.viewRecordsBinding = new BindingSource();
                this.viewRecordsBinding.DataSource = this.records;
            }

            this.viewRecordsBinding.Filter = expression;

            if (this.dgvRecordsView.DataSource == null)
            {
                this.dgvRecordsView.DataSource = viewRecordsBinding;
            }

            this.botRecDuplicatesIndex++;

            this.lblDupIndexCount.Text = $"{this.botRecDuplicatesIndex + 1} of {this.botRecDuplicates.Count} duplicates";
        }

        private void showNextError()
        {
            //show the error list
            string errors = String.Join(Environment.NewLine + Environment.NewLine, this.rowErrors.ToArray());
            this.rtbReportErrors.Text = errors;

            //show the record in the datagridview
            if (this.viewRecordsBinding == null)
            {
                this.viewRecordsBinding = new BindingSource();
            }

            //we need a DataTable for the bindingSource
            DataTable errorTbl = this.records.AsEnumerable().Where((row, index) => index == errorRecordIndex).CopyToDataTable();

            this.viewRecordsBinding.DataSource = errorTbl;
            this.dgvRecordsView.DataSource = this.viewRecordsBinding;

            this.lblErrorRowIndex.Text = $"error in row {this.errorRecordIndex + 1} of {this.records.Rows.Count}";
            this.lblErrorRowIndex.Visible = true;
        }

        private void GetFilesRecursive(string targetDirectory, List<string> files)
        {


            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
                files.Add(fileName);

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
            {
                GetFilesRecursive(subdirectory, files);
            }

        }

        private void saveTableAsDBF(string saveFilePath, DataTable dataSource)
        {
            if (saveFilePath != "")
            {
                //use the template
                string templateFile = this.txtSchemaFile.Text;

                if (String.IsNullOrEmpty(templateFile))
                {
                    MessageBox.Show("No template schema file is selected for saving");
                    return;
                }

                try
                {
                    File.Copy(Path.Combine(this.templateDir, templateFile), saveFilePath, true);

                    //we also need to copy the fpt file
                    string sourceFPT = templateFile.ToLower().Replace(".dbf", ".fpt");
                    string destFPT = saveFilePath.ToLower().Replace(".dbf", ".fpt");
                    File.Copy(Path.Combine(this.templateDir, sourceFPT), destFPT, true);
                }

                // Catch exception if the file was already copied.
                catch (IOException copyError)
                {
                    MessageBox.Show($"Error with saving file. Copy template failed with message: {copyError.Message}");
                    return;
                }

                //connect to the new file
                string fileNameOnly = Path.GetFileName(saveFilePath);
                string ext = Path.GetExtension(saveFilePath);
                string tableName = fileNameOnly.Replace(ext, "");

                string connectionString = @"Provider=VFPOLEDB.1;Data Source=" + saveFilePath;
                string deleteSQL = $"DELETE FROM [{tableName}]";

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

                    //clear all records
                    using (OleDbCommand deleteCmd = new OleDbCommand(deleteSQL, connection))
                    {
                        try
                        {
                            int deletedRecordCount = deleteCmd.ExecuteNonQuery();
                            //MessageBox.Show($"Records deleted: {deletedRecordCount}");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error with saving file. Delete records from template failed with message: {ex.Message}");
                            return;
                        }
                    }

                    //add the new records
                    List<string> columnNames = new List<string>();
                    foreach (DataColumn col in dataSource.Columns)
                    {
                        columnNames.Add(col.ColumnName.Trim());
                    }

                    string joinedColNames = String.Join(", ", columnNames.ToArray());

                    //we need the list of parameter placeholders for the sql statement
                    List<string> placeholders = new List<string>();
                    for (int i = 0; i < columnNames.Count; i++)
                    {
                        placeholders.Add("?");
                    }

                    string joinedPlaceholders = String.Join(", ", placeholders.ToArray());

                    string addSQL = $"INSERT INTO [{tableName}] ({joinedColNames}) VALUES ({joinedPlaceholders})";

                    List<string> saveErrorBarcodes = new List<string>();
                    using (OleDbCommand insertCommand = new OleDbCommand(addSQL, connection))
                    {
                        int counter = 0; // this is just for keeping track during debugging


                        foreach (DataRow row in dataSource.Rows)
                        {
                            counter++;
                            insertCommand.Parameters.Clear(); // clean everything out

                            foreach (string colName in columnNames)
                            {
                                //get the type for this column
                                string coltype = "";
                                bool isLong;
                                try
                                {
                                    DataRow schema = this.templateSchema.Select($"ColumnName = '{colName}'")[0];
                                    coltype = schema["DataType"].ToString();
                                    isLong = (bool)schema["IsLong"];

                                }
                                catch
                                {
                                    MessageBox.Show($"Could not find column {colName} in {tableName}. File save aborted.");
                                    return;
                                }

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
                                else if (coltype == "System.Decimal")
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
                                saveErrorBarcodes.Add(row["barcode"].ToString().Trim());
                            }
                        }
                    }


                    //if we get here, it worked!!

                    if (saveErrorBarcodes.Count == 0)
                    {
                        MessageBox.Show("Records successfully saved");
                    }
                    else
                    {
                        MessageBox.Show($"The following records were not saved due to errors: {String.Join("; ", saveErrorBarcodes.ToArray())}");
                    }
                }
            }
        }

        // Insert logic for processing found files here.
        public static void ProcessFile(string path)
        {
            Console.WriteLine("Processed file '{0}'.", path);
        }

        //PROPERTIES

        private string templateDir { get; set; }
        private string workingDir { get; set; } //The source directory that the RDE files come from
        private List<string> filesToImport { get; set; }

        public string modalMessage { get; set; }

        private DataTable templateSchema { get; set; } //the schema of the first DBF file imported, for checking other files and for use on save
        private List<string> templateColNames { get; set; } //the column names from the schema for checking additional files imported
        private List<string> templateColTypes { get; set; }
        private List<string> tablesNotAdded { get; set; } //to keep a list of tables where adding records failed - we are into serious state management here...
        private List<DataTable> schemasAccepted { get; set; } //for recording the schemas the user has accepted while tables are added. 

        //data
        private DataTable records { get; set; }
        private DataTable taxa { get; set; }
        private DataTable existingRecords { get; set; }
        private HashSet<string> existingBarcodes { get; set; }
        private List<int> existingBarcodeIndices { get; set; }
        private DataTable people { get; set; }

        private List<BotanicalRecordDuplicateTracker> botRecDuplicates { get; set; }
        private bool dupsSearched { get; set; }
        private int botRecDuplicatesIndex { get; set; }
        private int identicalDupsDeletedCount { get; set; }

        private List<string> peopleChecked { get; set; } //for names not in people but accepted by the user
        private List<string> missingColumnsToIgnore { get; set; } //for schema checks during RDE file import

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
