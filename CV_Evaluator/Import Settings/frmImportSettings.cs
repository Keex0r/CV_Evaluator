using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CV_Evaluator.Import_Settings
{
    public partial class frmImportSettings : Form
    {

        public ImportSettings Settings;

        public frmImportSettings()
        {
            InitializeComponent();
            if(string.IsNullOrEmpty(Properties.Settings.Default.ImportSettings))
            {
                Settings = new ImportSettings();
            } else
            {
                Settings = ImportSettings.FromXML(Properties.Settings.Default.ImportSettings);
                if (Settings == null) Settings = new ImportSettings();
            }
            pgSettings.SelectedObject = Settings;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.ImportSettings = Settings.GetXML();
            Properties.Settings.Default.Save();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Settings.GetXML());
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
