namespace PCRunCloningTool
{
    partial class SettingsForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.pcDbPasswordTxtBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.pcDbUserNameTxtBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pcDbHostNameTxtBox = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.createTargetDbBtn = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.customDbDatabaseNameTxtBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.customDbPasswordTxtBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.customDbUserNameTxtBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.customDbHostNameTxtBox = new System.Windows.Forms.TextBox();
            this.SaveSettingsBtn = new System.Windows.Forms.Button();
            this.CancelSettingsBtn = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.pcDbPasswordTxtBox);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.pcDbUserNameTxtBox);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.pcDbHostNameTxtBox);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(254, 108);
            this.panel1.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 82);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Password";
            // 
            // pcDbPasswordTxtBox
            // 
            this.pcDbPasswordTxtBox.Location = new System.Drawing.Point(67, 76);
            this.pcDbPasswordTxtBox.Name = "pcDbPasswordTxtBox";
            this.pcDbPasswordTxtBox.Size = new System.Drawing.Size(170, 20);
            this.pcDbPasswordTxtBox.TabIndex = 5;
            this.pcDbPasswordTxtBox.UseSystemPasswordChar = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "User name";
            // 
            // pcDbUserNameTxtBox
            // 
            this.pcDbUserNameTxtBox.Location = new System.Drawing.Point(67, 50);
            this.pcDbUserNameTxtBox.Name = "pcDbUserNameTxtBox";
            this.pcDbUserNameTxtBox.Size = new System.Drawing.Size(170, 20);
            this.pcDbUserNameTxtBox.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(36, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(180, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "PC MS SQL DB Connection Settings";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Host Name";
            // 
            // pcDbHostNameTxtBox
            // 
            this.pcDbHostNameTxtBox.Location = new System.Drawing.Point(67, 24);
            this.pcDbHostNameTxtBox.Name = "pcDbHostNameTxtBox";
            this.pcDbHostNameTxtBox.Size = new System.Drawing.Size(170, 20);
            this.pcDbHostNameTxtBox.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.createTargetDbBtn);
            this.panel2.Controls.Add(this.label9);
            this.panel2.Controls.Add(this.customDbDatabaseNameTxtBox);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.customDbPasswordTxtBox);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.customDbUserNameTxtBox);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.label8);
            this.panel2.Controls.Add(this.customDbHostNameTxtBox);
            this.panel2.Location = new System.Drawing.Point(272, 12);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(242, 156);
            this.panel2.TabIndex = 0;
            // 
            // createTargetDbBtn
            // 
            this.createTargetDbBtn.Location = new System.Drawing.Point(6, 128);
            this.createTargetDbBtn.Name = "createTargetDbBtn";
            this.createTargetDbBtn.Size = new System.Drawing.Size(121, 23);
            this.createTargetDbBtn.TabIndex = 10;
            this.createTargetDbBtn.Text = "Create Target DB";
            this.createTargetDbBtn.UseVisualStyleBackColor = true;
            this.createTargetDbBtn.Click += new System.EventHandler(this.createTargetDbBtn_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 108);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 13);
            this.label9.TabIndex = 8;
            this.label9.Text = "DB Name";
            // 
            // customDbDatabaseNameTxtBox
            // 
            this.customDbDatabaseNameTxtBox.Location = new System.Drawing.Point(71, 102);
            this.customDbDatabaseNameTxtBox.Name = "customDbDatabaseNameTxtBox";
            this.customDbDatabaseNameTxtBox.Size = new System.Drawing.Size(154, 20);
            this.customDbDatabaseNameTxtBox.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 82);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Password";
            // 
            // customDbPasswordTxtBox
            // 
            this.customDbPasswordTxtBox.Location = new System.Drawing.Point(71, 76);
            this.customDbPasswordTxtBox.Name = "customDbPasswordTxtBox";
            this.customDbPasswordTxtBox.Size = new System.Drawing.Size(154, 20);
            this.customDbPasswordTxtBox.TabIndex = 5;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 56);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "User name";
            // 
            // customDbUserNameTxtBox
            // 
            this.customDbUserNameTxtBox.Location = new System.Drawing.Point(71, 50);
            this.customDbUserNameTxtBox.Name = "customDbUserNameTxtBox";
            this.customDbUserNameTxtBox.Size = new System.Drawing.Size(154, 20);
            this.customDbUserNameTxtBox.TabIndex = 3;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(24, 4);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(201, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "Custom MS SQL DB Connection Settings";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 30);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(60, 13);
            this.label8.TabIndex = 1;
            this.label8.Text = "Host Name";
            // 
            // customDbHostNameTxtBox
            // 
            this.customDbHostNameTxtBox.Location = new System.Drawing.Point(71, 24);
            this.customDbHostNameTxtBox.Name = "customDbHostNameTxtBox";
            this.customDbHostNameTxtBox.Size = new System.Drawing.Size(154, 20);
            this.customDbHostNameTxtBox.TabIndex = 0;
            // 
            // SaveSettingsBtn
            // 
            this.SaveSettingsBtn.Location = new System.Drawing.Point(18, 140);
            this.SaveSettingsBtn.Name = "SaveSettingsBtn";
            this.SaveSettingsBtn.Size = new System.Drawing.Size(75, 23);
            this.SaveSettingsBtn.TabIndex = 1;
            this.SaveSettingsBtn.Text = "Save";
            this.SaveSettingsBtn.UseVisualStyleBackColor = true;
            this.SaveSettingsBtn.Click += new System.EventHandler(this.SaveSettingsBtn_Click);
            // 
            // CancelSettingsBtn
            // 
            this.CancelSettingsBtn.Location = new System.Drawing.Point(108, 140);
            this.CancelSettingsBtn.Name = "CancelSettingsBtn";
            this.CancelSettingsBtn.Size = new System.Drawing.Size(75, 23);
            this.CancelSettingsBtn.TabIndex = 2;
            this.CancelSettingsBtn.Text = "Cancel";
            this.CancelSettingsBtn.UseVisualStyleBackColor = true;
            this.CancelSettingsBtn.Click += new System.EventHandler(this.CancelSettingsBtn_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(532, 182);
            this.Controls.Add(this.CancelSettingsBtn);
            this.Controls.Add(this.SaveSettingsBtn);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "SettingsForm";
            this.Text = "Settings";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox pcDbPasswordTxtBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox pcDbUserNameTxtBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox pcDbHostNameTxtBox;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox customDbPasswordTxtBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox customDbUserNameTxtBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox customDbHostNameTxtBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox customDbDatabaseNameTxtBox;
        private System.Windows.Forms.Button SaveSettingsBtn;
        private System.Windows.Forms.Button CancelSettingsBtn;
        private System.Windows.Forms.Button createTargetDbBtn;
    }
}