namespace RDEManager
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.txtChooseDBF = new System.Windows.Forms.TextBox();
            this.btnChooseDBF = new System.Windows.Forms.Button();
            this.lblChooseDBF = new System.Windows.Forms.Label();
            this.btnCheckDuplicates = new System.Windows.Forms.Button();
            this.lblNumberOfRecords = new System.Windows.Forms.Label();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.btnAddDBFData = new System.Windows.Forms.Button();
            this.txtImageFolder = new System.Windows.Forms.TextBox();
            this.btnChooseImageFolder = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtTaxonBackbone = new System.Windows.Forms.TextBox();
            this.btnChooseTaxonBackbone = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.lblNoTaxa = new System.Windows.Forms.Label();
            this.txtQDSCountriesFile = new System.Windows.Forms.TextBox();
            this.btnChooseQDSCountries = new System.Windows.Forms.Button();
            this.lblChooseQDSCountries = new System.Windows.Forms.Label();
            this.txtPeopleTable = new System.Windows.Forms.TextBox();
            this.btnChoosePeople = new System.Windows.Forms.Button();
            this.lblChoosePeople = new System.Windows.Forms.Label();
            this.dgvRecordsView = new System.Windows.Forms.DataGridView();
            this.btnNextDuplicate = new System.Windows.Forms.Button();
            this.lblDupsCount = new System.Windows.Forms.Label();
            this.btnTestSomething = new System.Windows.Forms.Button();
            this.lblDupIndexCount = new System.Windows.Forms.Label();
            this.rtbReportErrors = new System.Windows.Forms.RichTextBox();
            this.lblReportErrors = new System.Windows.Forms.Label();
            this.btnDeleteRows = new System.Windows.Forms.Button();
            this.btnMergeDups = new System.Windows.Forms.Button();
            this.btnCleanRecords = new System.Windows.Forms.Button();
            this.gbCollNumberOptions = new System.Windows.Forms.GroupBox();
            this.rbNone = new System.Windows.Forms.RadioButton();
            this.rbNumberToSN = new System.Windows.Forms.RadioButton();
            this.rbNumberFromBarcode = new System.Windows.Forms.RadioButton();
            this.btnFindErrors = new System.Windows.Forms.Button();
            this.btnFindMissingRecords = new System.Windows.Forms.Button();
            this.btnAddQDSFromCoords = new System.Windows.Forms.Button();
            this.btnCheckQDSCountry = new System.Windows.Forms.Button();
            this.btnNextRowWithErrors = new System.Windows.Forms.Button();
            this.btnSaveChanges = new System.Windows.Forms.Button();
            this.lblErrorRowIndex = new System.Windows.Forms.Label();
            this.lblPeople = new System.Windows.Forms.Label();
            this.lblQDSCountries = new System.Windows.Forms.Label();
            this.chkAddOrganization = new System.Windows.Forms.CheckBox();
            this.txtAddWho = new System.Windows.Forms.TextBox();
            this.btnCheckTaxa = new System.Windows.Forms.Button();
            this.btnClearRecords = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecordsView)).BeginInit();
            this.gbCollNumberOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtChooseDBF
            // 
            this.txtChooseDBF.Location = new System.Drawing.Point(17, 51);
            this.txtChooseDBF.Name = "txtChooseDBF";
            this.txtChooseDBF.Size = new System.Drawing.Size(734, 20);
            this.txtChooseDBF.TabIndex = 10;
            // 
            // btnChooseDBF
            // 
            this.btnChooseDBF.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnChooseDBF.BackgroundImage")));
            this.btnChooseDBF.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnChooseDBF.Location = new System.Drawing.Point(216, 17);
            this.btnChooseDBF.Name = "btnChooseDBF";
            this.btnChooseDBF.Size = new System.Drawing.Size(43, 30);
            this.btnChooseDBF.TabIndex = 9;
            this.btnChooseDBF.UseVisualStyleBackColor = true;
            this.btnChooseDBF.Click += new System.EventHandler(this.btnChooseDBF_Click);
            // 
            // lblChooseDBF
            // 
            this.lblChooseDBF.AutoSize = true;
            this.lblChooseDBF.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblChooseDBF.Location = new System.Drawing.Point(12, 23);
            this.lblChooseDBF.Name = "lblChooseDBF";
            this.lblChooseDBF.Size = new System.Drawing.Size(203, 25);
            this.lblChooseDBF.TabIndex = 8;
            this.lblChooseDBF.Text = "Choose the RDE files:";
            // 
            // btnCheckDuplicates
            // 
            this.btnCheckDuplicates.Enabled = false;
            this.btnCheckDuplicates.Location = new System.Drawing.Point(17, 366);
            this.btnCheckDuplicates.Name = "btnCheckDuplicates";
            this.btnCheckDuplicates.Size = new System.Drawing.Size(204, 29);
            this.btnCheckDuplicates.TabIndex = 11;
            this.btnCheckDuplicates.Text = "Check duplicates";
            this.btnCheckDuplicates.UseVisualStyleBackColor = true;
            this.btnCheckDuplicates.Click += new System.EventHandler(this.btnCheckDuplicates_Click);
            // 
            // lblNumberOfRecords
            // 
            this.lblNumberOfRecords.AutoSize = true;
            this.lblNumberOfRecords.Location = new System.Drawing.Point(502, 26);
            this.lblNumberOfRecords.Name = "lblNumberOfRecords";
            this.lblNumberOfRecords.Size = new System.Drawing.Size(97, 13);
            this.lblNumberOfRecords.TabIndex = 12;
            this.lblNumberOfRecords.Text = "Number of records:";
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog";
            // 
            // btnAddDBFData
            // 
            this.btnAddDBFData.Enabled = false;
            this.btnAddDBFData.Location = new System.Drawing.Point(281, 18);
            this.btnAddDBFData.Name = "btnAddDBFData";
            this.btnAddDBFData.Size = new System.Drawing.Size(204, 29);
            this.btnAddDBFData.TabIndex = 13;
            this.btnAddDBFData.Text = "Import records from these files";
            this.btnAddDBFData.UseVisualStyleBackColor = true;
            this.btnAddDBFData.Click += new System.EventHandler(this.addFile_Click);
            // 
            // txtImageFolder
            // 
            this.txtImageFolder.Location = new System.Drawing.Point(17, 112);
            this.txtImageFolder.Name = "txtImageFolder";
            this.txtImageFolder.Size = new System.Drawing.Size(734, 20);
            this.txtImageFolder.TabIndex = 21;
            // 
            // btnChooseImageFolder
            // 
            this.btnChooseImageFolder.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnChooseImageFolder.BackgroundImage")));
            this.btnChooseImageFolder.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnChooseImageFolder.Enabled = false;
            this.btnChooseImageFolder.Location = new System.Drawing.Point(172, 79);
            this.btnChooseImageFolder.Name = "btnChooseImageFolder";
            this.btnChooseImageFolder.Size = new System.Drawing.Size(43, 30);
            this.btnChooseImageFolder.TabIndex = 20;
            this.btnChooseImageFolder.UseVisualStyleBackColor = true;
            this.btnChooseImageFolder.Click += new System.EventHandler(this.btnChooseImageFolder_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 84);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(154, 25);
            this.label3.TabIndex = 19;
            this.label3.Text = "Image file folder:";
            // 
            // txtTaxonBackbone
            // 
            this.txtTaxonBackbone.Location = new System.Drawing.Point(17, 171);
            this.txtTaxonBackbone.Name = "txtTaxonBackbone";
            this.txtTaxonBackbone.Size = new System.Drawing.Size(734, 20);
            this.txtTaxonBackbone.TabIndex = 24;
            // 
            // btnChooseTaxonBackbone
            // 
            this.btnChooseTaxonBackbone.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnChooseTaxonBackbone.BackgroundImage")));
            this.btnChooseTaxonBackbone.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnChooseTaxonBackbone.Location = new System.Drawing.Point(281, 138);
            this.btnChooseTaxonBackbone.Name = "btnChooseTaxonBackbone";
            this.btnChooseTaxonBackbone.Size = new System.Drawing.Size(43, 30);
            this.btnChooseTaxonBackbone.TabIndex = 23;
            this.btnChooseTaxonBackbone.UseVisualStyleBackColor = true;
            this.btnChooseTaxonBackbone.Click += new System.EventHandler(this.btnChooseTaxonBackbone_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(12, 143);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(263, 25);
            this.label4.TabIndex = 22;
            this.label4.Text = "Choose the taxon backbone:";
            // 
            // lblNoTaxa
            // 
            this.lblNoTaxa.AutoSize = true;
            this.lblNoTaxa.Location = new System.Drawing.Point(343, 147);
            this.lblNoTaxa.Name = "lblNoTaxa";
            this.lblNoTaxa.Size = new System.Drawing.Size(0, 13);
            this.lblNoTaxa.TabIndex = 25;
            // 
            // txtQDSCountriesFile
            // 
            this.txtQDSCountriesFile.Location = new System.Drawing.Point(17, 231);
            this.txtQDSCountriesFile.Name = "txtQDSCountriesFile";
            this.txtQDSCountriesFile.Size = new System.Drawing.Size(734, 20);
            this.txtQDSCountriesFile.TabIndex = 29;
            // 
            // btnChooseQDSCountries
            // 
            this.btnChooseQDSCountries.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnChooseQDSCountries.BackgroundImage")));
            this.btnChooseQDSCountries.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnChooseQDSCountries.Enabled = false;
            this.btnChooseQDSCountries.Location = new System.Drawing.Point(281, 198);
            this.btnChooseQDSCountries.Name = "btnChooseQDSCountries";
            this.btnChooseQDSCountries.Size = new System.Drawing.Size(43, 30);
            this.btnChooseQDSCountries.TabIndex = 28;
            this.btnChooseQDSCountries.UseVisualStyleBackColor = true;
            this.btnChooseQDSCountries.Click += new System.EventHandler(this.btnChooseQDSCountries_Click);
            // 
            // lblChooseQDSCountries
            // 
            this.lblChooseQDSCountries.AutoSize = true;
            this.lblChooseQDSCountries.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblChooseQDSCountries.Location = new System.Drawing.Point(12, 203);
            this.lblChooseQDSCountries.Name = "lblChooseQDSCountries";
            this.lblChooseQDSCountries.Size = new System.Drawing.Size(248, 25);
            this.lblChooseQDSCountries.TabIndex = 27;
            this.lblChooseQDSCountries.Text = "Choose QDS countries list:";
            // 
            // txtPeopleTable
            // 
            this.txtPeopleTable.Location = new System.Drawing.Point(17, 290);
            this.txtPeopleTable.Name = "txtPeopleTable";
            this.txtPeopleTable.Size = new System.Drawing.Size(734, 20);
            this.txtPeopleTable.TabIndex = 32;
            // 
            // btnChoosePeople
            // 
            this.btnChoosePeople.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnChoosePeople.BackgroundImage")));
            this.btnChoosePeople.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnChoosePeople.Enabled = false;
            this.btnChoosePeople.Location = new System.Drawing.Point(281, 257);
            this.btnChoosePeople.Name = "btnChoosePeople";
            this.btnChoosePeople.Size = new System.Drawing.Size(43, 30);
            this.btnChoosePeople.TabIndex = 31;
            this.btnChoosePeople.UseVisualStyleBackColor = true;
            this.btnChoosePeople.Click += new System.EventHandler(this.btnChoosePeople_Click);
            // 
            // lblChoosePeople
            // 
            this.lblChoosePeople.AutoSize = true;
            this.lblChoosePeople.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblChoosePeople.Location = new System.Drawing.Point(12, 262);
            this.lblChoosePeople.Name = "lblChoosePeople";
            this.lblChoosePeople.Size = new System.Drawing.Size(230, 25);
            this.lblChoosePeople.TabIndex = 30;
            this.lblChoosePeople.Text = "Choose the people table:";
            // 
            // dgvRecordsView
            // 
            this.dgvRecordsView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRecordsView.Location = new System.Drawing.Point(20, 510);
            this.dgvRecordsView.Name = "dgvRecordsView";
            this.dgvRecordsView.Size = new System.Drawing.Size(1421, 171);
            this.dgvRecordsView.TabIndex = 33;
            this.dgvRecordsView.SelectionChanged += new System.EventHandler(this.dgvRecordsView_SelectionChanged);
            this.dgvRecordsView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvRecordsView_KeyDown);
            // 
            // btnNextDuplicate
            // 
            this.btnNextDuplicate.Enabled = false;
            this.btnNextDuplicate.Location = new System.Drawing.Point(108, 477);
            this.btnNextDuplicate.Name = "btnNextDuplicate";
            this.btnNextDuplicate.Size = new System.Drawing.Size(204, 27);
            this.btnNextDuplicate.TabIndex = 34;
            this.btnNextDuplicate.Text = "Show next duplicate";
            this.btnNextDuplicate.UseVisualStyleBackColor = true;
            this.btnNextDuplicate.Click += new System.EventHandler(this.btnFindNextDuplicate_Click);
            // 
            // lblDupsCount
            // 
            this.lblDupsCount.AutoSize = true;
            this.lblDupsCount.Location = new System.Drawing.Point(17, 484);
            this.lblDupsCount.Name = "lblDupsCount";
            this.lblDupsCount.Size = new System.Drawing.Size(58, 13);
            this.lblDupsCount.TabIndex = 35;
            this.lblDupsCount.Text = "No. Dups: ";
            // 
            // btnTestSomething
            // 
            this.btnTestSomething.Location = new System.Drawing.Point(954, 476);
            this.btnTestSomething.Name = "btnTestSomething";
            this.btnTestSomething.Size = new System.Drawing.Size(204, 29);
            this.btnTestSomething.TabIndex = 36;
            this.btnTestSomething.Text = "test something";
            this.btnTestSomething.UseVisualStyleBackColor = true;
            this.btnTestSomething.Visible = false;
            this.btnTestSomething.Click += new System.EventHandler(this.btnTestSomething_Click);
            // 
            // lblDupIndexCount
            // 
            this.lblDupIndexCount.AutoSize = true;
            this.lblDupIndexCount.Location = new System.Drawing.Point(240, 408);
            this.lblDupIndexCount.Name = "lblDupIndexCount";
            this.lblDupIndexCount.Size = new System.Drawing.Size(0, 13);
            this.lblDupIndexCount.TabIndex = 37;
            // 
            // rtbReportErrors
            // 
            this.rtbReportErrors.Location = new System.Drawing.Point(773, 51);
            this.rtbReportErrors.Name = "rtbReportErrors";
            this.rtbReportErrors.Size = new System.Drawing.Size(385, 344);
            this.rtbReportErrors.TabIndex = 38;
            this.rtbReportErrors.Text = "";
            // 
            // lblReportErrors
            // 
            this.lblReportErrors.AutoSize = true;
            this.lblReportErrors.Location = new System.Drawing.Point(770, 23);
            this.lblReportErrors.Name = "lblReportErrors";
            this.lblReportErrors.Size = new System.Drawing.Size(76, 13);
            this.lblReportErrors.TabIndex = 39;
            this.lblReportErrors.Text = "Reports/Errors";
            // 
            // btnDeleteRows
            // 
            this.btnDeleteRows.Enabled = false;
            this.btnDeleteRows.Location = new System.Drawing.Point(528, 477);
            this.btnDeleteRows.Name = "btnDeleteRows";
            this.btnDeleteRows.Size = new System.Drawing.Size(204, 27);
            this.btnDeleteRows.TabIndex = 40;
            this.btnDeleteRows.Text = "Delete selected rows";
            this.btnDeleteRows.UseVisualStyleBackColor = true;
            this.btnDeleteRows.Click += new System.EventHandler(this.btnDeleteRows_Click);
            // 
            // btnMergeDups
            // 
            this.btnMergeDups.Enabled = false;
            this.btnMergeDups.Location = new System.Drawing.Point(318, 477);
            this.btnMergeDups.Name = "btnMergeDups";
            this.btnMergeDups.Size = new System.Drawing.Size(204, 27);
            this.btnMergeDups.TabIndex = 41;
            this.btnMergeDups.Text = "Merge duplicate specimens";
            this.btnMergeDups.UseVisualStyleBackColor = true;
            this.btnMergeDups.Click += new System.EventHandler(this.btnMergeDups_Click);
            // 
            // btnCleanRecords
            // 
            this.btnCleanRecords.Location = new System.Drawing.Point(243, 331);
            this.btnCleanRecords.Name = "btnCleanRecords";
            this.btnCleanRecords.Size = new System.Drawing.Size(204, 29);
            this.btnCleanRecords.TabIndex = 42;
            this.btnCleanRecords.Text = "Update missing fields";
            this.btnCleanRecords.UseVisualStyleBackColor = true;
            this.btnCleanRecords.Click += new System.EventHandler(this.btnCleanRecords_Click);
            // 
            // gbCollNumberOptions
            // 
            this.gbCollNumberOptions.Controls.Add(this.rbNone);
            this.gbCollNumberOptions.Controls.Add(this.rbNumberToSN);
            this.gbCollNumberOptions.Controls.Add(this.rbNumberFromBarcode);
            this.gbCollNumberOptions.Location = new System.Drawing.Point(243, 361);
            this.gbCollNumberOptions.Name = "gbCollNumberOptions";
            this.gbCollNumberOptions.Size = new System.Drawing.Size(200, 71);
            this.gbCollNumberOptions.TabIndex = 43;
            this.gbCollNumberOptions.TabStop = false;
            // 
            // rbNone
            // 
            this.rbNone.AutoSize = true;
            this.rbNone.Checked = true;
            this.rbNone.Location = new System.Drawing.Point(7, 48);
            this.rbNone.Name = "rbNone";
            this.rbNone.Size = new System.Drawing.Size(137, 17);
            this.rbNone.TabIndex = 2;
            this.rbNone.TabStop = true;
            this.rbNone.Text = "Don\'t change coll. num.";
            this.rbNone.UseVisualStyleBackColor = true;
            // 
            // rbNumberToSN
            // 
            this.rbNumberToSN.AutoSize = true;
            this.rbNumberToSN.Location = new System.Drawing.Point(7, 30);
            this.rbNumberToSN.Name = "rbNumberToSN";
            this.rbNumberToSN.Size = new System.Drawing.Size(148, 17);
            this.rbNumberToSN.TabIndex = 1;
            this.rbNumberToSN.Text = "Set empty coll. no. to S.N.";
            this.rbNumberToSN.UseVisualStyleBackColor = true;
            // 
            // rbNumberFromBarcode
            // 
            this.rbNumberFromBarcode.AutoSize = true;
            this.rbNumberFromBarcode.Location = new System.Drawing.Point(7, 11);
            this.rbNumberFromBarcode.Name = "rbNumberFromBarcode";
            this.rbNumberFromBarcode.Size = new System.Drawing.Size(164, 17);
            this.rbNumberFromBarcode.TabIndex = 0;
            this.rbNumberFromBarcode.Text = "Add coll. nums from barcodes";
            this.rbNumberFromBarcode.UseVisualStyleBackColor = true;
            // 
            // btnFindErrors
            // 
            this.btnFindErrors.Enabled = false;
            this.btnFindErrors.Location = new System.Drawing.Point(469, 331);
            this.btnFindErrors.Name = "btnFindErrors";
            this.btnFindErrors.Size = new System.Drawing.Size(204, 29);
            this.btnFindErrors.TabIndex = 44;
            this.btnFindErrors.Text = "Find errors";
            this.btnFindErrors.UseVisualStyleBackColor = true;
            this.btnFindErrors.Click += new System.EventHandler(this.btnFindErrors_Click);
            // 
            // btnFindMissingRecords
            // 
            this.btnFindMissingRecords.Enabled = false;
            this.btnFindMissingRecords.Location = new System.Drawing.Point(17, 331);
            this.btnFindMissingRecords.Name = "btnFindMissingRecords";
            this.btnFindMissingRecords.Size = new System.Drawing.Size(204, 29);
            this.btnFindMissingRecords.TabIndex = 45;
            this.btnFindMissingRecords.Text = "Check missing records";
            this.btnFindMissingRecords.UseVisualStyleBackColor = true;
            this.btnFindMissingRecords.Click += new System.EventHandler(this.btnFindMissingRecords_Click);
            // 
            // btnAddQDSFromCoords
            // 
            this.btnAddQDSFromCoords.Enabled = false;
            this.btnAddQDSFromCoords.Location = new System.Drawing.Point(469, 366);
            this.btnAddQDSFromCoords.Name = "btnAddQDSFromCoords";
            this.btnAddQDSFromCoords.Size = new System.Drawing.Size(204, 29);
            this.btnAddQDSFromCoords.TabIndex = 46;
            this.btnAddQDSFromCoords.Text = "Add QDS from coordinates";
            this.btnAddQDSFromCoords.UseVisualStyleBackColor = true;
            this.btnAddQDSFromCoords.Click += new System.EventHandler(this.btnAddQDSFromCoords_Click);
            // 
            // btnCheckQDSCountry
            // 
            this.btnCheckQDSCountry.Enabled = false;
            this.btnCheckQDSCountry.Location = new System.Drawing.Point(469, 403);
            this.btnCheckQDSCountry.Name = "btnCheckQDSCountry";
            this.btnCheckQDSCountry.Size = new System.Drawing.Size(204, 29);
            this.btnCheckQDSCountry.TabIndex = 47;
            this.btnCheckQDSCountry.Text = "Check new QDSs";
            this.btnCheckQDSCountry.UseVisualStyleBackColor = true;
            this.btnCheckQDSCountry.Click += new System.EventHandler(this.btnCheckQDSCountry_Click);
            // 
            // btnNextRowWithErrors
            // 
            this.btnNextRowWithErrors.Enabled = false;
            this.btnNextRowWithErrors.Location = new System.Drawing.Point(954, 401);
            this.btnNextRowWithErrors.Name = "btnNextRowWithErrors";
            this.btnNextRowWithErrors.Size = new System.Drawing.Size(204, 29);
            this.btnNextRowWithErrors.TabIndex = 49;
            this.btnNextRowWithErrors.Text = "Next error...";
            this.btnNextRowWithErrors.UseVisualStyleBackColor = true;
            this.btnNextRowWithErrors.Click += new System.EventHandler(this.btnNextRowWithErrors_Click);
            // 
            // btnSaveChanges
            // 
            this.btnSaveChanges.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSaveChanges.BackgroundImage")));
            this.btnSaveChanges.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnSaveChanges.Location = new System.Drawing.Point(1353, 690);
            this.btnSaveChanges.Name = "btnSaveChanges";
            this.btnSaveChanges.Size = new System.Drawing.Size(88, 70);
            this.btnSaveChanges.TabIndex = 50;
            this.btnSaveChanges.UseVisualStyleBackColor = true;
            this.btnSaveChanges.Click += new System.EventHandler(this.btnSaveChanges_Click);
            // 
            // lblErrorRowIndex
            // 
            this.lblErrorRowIndex.AutoSize = true;
            this.lblErrorRowIndex.Location = new System.Drawing.Point(780, 408);
            this.lblErrorRowIndex.Name = "lblErrorRowIndex";
            this.lblErrorRowIndex.Size = new System.Drawing.Size(0, 13);
            this.lblErrorRowIndex.TabIndex = 51;
            this.lblErrorRowIndex.Visible = false;
            // 
            // lblPeople
            // 
            this.lblPeople.AutoSize = true;
            this.lblPeople.Location = new System.Drawing.Point(343, 266);
            this.lblPeople.Name = "lblPeople";
            this.lblPeople.Size = new System.Drawing.Size(0, 13);
            this.lblPeople.TabIndex = 52;
            // 
            // lblQDSCountries
            // 
            this.lblQDSCountries.AutoSize = true;
            this.lblQDSCountries.Location = new System.Drawing.Point(334, 206);
            this.lblQDSCountries.Name = "lblQDSCountries";
            this.lblQDSCountries.Size = new System.Drawing.Size(0, 13);
            this.lblQDSCountries.TabIndex = 53;
            // 
            // chkAddOrganization
            // 
            this.chkAddOrganization.AutoSize = true;
            this.chkAddOrganization.Location = new System.Drawing.Point(244, 438);
            this.chkAddOrganization.Name = "chkAddOrganization";
            this.chkAddOrganization.Size = new System.Drawing.Size(89, 17);
            this.chkAddOrganization.TabIndex = 54;
            this.chkAddOrganization.Text = "Add to Who: ";
            this.chkAddOrganization.UseVisualStyleBackColor = true;
            // 
            // txtAddWho
            // 
            this.txtAddWho.Location = new System.Drawing.Point(330, 436);
            this.txtAddWho.Name = "txtAddWho";
            this.txtAddWho.Size = new System.Drawing.Size(112, 20);
            this.txtAddWho.TabIndex = 55;
            this.txtAddWho.Text = "NSCF";
            // 
            // btnCheckTaxa
            // 
            this.btnCheckTaxa.Enabled = false;
            this.btnCheckTaxa.Location = new System.Drawing.Point(17, 400);
            this.btnCheckTaxa.Name = "btnCheckTaxa";
            this.btnCheckTaxa.Size = new System.Drawing.Size(204, 29);
            this.btnCheckTaxa.TabIndex = 56;
            this.btnCheckTaxa.Text = "Check taxa";
            this.btnCheckTaxa.UseVisualStyleBackColor = true;
            this.btnCheckTaxa.Click += new System.EventHandler(this.btnCheckTaxa_Click);
            // 
            // btnClearRecords
            // 
            this.btnClearRecords.Enabled = false;
            this.btnClearRecords.Location = new System.Drawing.Point(672, 18);
            this.btnClearRecords.Name = "btnClearRecords";
            this.btnClearRecords.Size = new System.Drawing.Size(79, 29);
            this.btnClearRecords.TabIndex = 57;
            this.btnClearRecords.Text = "Clear";
            this.btnClearRecords.UseVisualStyleBackColor = true;
            this.btnClearRecords.Click += new System.EventHandler(this.btnClearRecords_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1472, 761);
            this.Controls.Add(this.btnClearRecords);
            this.Controls.Add(this.btnCheckTaxa);
            this.Controls.Add(this.txtAddWho);
            this.Controls.Add(this.chkAddOrganization);
            this.Controls.Add(this.lblQDSCountries);
            this.Controls.Add(this.lblPeople);
            this.Controls.Add(this.lblErrorRowIndex);
            this.Controls.Add(this.btnSaveChanges);
            this.Controls.Add(this.btnNextRowWithErrors);
            this.Controls.Add(this.btnCheckQDSCountry);
            this.Controls.Add(this.btnAddQDSFromCoords);
            this.Controls.Add(this.btnFindMissingRecords);
            this.Controls.Add(this.btnFindErrors);
            this.Controls.Add(this.gbCollNumberOptions);
            this.Controls.Add(this.btnCleanRecords);
            this.Controls.Add(this.btnMergeDups);
            this.Controls.Add(this.btnDeleteRows);
            this.Controls.Add(this.lblReportErrors);
            this.Controls.Add(this.rtbReportErrors);
            this.Controls.Add(this.lblDupIndexCount);
            this.Controls.Add(this.btnTestSomething);
            this.Controls.Add(this.lblDupsCount);
            this.Controls.Add(this.btnNextDuplicate);
            this.Controls.Add(this.dgvRecordsView);
            this.Controls.Add(this.txtPeopleTable);
            this.Controls.Add(this.btnChoosePeople);
            this.Controls.Add(this.lblChoosePeople);
            this.Controls.Add(this.txtQDSCountriesFile);
            this.Controls.Add(this.btnChooseQDSCountries);
            this.Controls.Add(this.lblChooseQDSCountries);
            this.Controls.Add(this.lblNoTaxa);
            this.Controls.Add(this.txtTaxonBackbone);
            this.Controls.Add(this.btnChooseTaxonBackbone);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtImageFolder);
            this.Controls.Add(this.btnChooseImageFolder);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnAddDBFData);
            this.Controls.Add(this.lblNumberOfRecords);
            this.Controls.Add(this.btnCheckDuplicates);
            this.Controls.Add(this.txtChooseDBF);
            this.Controls.Add(this.btnChooseDBF);
            this.Controls.Add(this.lblChooseDBF);
            this.Name = "Main";
            this.Text = "RDEManager";
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecordsView)).EndInit();
            this.gbCollNumberOptions.ResumeLayout(false);
            this.gbCollNumberOptions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtChooseDBF;
        private System.Windows.Forms.Button btnChooseDBF;
        private System.Windows.Forms.Label lblChooseDBF;
        private System.Windows.Forms.Button btnCheckDuplicates;
        private System.Windows.Forms.Label lblNumberOfRecords;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Button btnAddDBFData;
        private System.Windows.Forms.TextBox txtImageFolder;
        private System.Windows.Forms.Button btnChooseImageFolder;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtTaxonBackbone;
        private System.Windows.Forms.Button btnChooseTaxonBackbone;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblNoTaxa;
        private System.Windows.Forms.TextBox txtQDSCountriesFile;
        private System.Windows.Forms.Button btnChooseQDSCountries;
        private System.Windows.Forms.Label lblChooseQDSCountries;
        private System.Windows.Forms.TextBox txtPeopleTable;
        private System.Windows.Forms.Button btnChoosePeople;
        private System.Windows.Forms.Label lblChoosePeople;
        private System.Windows.Forms.DataGridView dgvRecordsView;
        private System.Windows.Forms.Button btnNextDuplicate;
        private System.Windows.Forms.Label lblDupsCount;
        private System.Windows.Forms.Button btnTestSomething;
        private System.Windows.Forms.Label lblDupIndexCount;
        private System.Windows.Forms.RichTextBox rtbReportErrors;
        private System.Windows.Forms.Label lblReportErrors;
        private System.Windows.Forms.Button btnDeleteRows;
        private System.Windows.Forms.Button btnMergeDups;
        private System.Windows.Forms.Button btnCleanRecords;
        private System.Windows.Forms.GroupBox gbCollNumberOptions;
        private System.Windows.Forms.RadioButton rbNumberToSN;
        private System.Windows.Forms.RadioButton rbNumberFromBarcode;
        private System.Windows.Forms.RadioButton rbNone;
        private System.Windows.Forms.Button btnFindErrors;
        private System.Windows.Forms.Button btnFindMissingRecords;
        private System.Windows.Forms.Button btnAddQDSFromCoords;
        private System.Windows.Forms.Button btnCheckQDSCountry;
        private System.Windows.Forms.Button btnNextRowWithErrors;
        private System.Windows.Forms.Button btnSaveChanges;
        private System.Windows.Forms.Label lblErrorRowIndex;
        private System.Windows.Forms.Label lblPeople;
        private System.Windows.Forms.Label lblQDSCountries;
        private System.Windows.Forms.CheckBox chkAddOrganization;
        private System.Windows.Forms.TextBox txtAddWho;
        private System.Windows.Forms.Button btnCheckTaxa;
        private System.Windows.Forms.Button btnClearRecords;
    }
}

