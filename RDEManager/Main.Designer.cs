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
            this.btnCheckRDEFile = new System.Windows.Forms.Button();
            this.lblNumberOfRecords = new System.Windows.Forms.Label();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.btnAddDBFData = new System.Windows.Forms.Button();
            this.lbRDEErrors = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtImageFolder = new System.Windows.Forms.TextBox();
            this.btnChooseImageFolder = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtTaxonBackbone = new System.Windows.Forms.TextBox();
            this.btnChooseTaxonBackbone = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.lblNoTaxa = new System.Windows.Forms.Label();
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
            this.btnChooseDBF.Location = new System.Drawing.Point(210, 18);
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
            this.lblChooseDBF.Size = new System.Drawing.Size(192, 25);
            this.lblChooseDBF.TabIndex = 8;
            this.lblChooseDBF.Text = "Choose the DBF file:";
            // 
            // btnCheckRDEFile
            // 
            this.btnCheckRDEFile.Enabled = false;
            this.btnCheckRDEFile.Location = new System.Drawing.Point(17, 317);
            this.btnCheckRDEFile.Name = "btnCheckRDEFile";
            this.btnCheckRDEFile.Size = new System.Drawing.Size(204, 29);
            this.btnCheckRDEFile.TabIndex = 11;
            this.btnCheckRDEFile.Text = "Check the records";
            this.btnCheckRDEFile.UseVisualStyleBackColor = true;
            this.btnCheckRDEFile.Click += new System.EventHandler(this.btnCheckRecords_Click);
            // 
            // lblNumberOfRecords
            // 
            this.lblNumberOfRecords.AutoSize = true;
            this.lblNumberOfRecords.Location = new System.Drawing.Point(238, 104);
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
            this.btnAddDBFData.Location = new System.Drawing.Point(17, 96);
            this.btnAddDBFData.Name = "btnAddDBFData";
            this.btnAddDBFData.Size = new System.Drawing.Size(204, 29);
            this.btnAddDBFData.TabIndex = 13;
            this.btnAddDBFData.Text = "Add selected RDE file/s";
            this.btnAddDBFData.UseVisualStyleBackColor = true;
            this.btnAddDBFData.Click += new System.EventHandler(this.addFile_Click);
            // 
            // lbRDEErrors
            // 
            this.lbRDEErrors.FormattingEnabled = true;
            this.lbRDEErrors.Location = new System.Drawing.Point(296, 317);
            this.lbRDEErrors.Name = "lbRDEErrors";
            this.lbRDEErrors.Size = new System.Drawing.Size(455, 121);
            this.lbRDEErrors.TabIndex = 17;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(296, 298);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(105, 13);
            this.label2.TabIndex = 18;
            this.label2.Text = "RDE Errors detected";
            // 
            // txtImageFolder
            // 
            this.txtImageFolder.Location = new System.Drawing.Point(17, 247);
            this.txtImageFolder.Name = "txtImageFolder";
            this.txtImageFolder.Size = new System.Drawing.Size(734, 20);
            this.txtImageFolder.TabIndex = 21;
            // 
            // btnChooseImageFolder
            // 
            this.btnChooseImageFolder.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnChooseImageFolder.BackgroundImage")));
            this.btnChooseImageFolder.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnChooseImageFolder.Location = new System.Drawing.Point(172, 214);
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
            this.label3.Location = new System.Drawing.Point(12, 219);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(154, 25);
            this.label3.TabIndex = 19;
            this.label3.Text = "Image file folder:";
            // 
            // txtTaxonBackbone
            // 
            this.txtTaxonBackbone.Location = new System.Drawing.Point(17, 484);
            this.txtTaxonBackbone.Name = "txtTaxonBackbone";
            this.txtTaxonBackbone.Size = new System.Drawing.Size(734, 20);
            this.txtTaxonBackbone.TabIndex = 24;
            // 
            // btnChooseTaxonBackbone
            // 
            this.btnChooseTaxonBackbone.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnChooseTaxonBackbone.BackgroundImage")));
            this.btnChooseTaxonBackbone.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnChooseTaxonBackbone.Location = new System.Drawing.Point(281, 451);
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
            this.label4.Location = new System.Drawing.Point(12, 456);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(263, 25);
            this.label4.TabIndex = 22;
            this.label4.Text = "Choose the taxon backbone:";
            // 
            // lblNoTaxa
            // 
            this.lblNoTaxa.AutoSize = true;
            this.lblNoTaxa.Location = new System.Drawing.Point(343, 460);
            this.lblNoTaxa.Name = "lblNoTaxa";
            this.lblNoTaxa.Size = new System.Drawing.Size(119, 13);
            this.lblNoTaxa.TabIndex = 25;
            this.lblNoTaxa.Text = "No. of species records: ";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 626);
            this.Controls.Add(this.lblNoTaxa);
            this.Controls.Add(this.txtTaxonBackbone);
            this.Controls.Add(this.btnChooseTaxonBackbone);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtImageFolder);
            this.Controls.Add(this.btnChooseImageFolder);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lbRDEErrors);
            this.Controls.Add(this.btnAddDBFData);
            this.Controls.Add(this.lblNumberOfRecords);
            this.Controls.Add(this.btnCheckRDEFile);
            this.Controls.Add(this.txtChooseDBF);
            this.Controls.Add(this.btnChooseDBF);
            this.Controls.Add(this.lblChooseDBF);
            this.Name = "Main";
            this.Text = "RDEManager";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtChooseDBF;
        private System.Windows.Forms.Button btnChooseDBF;
        private System.Windows.Forms.Label lblChooseDBF;
        private System.Windows.Forms.Button btnCheckRDEFile;
        private System.Windows.Forms.Label lblNumberOfRecords;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Button btnAddDBFData;
        private System.Windows.Forms.ListBox lbRDEErrors;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtImageFolder;
        private System.Windows.Forms.Button btnChooseImageFolder;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtTaxonBackbone;
        private System.Windows.Forms.Button btnChooseTaxonBackbone;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblNoTaxa;
    }
}

