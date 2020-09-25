namespace RDEManager
{
    partial class FixCollectors
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
            this.components = new System.ComponentModel.Container();
            this.lblAgents = new System.Windows.Forms.Label();
            this.lbAgents = new System.Windows.Forms.ListBox();
            this.lblCollectorName = new System.Windows.Forms.Label();
            this.txtAgentName = new System.Windows.Forms.TextBox();
            this.lblNumRecords = new System.Windows.Forms.Label();
            this.txtNumRecords = new System.Windows.Forms.TextBox();
            this.lblCorrectedName = new System.Windows.Forms.Label();
            this.btnSaveChanges = new System.Windows.Forms.Button();
            this.tmrUpdateNamesList = new System.Windows.Forms.Timer(this.components);
            this.lstCorrectedNameOptions = new System.Windows.Forms.ListBox();
            this.txtCorrectedName = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lblAgents
            // 
            this.lblAgents.AutoSize = true;
            this.lblAgents.Location = new System.Drawing.Point(24, 12);
            this.lblAgents.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblAgents.Name = "lblAgents";
            this.lblAgents.Size = new System.Drawing.Size(117, 13);
            this.lblAgents.TabIndex = 1;
            this.lblAgents.Text = "Collectors/Determiners:";
            // 
            // lbAgents
            // 
            this.lbAgents.FormattingEnabled = true;
            this.lbAgents.Location = new System.Drawing.Point(27, 27);
            this.lbAgents.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.lbAgents.Name = "lbAgents";
            this.lbAgents.Size = new System.Drawing.Size(270, 342);
            this.lbAgents.TabIndex = 2;
            this.lbAgents.SelectedIndexChanged += new System.EventHandler(this.lbCollectors_SelectedIndexChanged);
            // 
            // lblCollectorName
            // 
            this.lblCollectorName.AutoSize = true;
            this.lblCollectorName.Location = new System.Drawing.Point(367, 35);
            this.lblCollectorName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCollectorName.Name = "lblCollectorName";
            this.lblCollectorName.Size = new System.Drawing.Size(38, 13);
            this.lblCollectorName.TabIndex = 3;
            this.lblCollectorName.Text = "Name:";
            // 
            // txtAgentName
            // 
            this.txtAgentName.Enabled = false;
            this.txtAgentName.Location = new System.Drawing.Point(408, 33);
            this.txtAgentName.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtAgentName.Name = "txtAgentName";
            this.txtAgentName.Size = new System.Drawing.Size(208, 20);
            this.txtAgentName.TabIndex = 4;
            // 
            // lblNumRecords
            // 
            this.lblNumRecords.AutoSize = true;
            this.lblNumRecords.Location = new System.Drawing.Point(355, 68);
            this.lblNumRecords.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblNumRecords.Name = "lblNumRecords";
            this.lblNumRecords.Size = new System.Drawing.Size(50, 13);
            this.lblNumRecords.TabIndex = 5;
            this.lblNumRecords.Text = "Records:";
            // 
            // txtNumRecords
            // 
            this.txtNumRecords.Enabled = false;
            this.txtNumRecords.Location = new System.Drawing.Point(408, 66);
            this.txtNumRecords.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtNumRecords.Name = "txtNumRecords";
            this.txtNumRecords.Size = new System.Drawing.Size(208, 20);
            this.txtNumRecords.TabIndex = 6;
            // 
            // lblCorrectedName
            // 
            this.lblCorrectedName.AutoSize = true;
            this.lblCorrectedName.Location = new System.Drawing.Point(318, 102);
            this.lblCorrectedName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCorrectedName.Name = "lblCorrectedName";
            this.lblCorrectedName.Size = new System.Drawing.Size(87, 13);
            this.lblCorrectedName.TabIndex = 8;
            this.lblCorrectedName.Text = "Corrected Name:";
            // 
            // btnSaveChanges
            // 
            this.btnSaveChanges.Location = new System.Drawing.Point(481, 343);
            this.btnSaveChanges.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnSaveChanges.Name = "btnSaveChanges";
            this.btnSaveChanges.Size = new System.Drawing.Size(134, 25);
            this.btnSaveChanges.TabIndex = 9;
            this.btnSaveChanges.Text = "Save changes";
            this.btnSaveChanges.UseVisualStyleBackColor = true;
            this.btnSaveChanges.Click += new System.EventHandler(this.btnSaveChanges_Click);
            // 
            // tmrUpdateNamesList
            // 
            this.tmrUpdateNamesList.Interval = 500;
            this.tmrUpdateNamesList.Tick += new System.EventHandler(this.tmrUpdateNamesList_Tick);
            // 
            // lstCorrectedNameOptions
            // 
            this.lstCorrectedNameOptions.FormattingEnabled = true;
            this.lstCorrectedNameOptions.Location = new System.Drawing.Point(408, 120);
            this.lstCorrectedNameOptions.Name = "lstCorrectedNameOptions";
            this.lstCorrectedNameOptions.Size = new System.Drawing.Size(208, 199);
            this.lstCorrectedNameOptions.TabIndex = 10;
            this.lstCorrectedNameOptions.SelectedIndexChanged += new System.EventHandler(this.lstCorrectedNameOptions_SelectedIndexChanged);
            // 
            // txtCorrectedName
            // 
            this.txtCorrectedName.Location = new System.Drawing.Point(408, 97);
            this.txtCorrectedName.Name = "txtCorrectedName";
            this.txtCorrectedName.Size = new System.Drawing.Size(208, 20);
            this.txtCorrectedName.TabIndex = 11;
            this.txtCorrectedName.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtCorrectedName_KeyUp);
            // 
            // FixCollectors
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 378);
            this.Controls.Add(this.txtCorrectedName);
            this.Controls.Add(this.lstCorrectedNameOptions);
            this.Controls.Add(this.btnSaveChanges);
            this.Controls.Add(this.lblCorrectedName);
            this.Controls.Add(this.txtNumRecords);
            this.Controls.Add(this.lblNumRecords);
            this.Controls.Add(this.txtAgentName);
            this.Controls.Add(this.lblCollectorName);
            this.Controls.Add(this.lbAgents);
            this.Controls.Add(this.lblAgents);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "FixCollectors";
            this.Text = "FixCollectors";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblAgents;
        private System.Windows.Forms.ListBox lbAgents;
        private System.Windows.Forms.Label lblCollectorName;
        private System.Windows.Forms.TextBox txtAgentName;
        private System.Windows.Forms.Label lblNumRecords;
        private System.Windows.Forms.TextBox txtNumRecords;
        private System.Windows.Forms.Label lblCorrectedName;
        private System.Windows.Forms.Button btnSaveChanges;
        private System.Windows.Forms.Timer tmrUpdateNamesList;
        private System.Windows.Forms.ListBox lstCorrectedNameOptions;
        private System.Windows.Forms.TextBox txtCorrectedName;
    }
}