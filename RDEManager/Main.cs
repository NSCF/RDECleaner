using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace RDEManager
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            
            
            this.alreadyReadFileNames = new List<string>();
            fileNamesBinding = new BindingList<string>(alreadyReadFileNames);

            localityWithValues = new List<DataRow>();
            majorareaWithValues = new List<DataRow>();
            minorareaWithValues = new List<DataRow>();

        }

        //functions to add DBF files

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

        //helpers

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

        //functions to check data

        private void btnCheckRecords_Click(object sender, EventArgs e)
        {

            this.RDEErrors = new List<string>();
            this.errorsBinding = new BindingList<string>(RDEErrors);
            lbRDEErrors.DataSource = errorsBinding;

            this.columnNames = this.records.Columns.Cast<DataColumn>()
                                 .Select(x => x.ColumnName)
                                 .ToArray();

            //remove exclamations from barcode numbers - these are added if the barcode is found on the actual live database
            //these will be checked again by the person uploading the records into the source database
            DataColumn col = this.records.Columns["barcode"];
            foreach (DataRow row in this.records.Rows)
            {
                string barcodeVal = row[col].ToString();
                if (barcodeVal.Contains("!"))
                {
                    row[col] = barcodeVal.Replace("!", "");
                }
            }

            //Delete identical duplicates
            int countBefore = this.records.Rows.Count;
            this.records = this.records.DefaultView.ToTable(true);
            int countAfter = this.records.Rows.Count;

            if (countBefore > countAfter)
            {
                errorsBinding.Add($"There were {countBefore - countAfter} identical duplicate records. These have been removed.");
            }

           
            List<string> capturedBarcodes = this.records.AsEnumerable().Select(x => x["barcode"].ToString().ToUpper().Trim()).ToList();

            List<string> errorLogs = new List<string>(); //because sometimes the log message are different to the on screen messages
         

            //get the remaining duplicates
            var duplicates = capturedBarcodes
                //.Select(b => b.Contains("-") ? b.Substring(0, b.LastIndexOf('-')).Trim() : b.Trim()) //comment this out if we don't want to look at the dash numbers (sheet numbers)
                .GroupBy(x => x.Trim())
                .Where(g => g.Count() > 1)
                .Select(y => new { Element = y.Key, Counter = y.Count() })
                .ToList();

            if(duplicates.Count > 0)
            {

                string dupMessage = $"The following {duplicates.Count} records have non-identical duplicates: ";
                duplicates.ForEach(dup =>
                {
                    dupMessage += $"{dup.Element} ({dup.Counter} duplicates); ";
                });

                dupMessage = dupMessage.Substring(0, dupMessage.Length - 2); //trim the last '; '

                errorsBinding.Add($"There are {duplicates.Count} non-identical duplicates. See the log.");
                errorLogs.Add(dupMessage);
            }

            //locality must be empty
            if (columnNames.Contains("locality"))
            {
                localityWithValues = this.records.Select("locality is not null and trim(locality) <> ''").ToList();
                if (localityWithValues.Count > 0)
                {
                    string locmsg = $"Field 'locality' contains values for {localityWithValues.Count} records. All locality data must be recorded in 'locnotes'. See the log.";
                    errorsBinding.Add(locmsg);
                    string loclogmsg = $"{localityWithValues.Count } records with locality values: {String.Join("; ", localityWithValues.Select(row => row["barcode"].ToString().Trim()).ToArray())}.";
                    errorLogs.Add(loclogmsg);
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
                    errorLogs.Add(majorlogmsg);
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
                    errorLogs.Add(minorlogmsg);
                }
            }

            //dups must be empty
            if (columnNames.Contains("dups"))
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

            //check all images have been captured
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
                    this.notCaptured = barcodesFromFileNames.Where(bf => !capturedBarcodes.Contains(bf)).ToList();

                    //get those in the list where we don't have images
                    List<string> noImages = capturedBarcodes.Where(bc => !barcodesFromFileNames.Contains(bc)).ToList();

                    if (notCaptured.Count > 0)
                    {
                        notCaptured.Sort();
                        errorsBinding.Add($"{notCaptured.Count} image files that do not match captured barcodes. See the log.");
                        string logNotCapturedMsg = $"Image file names without matching barcode values in RDE files ({notCaptured.Count}): {String.Join("; ", notCaptured.ToArray())}";
                        errorLogs.Add(logNotCapturedMsg);
                    }

                    if (noImages.Count > 0)
                    {
                        noImages.Sort();
                        errorsBinding.Add($"{noImages.Count} captured barcodes that do not match image files. See the log.");
                        string noImagesMsg = $"No corresponding images for the following RDE barcodes ({noImages.Count}): {String.Join("; ", noImages.ToArray())}";
                        errorLogs.Add(noImagesMsg);
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
                errorLogs.Insert(0, "IMAGES NOT CHECKED!!!!");
            }

            //update the imagelist field to include the image paths
            int imagePathsUpdated = 0;
            foreach(DataRow row in this.records.Rows)
            {
                string bc = row["barcode"].ToString().Trim();
                List<string> paths = this.imagePaths.Where(s => s.Contains(bc)).ToList();
                if(paths.Count > 0)
                {
                    row["imagelist"] = String.Join(Environment.NewLine, paths);
                    imagePathsUpdated++;
                }
            }

            if(imagePathsUpdated > 0)
            {
                errorsBinding.Add($"'imagelist' field updated for {imagePathsUpdated} records");
            }

            //check taxon names against the backbone
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
                    errorLogs.Add($"Family names are missing for: {String.Join("; ", noFamily.ToArray())}");
                }

                if (noGenus.Count > 0)
                {
                    errorLogs.Add($"Generus names are missing for: {String.Join("; ", noGenus.ToArray())}");
                }

                if (noSpecies.Count > 0)
                {
                    errorLogs.Add($"Species names are missing for: {String.Join("; ", noSpecies.ToArray())}");
                }

                if (noSubspecies.Count > 0)
                {
                    errorLogs.Add($"Subspecies names are missing for: {String.Join("; ", noSubspecies.ToArray())}");
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
                    errorLogs.Add($"The following invalid family names were found: {String.Join("; ", invalidFamilies.ToArray())}");
                }

                if (invalidGenera.Count > 0)
                {
                    errorLogs.Add($"The following invalid genus names were found: {String.Join("; ", invalidGenera.ToArray())}");
                }

                if (invalidSpecies.Count > 0)
                {
                    List<string> fullSpeciesNames = new List<string>();
                    invalidSpecies.ForEach(sp => fullSpeciesNames.Add($"{sp.genus} {sp.species}"));
                    errorLogs.Add($"The following invalid species names were found: {String.Join("; ", fullSpeciesNames.ToArray())}");
                }

                if (invalidSubspecies.Count > 0)
                {
                    List<string> fullSubspeciesNames = new List<string>();
                    invalidSubspecies.ForEach(sp => fullSubspeciesNames.Add($"{sp.genus} {sp.species} {sp.subspecies}"));
                    errorLogs.Add($"The following invalid subspecies names were found: {String.Join("; ", fullSubspeciesNames.ToArray())}");
                }

                if (invalidVars.Count > 0)
                {
                    List<string> fullVarNames = new List<string>();
                    invalidVars.ForEach(var => fullVarNames.Add($"{var.genus} {var.species} {var.subspecies} {var.var}"));
                    errorLogs.Add($"The following invalid variety/form names were found: {String.Join("; ", fullVarNames.ToArray())}");
                }

            }

            //TODO: add the data capturer names and dates

            //write the error log
            
            using (System.IO.StreamWriter logfile =
            new System.IO.StreamWriter($"{this.workingDir}\\errorlog.txt"))
            {
                string lastDir = this.workingDir.Substring(this.workingDir.LastIndexOf('\\') + 1);

                logfile.WriteLine($"Error log for {lastDir} \\ {txtChooseDBF.Text}");
                logfile.WriteLine($"{DateTime.Now}");
                logfile.Write(Environment.NewLine);

                foreach (string log in errorLogs)
                {
                    logfile.WriteLine(log);
                    logfile.Write(Environment.NewLine);
                }
            }

        }

        //properties

        private string workingDir { get; set; }

        private List<string> alreadyReadFileNames { get; set; }
        private DataTable records { get; set; }
        private DataTable taxa { get; set; }
        private string[] columnNames { get; set; }
        private BindingList<string> fileNamesBinding { get; set; }
        private BindingList<string> errorsBinding { get; set; }
        private List<string> RDEErrors { get; set; }
        private List<string> imagePaths { get; set; }


        private List<DataRow> localityWithValues { get; set; }
        private List<DataRow> majorareaWithValues { get; set; }
        private List<DataRow> minorareaWithValues { get; set; }
        private List<string> notCaptured { get; set; }

        
    }
}
