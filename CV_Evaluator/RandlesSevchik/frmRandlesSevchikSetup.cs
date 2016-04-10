using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CV_Evaluator.RandlesSevchik
{
    public partial class frmRandlesSevchikSetup : Form
    {

        public RandlesSevchikSettings Settings;

        public frmRandlesSevchikSetup()
        {
            InitializeComponent();
            Settings = new RandlesSevchikSettings();
            pgSettings.SelectedObject = Settings;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
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
