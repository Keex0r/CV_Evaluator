using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CV_Evaluator.PeakPicking
{
    public partial class frmPeakPickingSetup : Form
    {
        public frmPeakPickingSetup()
        {
            InitializeComponent();
        }
        
        public PeakPickSettings GetSettings()
        {
            var settings = new PeakPickSettings();
            settings.Window = (int)numWindow.Value;
            settings.SteepnessLimit = numSteepness.Value;
            settings.MinHeight = numMinHeight.Value;
            settings.BaselineStdDevLimit = numStdLimit.Value;
            return settings;
        }
        public void SetSettings(PeakPickSettings settings)
        {
            numWindow.Value= settings.Window;
            numSteepness.Value= settings.SteepnessLimit;
            numMinHeight.Value= settings.MinHeight;
            numStdLimit.Value=settings.BaselineStdDevLimit;
        }

        private void frmPeakPickingSetup_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }
    }
}
