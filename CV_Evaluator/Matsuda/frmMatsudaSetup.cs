using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CV_Evaluator.Matsuda
{
    public partial class frmMatsudaSetup : Form
    {

        public MatsudaSettings Settings;

        public frmMatsudaSetup()
        {
            InitializeComponent();
            if(string.IsNullOrEmpty(Properties.Settings.Default.RandlesSevSettings))
            {
                Settings = new MatsudaSettings();
            } else
            {
                Settings = MatsudaSettings.FromXML(Properties.Settings.Default.RandlesSevSettings);
                if (Settings == null) Settings = new MatsudaSettings();
            }
            pgSettings.SelectedObject = Settings;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.RandlesSevSettings = Settings.GetXML();
            Properties.Settings.Default.Save();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
