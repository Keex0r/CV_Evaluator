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
        public void SetSetting(ImportSettings settings)
        {
            this.Settings = settings;
            pgSettings.SelectedObject = settings;
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
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnDefault_Click(object sender, EventArgs e)
        {
            Settings.SetFrom(new ImportSettings());
            pgSettings.Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Settings.SetFrom(new ImportSettings());
            Settings.ColumnsPerCV = 6;
            Settings.CurrentColumn = 6;
            Settings.Delimiter = ImportSettings.enDelimiter.Tab;
            Settings.DontSplit = false;
            Settings.IgnorePoints = 0;
            Settings.SplitByColumns = "1";
            Settings.TimeColumn = 2;
            Settings.VoltColumn = 3;
            pgSettings.Refresh();
        }

        private void btnBiologicSingle_Click(object sender, EventArgs e)
        {
            Settings.SetFrom(new ImportSettings());
            Settings.ColumnsPerCV = 13;
            Settings.CurrentColumn = 10;
            Settings.CurrentMulti = 1e-3;
            Settings.Delimiter = ImportSettings.enDelimiter.Tab;
            Settings.DontSplit = true;
            Settings.IgnorePoints = 0;
            Settings.SplitByColumns = "";
            Settings.TimeColumn = 7;
            Settings.VoltColumn = 9;
            pgSettings.Refresh();
        }

        private void btnBiologicMulti_Click(object sender, EventArgs e)
        {
            Settings.SetFrom(new ImportSettings());
            Settings.ColumnsPerCV = 15;
            Settings.CurrentColumn = 10;
            Settings.CurrentMulti = 1e-3;
            Settings.Delimiter = ImportSettings.enDelimiter.Tab;
            Settings.DontSplit = false;
            Settings.IgnorePoints = 0;
            Settings.SplitByColumns = "11";
            Settings.TimeColumn = 13;
            Settings.VoltColumn = 9;
            pgSettings.Refresh();
        }

        private void btnZahner_Click(object sender, EventArgs e)
        {
            Settings.SetFrom(new ImportSettings());
            Settings.ColumnsPerCV = 4;
            Settings.CurrentColumn = 4;
            Settings.Delimiter = ImportSettings.enDelimiter.Whitespace;
            Settings.DontSplit = false;
            Settings.TimeColumn = 2;
            Settings.VoltColumn = 3;
            pgSettings.Refresh();
        }
    }
}
