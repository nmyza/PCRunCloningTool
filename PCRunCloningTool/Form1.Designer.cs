namespace PCRunCloningTool
{
    partial class Form1
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
            this.loadRunsBtn = new System.Windows.Forms.Button();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.downloadBtn = new System.Windows.Forms.Button();
            this.domainComBox = new System.Windows.Forms.ComboBox();
            this.projectComBox = new System.Windows.Forms.ComboBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.folderDialogBtn = new System.Windows.Forms.Button();
            this.folderPathBox = new System.Windows.Forms.TextBox();
            this.unzipCheckBox = new System.Windows.Forms.CheckBox();
            this.settingsBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // loadRunsBtn
            // 
            this.loadRunsBtn.Location = new System.Drawing.Point(468, 66);
            this.loadRunsBtn.Margin = new System.Windows.Forms.Padding(2);
            this.loadRunsBtn.Name = "loadRunsBtn";
            this.loadRunsBtn.Size = new System.Drawing.Size(121, 28);
            this.loadRunsBtn.TabIndex = 0;
            this.loadRunsBtn.Text = "Load Runs";
            this.loadRunsBtn.UseVisualStyleBackColor = true;
            this.loadRunsBtn.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.CheckOnClick = true;
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(12, 12);
            this.checkedListBox1.MultiColumn = true;
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(435, 334);
            this.checkedListBox1.TabIndex = 1;
            // 
            // downloadBtn
            // 
            this.downloadBtn.Location = new System.Drawing.Point(468, 99);
            this.downloadBtn.Name = "downloadBtn";
            this.downloadBtn.Size = new System.Drawing.Size(121, 28);
            this.downloadBtn.TabIndex = 2;
            this.downloadBtn.Text = "Download";
            this.downloadBtn.UseVisualStyleBackColor = true;
            this.downloadBtn.Click += new System.EventHandler(this.DownloadBtn_Click);
            // 
            // domainComBox
            // 
            this.domainComBox.FormattingEnabled = true;
            this.domainComBox.Location = new System.Drawing.Point(468, 12);
            this.domainComBox.Name = "domainComBox";
            this.domainComBox.Size = new System.Drawing.Size(121, 21);
            this.domainComBox.TabIndex = 4;
            this.domainComBox.SelectedIndexChanged += new System.EventHandler(this.domainComBox_SelectedIndexChanged);
            // 
            // projectComBox
            // 
            this.projectComBox.Enabled = false;
            this.projectComBox.FormattingEnabled = true;
            this.projectComBox.Location = new System.Drawing.Point(468, 40);
            this.projectComBox.Name = "projectComBox";
            this.projectComBox.Size = new System.Drawing.Size(121, 21);
            this.projectComBox.TabIndex = 5;
            this.projectComBox.SelectedIndexChanged += new System.EventHandler(this.projectComBox_SelectedIndexChanged);
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.SelectedPath = "\\\\mir\\qa\\temp\\nazamyz";
            // 
            // folderDialogBtn
            // 
            this.folderDialogBtn.Location = new System.Drawing.Point(453, 349);
            this.folderDialogBtn.Name = "folderDialogBtn";
            this.folderDialogBtn.Size = new System.Drawing.Size(75, 23);
            this.folderDialogBtn.TabIndex = 6;
            this.folderDialogBtn.Text = "Select";
            this.folderDialogBtn.UseVisualStyleBackColor = true;
            this.folderDialogBtn.Click += new System.EventHandler(this.folderDialogBtn_Click);
            // 
            // folderPathBox
            // 
            this.folderPathBox.Location = new System.Drawing.Point(12, 352);
            this.folderPathBox.Name = "folderPathBox";
            this.folderPathBox.Size = new System.Drawing.Size(435, 20);
            this.folderPathBox.TabIndex = 7;
            this.folderPathBox.Text = "\\\\mir\\qa\\temp\\nazamyz";
            // 
            // unzipCheckBox
            // 
            this.unzipCheckBox.AutoSize = true;
            this.unzipCheckBox.Checked = true;
            this.unzipCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.unzipCheckBox.Location = new System.Drawing.Point(468, 133);
            this.unzipCheckBox.Name = "unzipCheckBox";
            this.unzipCheckBox.Size = new System.Drawing.Size(111, 17);
            this.unzipCheckBox.TabIndex = 8;
            this.unzipCheckBox.Text = "Unzip and Upload";
            this.unzipCheckBox.UseVisualStyleBackColor = true;
            // 
            // settingsBtn
            // 
            this.settingsBtn.Location = new System.Drawing.Point(453, 320);
            this.settingsBtn.Name = "settingsBtn";
            this.settingsBtn.Size = new System.Drawing.Size(75, 23);
            this.settingsBtn.TabIndex = 10;
            this.settingsBtn.Text = "Settings";
            this.settingsBtn.UseVisualStyleBackColor = true;
            this.settingsBtn.Click += new System.EventHandler(this.settingsBtn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 388);
            this.Controls.Add(this.settingsBtn);
            this.Controls.Add(this.unzipCheckBox);
            this.Controls.Add(this.folderPathBox);
            this.Controls.Add(this.folderDialogBtn);
            this.Controls.Add(this.projectComBox);
            this.Controls.Add(this.domainComBox);
            this.Controls.Add(this.downloadBtn);
            this.Controls.Add(this.checkedListBox1);
            this.Controls.Add(this.loadRunsBtn);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "Cloning Tool";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button loadRunsBtn;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.Button downloadBtn;
        private System.Windows.Forms.ComboBox domainComBox;
        private System.Windows.Forms.ComboBox projectComBox;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.TextBox folderPathBox;
        private System.Windows.Forms.Button folderDialogBtn;
        private System.Windows.Forms.CheckBox unzipCheckBox;
        private System.Windows.Forms.Button settingsBtn;
    }
}

