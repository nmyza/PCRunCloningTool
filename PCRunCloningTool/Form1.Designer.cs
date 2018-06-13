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
            this.testRunsGridView = new System.Windows.Forms.DataGridView();
            this.downloadAllBtn = new System.Windows.Forms.Button();
            this.consoleBox = new System.Windows.Forms.RichTextBox();
            this.firstLevelFolderComBox = new System.Windows.Forms.ComboBox();
            this.secondLevelFolderComBox = new System.Windows.Forms.ComboBox();
            this.threeLevelFolderComBox = new System.Windows.Forms.ComboBox();
            this.fourLevelFolderComBox = new System.Windows.Forms.ComboBox();
            this.labelDomain = new System.Windows.Forms.Label();
            this.labelProject = new System.Windows.Forms.Label();
            this.labelProduct = new System.Windows.Forms.Label();
            this.labelRelease = new System.Windows.Forms.Label();
            this.labelVersion = new System.Windows.Forms.Label();
            this.labelSubVersion = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.testRunsGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // loadRunsBtn
            // 
            this.loadRunsBtn.Location = new System.Drawing.Point(521, 223);
            this.loadRunsBtn.Margin = new System.Windows.Forms.Padding(2);
            this.loadRunsBtn.Name = "loadRunsBtn";
            this.loadRunsBtn.Size = new System.Drawing.Size(121, 28);
            this.loadRunsBtn.TabIndex = 0;
            this.loadRunsBtn.Text = "Load Runs";
            this.loadRunsBtn.UseVisualStyleBackColor = true;
            this.loadRunsBtn.Click += new System.EventHandler(this.loadRunsBtn_Click);
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
            this.downloadBtn.Location = new System.Drawing.Point(521, 256);
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
            this.domainComBox.Location = new System.Drawing.Point(521, 12);
            this.domainComBox.Name = "domainComBox";
            this.domainComBox.Size = new System.Drawing.Size(121, 21);
            this.domainComBox.TabIndex = 4;
            this.domainComBox.SelectedIndexChanged += new System.EventHandler(this.domainComBox_SelectedIndexChanged);
            // 
            // projectComBox
            // 
            this.projectComBox.Enabled = false;
            this.projectComBox.FormattingEnabled = true;
            this.projectComBox.Location = new System.Drawing.Point(521, 39);
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
            this.folderDialogBtn.Location = new System.Drawing.Point(453, 580);
            this.folderDialogBtn.Name = "folderDialogBtn";
            this.folderDialogBtn.Size = new System.Drawing.Size(75, 23);
            this.folderDialogBtn.TabIndex = 6;
            this.folderDialogBtn.Text = "Select";
            this.folderDialogBtn.UseVisualStyleBackColor = true;
            this.folderDialogBtn.Click += new System.EventHandler(this.folderDialogBtn_Click);
            // 
            // folderPathBox
            // 
            this.folderPathBox.Location = new System.Drawing.Point(12, 583);
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
            this.unzipCheckBox.Location = new System.Drawing.Point(521, 290);
            this.unzipCheckBox.Name = "unzipCheckBox";
            this.unzipCheckBox.Size = new System.Drawing.Size(111, 17);
            this.unzipCheckBox.TabIndex = 8;
            this.unzipCheckBox.Text = "Unzip and Upload";
            this.unzipCheckBox.UseVisualStyleBackColor = true;
            // 
            // settingsBtn
            // 
            this.settingsBtn.Location = new System.Drawing.Point(453, 551);
            this.settingsBtn.Name = "settingsBtn";
            this.settingsBtn.Size = new System.Drawing.Size(75, 23);
            this.settingsBtn.TabIndex = 10;
            this.settingsBtn.Text = "Settings";
            this.settingsBtn.UseVisualStyleBackColor = true;
            this.settingsBtn.Click += new System.EventHandler(this.settingsBtn_Click);
            // 
            // testRunsGridView
            // 
            this.testRunsGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.testRunsGridView.Location = new System.Drawing.Point(12, 496);
            this.testRunsGridView.Name = "testRunsGridView";
            this.testRunsGridView.Size = new System.Drawing.Size(129, 107);
            this.testRunsGridView.TabIndex = 11;
            this.testRunsGridView.Visible = false;
            // 
            // downloadAllBtn
            // 
            this.downloadAllBtn.Location = new System.Drawing.Point(521, 313);
            this.downloadAllBtn.Name = "downloadAllBtn";
            this.downloadAllBtn.Size = new System.Drawing.Size(121, 28);
            this.downloadAllBtn.TabIndex = 12;
            this.downloadAllBtn.Text = "Download All";
            this.downloadAllBtn.UseVisualStyleBackColor = true;
            this.downloadAllBtn.Click += new System.EventHandler(this.downloadAllBtn_Click);
            // 
            // consoleBox
            // 
            this.consoleBox.Location = new System.Drawing.Point(12, 366);
            this.consoleBox.Name = "consoleBox";
            this.consoleBox.Size = new System.Drawing.Size(435, 211);
            this.consoleBox.TabIndex = 13;
            this.consoleBox.Text = "";
            // 
            // firstLevelFolderComBox
            // 
            this.firstLevelFolderComBox.Enabled = false;
            this.firstLevelFolderComBox.FormattingEnabled = true;
            this.firstLevelFolderComBox.Location = new System.Drawing.Point(521, 66);
            this.firstLevelFolderComBox.Name = "firstLevelFolderComBox";
            this.firstLevelFolderComBox.Size = new System.Drawing.Size(121, 21);
            this.firstLevelFolderComBox.TabIndex = 14;
            this.firstLevelFolderComBox.Visible = false;
            this.firstLevelFolderComBox.SelectedIndexChanged += new System.EventHandler(this.firstLevelFolderComBox_SelectedIndexChanged);
            // 
            // secondLevelFolderComBox
            // 
            this.secondLevelFolderComBox.Enabled = false;
            this.secondLevelFolderComBox.FormattingEnabled = true;
            this.secondLevelFolderComBox.Location = new System.Drawing.Point(521, 93);
            this.secondLevelFolderComBox.Name = "secondLevelFolderComBox";
            this.secondLevelFolderComBox.Size = new System.Drawing.Size(121, 21);
            this.secondLevelFolderComBox.TabIndex = 15;
            this.secondLevelFolderComBox.Visible = false;
            this.secondLevelFolderComBox.SelectedIndexChanged += new System.EventHandler(this.secondLevelFolderComBox_SelectedIndexChanged);
            // 
            // threeLevelFolderComBox
            // 
            this.threeLevelFolderComBox.Enabled = false;
            this.threeLevelFolderComBox.FormattingEnabled = true;
            this.threeLevelFolderComBox.Location = new System.Drawing.Point(521, 120);
            this.threeLevelFolderComBox.Name = "threeLevelFolderComBox";
            this.threeLevelFolderComBox.Size = new System.Drawing.Size(121, 21);
            this.threeLevelFolderComBox.TabIndex = 16;
            this.threeLevelFolderComBox.Visible = false;
            // 
            // fourLevelFolderComBox
            // 
            this.fourLevelFolderComBox.Enabled = false;
            this.fourLevelFolderComBox.FormattingEnabled = true;
            this.fourLevelFolderComBox.Location = new System.Drawing.Point(521, 147);
            this.fourLevelFolderComBox.Name = "fourLevelFolderComBox";
            this.fourLevelFolderComBox.Size = new System.Drawing.Size(121, 21);
            this.fourLevelFolderComBox.TabIndex = 17;
            this.fourLevelFolderComBox.Visible = false;
            // 
            // labelDomain
            // 
            this.labelDomain.AutoSize = true;
            this.labelDomain.Location = new System.Drawing.Point(473, 15);
            this.labelDomain.Name = "labelDomain";
            this.labelDomain.Size = new System.Drawing.Size(43, 13);
            this.labelDomain.TabIndex = 18;
            this.labelDomain.Text = "Domain";
            // 
            // labelProject
            // 
            this.labelProject.AutoSize = true;
            this.labelProject.Location = new System.Drawing.Point(473, 42);
            this.labelProject.Name = "labelProject";
            this.labelProject.Size = new System.Drawing.Size(40, 13);
            this.labelProject.TabIndex = 19;
            this.labelProject.Text = "Project";
            // 
            // labelProduct
            // 
            this.labelProduct.AutoSize = true;
            this.labelProduct.Location = new System.Drawing.Point(469, 69);
            this.labelProduct.Name = "labelProduct";
            this.labelProduct.Size = new System.Drawing.Size(44, 13);
            this.labelProduct.TabIndex = 20;
            this.labelProduct.Text = "Product";
            this.labelProduct.Visible = false;
            // 
            // labelRelease
            // 
            this.labelRelease.AutoSize = true;
            this.labelRelease.Location = new System.Drawing.Point(469, 96);
            this.labelRelease.Name = "labelRelease";
            this.labelRelease.Size = new System.Drawing.Size(46, 13);
            this.labelRelease.TabIndex = 21;
            this.labelRelease.Text = "Release";
            this.labelRelease.Visible = false;
            // 
            // labelVersion
            // 
            this.labelVersion.AutoSize = true;
            this.labelVersion.Location = new System.Drawing.Point(473, 123);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(42, 13);
            this.labelVersion.TabIndex = 22;
            this.labelVersion.Text = "Version";
            this.labelVersion.Visible = false;
            // 
            // labelSubVersion
            // 
            this.labelSubVersion.AutoSize = true;
            this.labelSubVersion.Location = new System.Drawing.Point(453, 150);
            this.labelSubVersion.Name = "labelSubVersion";
            this.labelSubVersion.Size = new System.Drawing.Size(64, 13);
            this.labelSubVersion.TabIndex = 23;
            this.labelSubVersion.Text = "Sub Version";
            this.labelSubVersion.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(661, 614);
            this.Controls.Add(this.labelSubVersion);
            this.Controls.Add(this.labelVersion);
            this.Controls.Add(this.labelRelease);
            this.Controls.Add(this.labelProduct);
            this.Controls.Add(this.labelProject);
            this.Controls.Add(this.labelDomain);
            this.Controls.Add(this.fourLevelFolderComBox);
            this.Controls.Add(this.threeLevelFolderComBox);
            this.Controls.Add(this.secondLevelFolderComBox);
            this.Controls.Add(this.firstLevelFolderComBox);
            this.Controls.Add(this.consoleBox);
            this.Controls.Add(this.downloadAllBtn);
            this.Controls.Add(this.testRunsGridView);
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
            ((System.ComponentModel.ISupportInitialize)(this.testRunsGridView)).EndInit();
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
        private System.Windows.Forms.DataGridView testRunsGridView;
        private System.Windows.Forms.Button downloadAllBtn;
        private System.Windows.Forms.RichTextBox consoleBox;
        private System.Windows.Forms.ComboBox firstLevelFolderComBox;
        private System.Windows.Forms.ComboBox secondLevelFolderComBox;
        private System.Windows.Forms.ComboBox threeLevelFolderComBox;
        private System.Windows.Forms.ComboBox fourLevelFolderComBox;
        private System.Windows.Forms.Label labelDomain;
        private System.Windows.Forms.Label labelProject;
        private System.Windows.Forms.Label labelProduct;
        private System.Windows.Forms.Label labelRelease;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.Label labelSubVersion;
    }
}

