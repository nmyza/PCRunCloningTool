using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PCRunCloningTool
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();

            FillFields();
        }

        private void FillFields()
        {
            this.pcDbHostNameTxtBox.Text = GlobalSettings.msSqlServerNamePC;
            this.pcDbUserNameTxtBox.Text = GlobalSettings.msSqlUserNamePC;
            this.pcDbPasswordTxtBox.Text = GlobalSettings.msSqlPasswordPC;

            this.customDbHostNameTxtBox.Text = GlobalSettings.msSqlServerNameCustom;
            this.customDbUserNameTxtBox.Text = GlobalSettings.msSqlUserNameCustom;
            this.customDbPasswordTxtBox.Text = GlobalSettings.msSqlPasswordCustom;
            this.customDbDatabaseNameTxtBox.Text = GlobalSettings.msSqlDatabaseNameCustom;
        }

        private void SaveSettingsBtn_Click(object sender, EventArgs e)
        {
            SaveSettings();
            this.Close();
        }

        private void CancelSettingsBtn_Click(object sender, EventArgs e)
        {
            FillFields();
            this.Close();
        }

        private void createTargetDbBtn_Click(object sender, EventArgs e)
        {
            SaveSettings();
            DbManager.PrepareDb();
            MessageBox.Show("DataBase is created");
        }

        private void SaveSettings()
        {
            GlobalSettings.msSqlServerNamePC = this.pcDbHostNameTxtBox.Text;
            GlobalSettings.msSqlUserNamePC = this.pcDbUserNameTxtBox.Text;
            GlobalSettings.msSqlPasswordPC = this.pcDbPasswordTxtBox.Text;

            GlobalSettings.msSqlServerNameCustom = this.customDbHostNameTxtBox.Text;
            GlobalSettings.msSqlUserNameCustom = this.customDbUserNameTxtBox.Text;
            GlobalSettings.msSqlPasswordCustom = this.customDbPasswordTxtBox.Text;
            GlobalSettings.msSqlDatabaseNameCustom = this.customDbDatabaseNameTxtBox.Text;
        }
    }
}
