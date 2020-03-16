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
            this.lblDupIndexCount = new System.Windows.Forms.Label();
            this.rtbReportErrors = new System.Windows.Forms.RichTextBox();
            this.lblReportErrors = new System.Windows.Forms.Label();
            this.btnDeleteRows = new System.Windows.Forms.Button();
            this.btnMergeDups = new System.Windows.Forms.Button();
            this.btnUpdateMissingData = new System.Windows.Forms.Button();
            this.gbCollNumberOptions = new System.Windows.Forms.GroupBox();
            this.rbNone = new System.Windows.Forms.RadioButton();
            this.rbNumberToSN = new System.Windows.Forms.RadioButton();
            this.rbNumberFromBarcode = new System.Windows.Forms.RadioButton();
            this.btnFindQDSCoordErrors = new System.Windows.Forms.Button();
            this.btnFindMissingRecords = new System.Windows.Forms.Button();
            this.btnNextRowWithErrors = new System.Windows.Forms.Button();
            this.btnSaveChanges = new System.Windows.Forms.Button();
            this.lblErrorRowIndex = new System.Windows.Forms.Label();
            this.lblPeople = new System.Windows.Forms.Label();
            this.lblQDSCountries = new System.Windows.Forms.Label();
            this.chkAddOrganization = new System.Windows.Forms.CheckBox();
            this.txtAddWho = new System.Windows.Forms.TextBox();
            this.btnCheckTaxa = new System.Windows.Forms.Button();
            this.btnClearRecords = new System.Windows.Forms.Button();
            this.btnFindCaptureErrors = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.btnShowImage = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnChooseSchema = new System.Windows.Forms.Button();
            this.txtSchemaFile = new System.Windows.Forms.TextBox();
            this.lblSchemaFields = new System.Windows.Forms.Label();
            this.btnConfirmPeople = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecordsView)).BeginInit();
            this.gbCollNumberOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.SuspendLayout();
            // 
            // txtChooseDBF
            // 
            this.txtChooseDBF.Location = new System.Drawing.Point(18, 120);
            this.txtChooseDBF.Name = "txtChooseDBF";
            this.txtChooseDBF.Size = new System.Drawing.Size(734, 20);
            this.txtChooseDBF.TabIndex = 10;
            // 
            // btnChooseDBF
            // 
            this.btnChooseDBF.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnChooseDBF.BackgroundImage")));
            this.btnChooseDBF.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnChooseDBF.Location = new System.Drawing.Point(217, 86);
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
            this.lblChooseDBF.Location = new System.Drawing.Point(13, 92);
            this.lblChooseDBF.Name = "lblChooseDBF";
            this.lblChooseDBF.Size = new System.Drawing.Size(203, 25);
            this.lblChooseDBF.TabIndex = 8;
            this.lblChooseDBF.Text = "Choose the RDE files:";
            // 
            // btnCheckDuplicates
            // 
            this.btnCheckDuplicates.Enabled = false;
            this.btnCheckDuplicates.Location = new System.Drawing.Point(18, 435);
            this.btnCheckDuplicates.Name = "btnCheckDuplicates";
            this.btnCheckDuplicates.Size = new System.Drawing.Size(149, 29);
            this.btnCheckDuplicates.TabIndex = 11;
            this.btnCheckDuplicates.Text = "Check duplicates";
            this.btnCheckDuplicates.UseVisualStyleBackColor = true;
            this.btnCheckDuplicates.Click += new System.EventHandler(this.btnCheckDuplicates_Click);
            // 
            // lblNumberOfRecords
            // 
            this.lblNumberOfRecords.AutoSize = true;
            this.lblNumberOfRecords.Location = new System.Drawing.Point(503, 95);
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
            this.btnAddDBFData.Location = new System.Drawing.Point(282, 87);
            this.btnAddDBFData.Name = "btnAddDBFData";
            this.btnAddDBFData.Size = new System.Drawing.Size(204, 29);
            this.btnAddDBFData.TabIndex = 13;
            this.btnAddDBFData.Text = "Import records from these files";
            this.btnAddDBFData.UseVisualStyleBackColor = true;
            this.btnAddDBFData.Click += new System.EventHandler(this.addFile_Click);
            // 
            // txtImageFolder
            // 
            this.txtImageFolder.Location = new System.Drawing.Point(18, 181);
            this.txtImageFolder.Name = "txtImageFolder";
            this.txtImageFolder.Size = new System.Drawing.Size(734, 20);
            this.txtImageFolder.TabIndex = 21;
            // 
            // btnChooseImageFolder
            // 
            this.btnChooseImageFolder.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnChooseImageFolder.BackgroundImage")));
            this.btnChooseImageFolder.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnChooseImageFolder.Location = new System.Drawing.Point(173, 148);
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
            this.label3.Location = new System.Drawing.Point(13, 153);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(154, 25);
            this.label3.TabIndex = 19;
            this.label3.Text = "Image file folder:";
            // 
            // txtTaxonBackbone
            // 
            this.txtTaxonBackbone.Location = new System.Drawing.Point(18, 240);
            this.txtTaxonBackbone.Name = "txtTaxonBackbone";
            this.txtTaxonBackbone.Size = new System.Drawing.Size(734, 20);
            this.txtTaxonBackbone.TabIndex = 24;
            // 
            // btnChooseTaxonBackbone
            // 
            this.btnChooseTaxonBackbone.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnChooseTaxonBackbone.BackgroundImage")));
            this.btnChooseTaxonBackbone.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnChooseTaxonBackbone.Location = new System.Drawing.Point(282, 207);
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
            this.label4.Location = new System.Drawing.Point(13, 212);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(263, 25);
            this.label4.TabIndex = 22;
            this.label4.Text = "Choose the taxon backbone:";
            // 
            // lblNoTaxa
            // 
            this.lblNoTaxa.AutoSize = true;
            this.lblNoTaxa.Location = new System.Drawing.Point(344, 216);
            this.lblNoTaxa.Name = "lblNoTaxa";
            this.lblNoTaxa.Size = new System.Drawing.Size(0, 13);
            this.lblNoTaxa.TabIndex = 25;
            // 
            // txtQDSCountriesFile
            // 
            this.txtQDSCountriesFile.Location = new System.Drawing.Point(18, 300);
            this.txtQDSCountriesFile.Name = "txtQDSCountriesFile";
            this.txtQDSCountriesFile.Size = new System.Drawing.Size(734, 20);
            this.txtQDSCountriesFile.TabIndex = 29;
            // 
            // btnChooseQDSCountries
            // 
            this.btnChooseQDSCountries.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnChooseQDSCountries.BackgroundImage")));
            this.btnChooseQDSCountries.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnChooseQDSCountries.Enabled = false;
            this.btnChooseQDSCountries.Location = new System.Drawing.Point(282, 267);
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
            this.lblChooseQDSCountries.Location = new System.Drawing.Point(13, 272);
            this.lblChooseQDSCountries.Name = "lblChooseQDSCountries";
            this.lblChooseQDSCountries.Size = new System.Drawing.Size(248, 25);
            this.lblChooseQDSCountries.TabIndex = 27;
            this.lblChooseQDSCountries.Text = "Choose QDS countries list:";
            // 
            // txtPeopleTable
            // 
            this.txtPeopleTable.Location = new System.Drawing.Point(18, 359);
            this.txtPeopleTable.Name = "txtPeopleTable";
            this.txtPeopleTable.Size = new System.Drawing.Size(734, 20);
            this.txtPeopleTable.TabIndex = 32;
            // 
            // btnChoosePeople
            // 
            this.btnChoosePeople.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnChoosePeople.BackgroundImage")));
            this.btnChoosePeople.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnChoosePeople.Enabled = false;
            this.btnChoosePeople.Location = new System.Drawing.Point(282, 326);
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
            this.lblChoosePeople.Location = new System.Drawing.Point(13, 331);
            this.lblChoosePeople.Name = "lblChoosePeople";
            this.lblChoosePeople.Size = new System.Drawing.Size(230, 25);
            this.lblChoosePeople.TabIndex = 30;
            this.lblChoosePeople.Text = "Choose the people table:";
            // 
            // dgvRecordsView
            // 
            this.dgvRecordsView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRecordsView.Location = new System.Drawing.Point(21, 579);
            this.dgvRecordsView.Name = "dgvRecordsView";
            this.dgvRecordsView.Size = new System.Drawing.Size(1138, 171);
            this.dgvRecordsView.TabIndex = 33;
            this.dgvRecordsView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvRecordsView_KeyDown);
            // 
            // btnNextDuplicate
            // 
            this.btnNextDuplicate.Enabled = false;
            this.btnNextDuplicate.Location = new System.Drawing.Point(109, 546);
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
            this.lblDupsCount.Location = new System.Drawing.Point(18, 553);
            this.lblDupsCount.Name = "lblDupsCount";
            this.lblDupsCount.Size = new System.Drawing.Size(58, 13);
            this.lblDupsCount.TabIndex = 35;
            this.lblDupsCount.Text = "No. Dups: ";
            // 
            // lblDupIndexCount
            // 
            this.lblDupIndexCount.AutoSize = true;
            this.lblDupIndexCount.Location = new System.Drawing.Point(394, 433);
            this.lblDupIndexCount.Name = "lblDupIndexCount";
            this.lblDupIndexCount.Size = new System.Drawing.Size(0, 13);
            this.lblDupIndexCount.TabIndex = 37;
            // 
            // rtbReportErrors
            // 
            this.rtbReportErrors.Location = new System.Drawing.Point(774, 397);
            this.rtbReportErrors.Name = "rtbReportErrors";
            this.rtbReportErrors.Size = new System.Drawing.Size(385, 84);
            this.rtbReportErrors.TabIndex = 38;
            this.rtbReportErrors.Text = "";
            // 
            // lblReportErrors
            // 
            this.lblReportErrors.AutoSize = true;
            this.lblReportErrors.Location = new System.Drawing.Point(781, 381);
            this.lblReportErrors.Name = "lblReportErrors";
            this.lblReportErrors.Size = new System.Drawing.Size(76, 13);
            this.lblReportErrors.TabIndex = 39;
            this.lblReportErrors.Text = "Reports/Errors";
            // 
            // btnDeleteRows
            // 
            this.btnDeleteRows.Enabled = false;
            this.btnDeleteRows.Location = new System.Drawing.Point(529, 546);
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
            this.btnMergeDups.Location = new System.Drawing.Point(319, 546);
            this.btnMergeDups.Name = "btnMergeDups";
            this.btnMergeDups.Size = new System.Drawing.Size(204, 27);
            this.btnMergeDups.TabIndex = 41;
            this.btnMergeDups.Text = "Merge duplicate specimens";
            this.btnMergeDups.UseVisualStyleBackColor = true;
            this.btnMergeDups.Click += new System.EventHandler(this.btnMergeDups_Click);
            // 
            // btnUpdateMissingData
            // 
            this.btnUpdateMissingData.Enabled = false;
            this.btnUpdateMissingData.Location = new System.Drawing.Point(404, 486);
            this.btnUpdateMissingData.Name = "btnUpdateMissingData";
            this.btnUpdateMissingData.Size = new System.Drawing.Size(149, 29);
            this.btnUpdateMissingData.TabIndex = 42;
            this.btnUpdateMissingData.Text = "Update missing fields";
            this.btnUpdateMissingData.UseVisualStyleBackColor = true;
            this.btnUpdateMissingData.Click += new System.EventHandler(this.btnUpdateMissingData_Click);
            // 
            // gbCollNumberOptions
            // 
            this.gbCollNumberOptions.Controls.Add(this.rbNone);
            this.gbCollNumberOptions.Controls.Add(this.rbNumberToSN);
            this.gbCollNumberOptions.Controls.Add(this.rbNumberFromBarcode);
            this.gbCollNumberOptions.Location = new System.Drawing.Point(397, 386);
            this.gbCollNumberOptions.Name = "gbCollNumberOptions";
            this.gbCollNumberOptions.Size = new System.Drawing.Size(167, 71);
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
            // btnFindQDSCoordErrors
            // 
            this.btnFindQDSCoordErrors.Enabled = false;
            this.btnFindQDSCoordErrors.Location = new System.Drawing.Point(603, 434);
            this.btnFindQDSCoordErrors.Name = "btnFindQDSCoordErrors";
            this.btnFindQDSCoordErrors.Size = new System.Drawing.Size(149, 29);
            this.btnFindQDSCoordErrors.TabIndex = 44;
            this.btnFindQDSCoordErrors.Text = "Check coords and QDSs";
            this.btnFindQDSCoordErrors.UseVisualStyleBackColor = true;
            this.btnFindQDSCoordErrors.Click += new System.EventHandler(this.btnFindQDSCoordErrors_Click);
            // 
            // btnFindMissingRecords
            // 
            this.btnFindMissingRecords.Enabled = false;
            this.btnFindMissingRecords.Location = new System.Drawing.Point(18, 400);
            this.btnFindMissingRecords.Name = "btnFindMissingRecords";
            this.btnFindMissingRecords.Size = new System.Drawing.Size(149, 29);
            this.btnFindMissingRecords.TabIndex = 45;
            this.btnFindMissingRecords.Text = "Check missing records";
            this.btnFindMissingRecords.UseVisualStyleBackColor = true;
            this.btnFindMissingRecords.Click += new System.EventHandler(this.btnFindMissingRecords_Click);
            // 
            // btnNextRowWithErrors
            // 
            this.btnNextRowWithErrors.Enabled = false;
            this.btnNextRowWithErrors.Location = new System.Drawing.Point(1000, 486);
            this.btnNextRowWithErrors.Name = "btnNextRowWithErrors";
            this.btnNextRowWithErrors.Size = new System.Drawing.Size(159, 29);
            this.btnNextRowWithErrors.TabIndex = 49;
            this.btnNextRowWithErrors.Text = "Next error...";
            this.btnNextRowWithErrors.UseVisualStyleBackColor = true;
            this.btnNextRowWithErrors.Click += new System.EventHandler(this.btnNextRowWithErrors_Click);
            // 
            // btnSaveChanges
            // 
            this.btnSaveChanges.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSaveChanges.BackgroundImage")));
            this.btnSaveChanges.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnSaveChanges.Location = new System.Drawing.Point(1071, 756);
            this.btnSaveChanges.Name = "btnSaveChanges";
            this.btnSaveChanges.Size = new System.Drawing.Size(88, 70);
            this.btnSaveChanges.TabIndex = 50;
            this.btnSaveChanges.UseVisualStyleBackColor = true;
            this.btnSaveChanges.Click += new System.EventHandler(this.btnSaveChanges_Click);
            // 
            // lblErrorRowIndex
            // 
            this.lblErrorRowIndex.AutoSize = true;
            this.lblErrorRowIndex.Location = new System.Drawing.Point(781, 530);
            this.lblErrorRowIndex.Name = "lblErrorRowIndex";
            this.lblErrorRowIndex.Size = new System.Drawing.Size(0, 13);
            this.lblErrorRowIndex.TabIndex = 51;
            this.lblErrorRowIndex.Visible = false;
            // 
            // lblPeople
            // 
            this.lblPeople.AutoSize = true;
            this.lblPeople.Location = new System.Drawing.Point(344, 335);
            this.lblPeople.Name = "lblPeople";
            this.lblPeople.Size = new System.Drawing.Size(0, 13);
            this.lblPeople.TabIndex = 52;
            // 
            // lblQDSCountries
            // 
            this.lblQDSCountries.AutoSize = true;
            this.lblQDSCountries.Location = new System.Drawing.Point(335, 275);
            this.lblQDSCountries.Name = "lblQDSCountries";
            this.lblQDSCountries.Size = new System.Drawing.Size(0, 13);
            this.lblQDSCountries.TabIndex = 53;
            // 
            // chkAddOrganization
            // 
            this.chkAddOrganization.AutoSize = true;
            this.chkAddOrganization.Location = new System.Drawing.Point(398, 463);
            this.chkAddOrganization.Name = "chkAddOrganization";
            this.chkAddOrganization.Size = new System.Drawing.Size(89, 17);
            this.chkAddOrganization.TabIndex = 54;
            this.chkAddOrganization.Text = "Add to Who: ";
            this.chkAddOrganization.UseVisualStyleBackColor = true;
            // 
            // txtAddWho
            // 
            this.txtAddWho.Location = new System.Drawing.Point(484, 461);
            this.txtAddWho.Name = "txtAddWho";
            this.txtAddWho.Size = new System.Drawing.Size(80, 20);
            this.txtAddWho.TabIndex = 55;
            this.txtAddWho.Text = "NSCF";
            // 
            // btnCheckTaxa
            // 
            this.btnCheckTaxa.Enabled = false;
            this.btnCheckTaxa.Location = new System.Drawing.Point(18, 469);
            this.btnCheckTaxa.Name = "btnCheckTaxa";
            this.btnCheckTaxa.Size = new System.Drawing.Size(149, 29);
            this.btnCheckTaxa.TabIndex = 56;
            this.btnCheckTaxa.Text = "Check taxa";
            this.btnCheckTaxa.UseVisualStyleBackColor = true;
            this.btnCheckTaxa.Click += new System.EventHandler(this.btnCheckTaxa_Click);
            // 
            // btnClearRecords
            // 
            this.btnClearRecords.Enabled = false;
            this.btnClearRecords.Location = new System.Drawing.Point(673, 87);
            this.btnClearRecords.Name = "btnClearRecords";
            this.btnClearRecords.Size = new System.Drawing.Size(79, 29);
            this.btnClearRecords.TabIndex = 57;
            this.btnClearRecords.Text = "Clear";
            this.btnClearRecords.UseVisualStyleBackColor = true;
            this.btnClearRecords.Click += new System.EventHandler(this.btnClearRecords_Click);
            // 
            // btnFindCaptureErrors
            // 
            this.btnFindCaptureErrors.Enabled = false;
            this.btnFindCaptureErrors.Location = new System.Drawing.Point(207, 435);
            this.btnFindCaptureErrors.Name = "btnFindCaptureErrors";
            this.btnFindCaptureErrors.Size = new System.Drawing.Size(149, 29);
            this.btnFindCaptureErrors.TabIndex = 58;
            this.btnFindCaptureErrors.Text = "Check capture errors";
            this.btnFindCaptureErrors.UseVisualStyleBackColor = true;
            this.btnFindCaptureErrors.Click += new System.EventHandler(this.btnFindCaptureErrors_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::RDEManager.Properties.Resources.rightarrow;
            this.pictureBox1.Location = new System.Drawing.Point(178, 435);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(29, 44);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 59;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::RDEManager.Properties.Resources.rightarrow;
            this.pictureBox2.Location = new System.Drawing.Point(363, 433);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(29, 44);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 60;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::RDEManager.Properties.Resources.rightarrow;
            this.pictureBox3.Location = new System.Drawing.Point(571, 433);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(29, 44);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox3.TabIndex = 61;
            this.pictureBox3.TabStop = false;
            // 
            // btnShowImage
            // 
            this.btnShowImage.Enabled = false;
            this.btnShowImage.Location = new System.Drawing.Point(1000, 522);
            this.btnShowImage.Name = "btnShowImage";
            this.btnShowImage.Size = new System.Drawing.Size(159, 29);
            this.btnShowImage.TabIndex = 62;
            this.btnShowImage.Text = "Show image...";
            this.btnShowImage.UseVisualStyleBackColor = true;
            this.btnShowImage.Click += new System.EventHandler(this.btnShowImage_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(287, 25);
            this.label1.TabIndex = 63;
            this.label1.Text = "Select the schema template file:";
            // 
            // btnChooseSchema
            // 
            this.btnChooseSchema.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnChooseSchema.BackgroundImage")));
            this.btnChooseSchema.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnChooseSchema.Location = new System.Drawing.Point(301, 35);
            this.btnChooseSchema.Name = "btnChooseSchema";
            this.btnChooseSchema.Size = new System.Drawing.Size(43, 30);
            this.btnChooseSchema.TabIndex = 64;
            this.btnChooseSchema.UseVisualStyleBackColor = true;
            this.btnChooseSchema.Click += new System.EventHandler(this.btnChooseSchema_Click);
            // 
            // txtSchemaFile
            // 
            this.txtSchemaFile.Location = new System.Drawing.Point(350, 40);
            this.txtSchemaFile.Name = "txtSchemaFile";
            this.txtSchemaFile.Size = new System.Drawing.Size(389, 20);
            this.txtSchemaFile.TabIndex = 65;
            // 
            // lblSchemaFields
            // 
            this.lblSchemaFields.AutoSize = true;
            this.lblSchemaFields.Location = new System.Drawing.Point(745, 43);
            this.lblSchemaFields.Name = "lblSchemaFields";
            this.lblSchemaFields.Size = new System.Drawing.Size(0, 13);
            this.lblSchemaFields.TabIndex = 66;
            // 
            // btnConfirmPeople
            // 
            this.btnConfirmPeople.Enabled = false;
            this.btnConfirmPeople.Location = new System.Drawing.Point(774, 486);
            this.btnConfirmPeople.Name = "btnConfirmPeople";
            this.btnConfirmPeople.Size = new System.Drawing.Size(159, 29);
            this.btnConfirmPeople.TabIndex = 67;
            this.btnConfirmPeople.Text = "These names are correct...";
            this.btnConfirmPeople.UseVisualStyleBackColor = true;
            this.btnConfirmPeople.Click += new System.EventHandler(this.btnConfirmPeople_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1171, 838);
            this.Controls.Add(this.btnConfirmPeople);
            this.Controls.Add(this.lblSchemaFields);
            this.Controls.Add(this.txtSchemaFile);
            this.Controls.Add(this.btnChooseSchema);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnShowImage);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnFindCaptureErrors);
            this.Controls.Add(this.btnClearRecords);
            this.Controls.Add(this.btnCheckTaxa);
            this.Controls.Add(this.txtAddWho);
            this.Controls.Add(this.chkAddOrganization);
            this.Controls.Add(this.lblQDSCountries);
            this.Controls.Add(this.lblPeople);
            this.Controls.Add(this.lblErrorRowIndex);
            this.Controls.Add(this.btnSaveChanges);
            this.Controls.Add(this.btnNextRowWithErrors);
            this.Controls.Add(this.btnFindMissingRecords);
            this.Controls.Add(this.btnFindQDSCoordErrors);
            this.Controls.Add(this.gbCollNumberOptions);
            this.Controls.Add(this.btnUpdateMissingData);
            this.Controls.Add(this.btnMergeDups);
            this.Controls.Add(this.btnDeleteRows);
            this.Controls.Add(this.lblReportErrors);
            this.Controls.Add(this.rtbReportErrors);
            this.Controls.Add(this.lblDupIndexCount);
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
            this.Text = "RDE Cleaner";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecordsView)).EndInit();
            this.gbCollNumberOptions.ResumeLayout(false);
            this.gbCollNumberOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
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
        private System.Windows.Forms.Label lblDupIndexCount;
        private System.Windows.Forms.RichTextBox rtbReportErrors;
        private System.Windows.Forms.Label lblReportErrors;
        private System.Windows.Forms.Button btnDeleteRows;
        private System.Windows.Forms.Button btnMergeDups;
        private System.Windows.Forms.Button btnUpdateMissingData;
        private System.Windows.Forms.GroupBox gbCollNumberOptions;
        private System.Windows.Forms.RadioButton rbNumberToSN;
        private System.Windows.Forms.RadioButton rbNumberFromBarcode;
        private System.Windows.Forms.RadioButton rbNone;
        private System.Windows.Forms.Button btnFindQDSCoordErrors;
        private System.Windows.Forms.Button btnFindMissingRecords;
        private System.Windows.Forms.Button btnNextRowWithErrors;
        private System.Windows.Forms.Button btnSaveChanges;
        private System.Windows.Forms.Label lblErrorRowIndex;
        private System.Windows.Forms.Label lblPeople;
        private System.Windows.Forms.Label lblQDSCountries;
        private System.Windows.Forms.CheckBox chkAddOrganization;
        private System.Windows.Forms.TextBox txtAddWho;
        private System.Windows.Forms.Button btnCheckTaxa;
        private System.Windows.Forms.Button btnClearRecords;
        private System.Windows.Forms.Button btnFindCaptureErrors;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.Button btnShowImage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnChooseSchema;
        private System.Windows.Forms.TextBox txtSchemaFile;
        private System.Windows.Forms.Label lblSchemaFields;
        private System.Windows.Forms.Button btnConfirmPeople;
    }
}

