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
            numStdLimit.TextChanged += numMinHeight_TextChanged;
            numSteepness.TextChanged += numMinHeight_TextChanged;
            numWindow.TextChanged += numMinHeight_TextChanged;
            cbJustMaxMin.CheckedChanged += (s,e) => SaveSettings(); ;
        }
        
        public PeakPickSettings GetSettings()
        {
            var settings = new PeakPickSettings();
            settings.Window = numWindow.Value;
            settings.SteepnessLimit = numSteepness.Value;
            settings.MinHeight = numMinHeight.Value;
            settings.BaselineStdDevLimit = numStdLimit.Value;
            settings.JustUseMaxMin = cbJustMaxMin.Checked;
            return settings;
        }
        public void SetSettings(PeakPickSettings settings)
        {
            numWindow.Value= settings.Window;
            numSteepness.Value= settings.SteepnessLimit;
            numMinHeight.Value= settings.MinHeight;
            numStdLimit.Value=settings.BaselineStdDevLimit;
            cbJustMaxMin.Checked = settings.JustUseMaxMin;
        }

        private void frmPeakPickingSetup_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }
        public void LoadSettings()
        {
            if(!string.IsNullOrEmpty(CV_Evaluator.Properties.Settings.Default.PeakPickSettings))
            {
                var newsets = PeakPickSettings.FromXML(CV_Evaluator.Properties.Settings.Default.PeakPickSettings);
                SetSettings(newsets);
            }
        }
        public void SaveSettings()
        {
            CV_Evaluator.Properties.Settings.Default.PeakPickSettings = GetSettings().GetXML();
            CV_Evaluator.Properties.Settings.Default.Save();
        }

        private void numMinHeight_TextChanged(object sender, EventArgs e)
        {
            if(((jwGraph.GeneralTools.NumericInputbox)sender).IsValid)
            {
                SaveSettings();
            }
        }
    }
}
