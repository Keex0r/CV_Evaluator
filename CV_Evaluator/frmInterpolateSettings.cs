using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CV_Evaluator
{
    public partial class frmInterpolateSettings : Form
    {

        public double dt
        {
            get
            {
                return (double)numdt.Value;
            }
        }
        public int Skips
        {
            get
            {
                return (int)numSkips.Value;
            }
        }
        public bool SettZero
        {
            get
            {
                return cbSettZero.Checked;
            }
        }
        public bool FromScanrate
        {
            get
            {
                return cbFromScanrate.Checked;
            }
        }
        public bool InPlace
        {
            get
            {
                return cbInPlace.Checked;
            }
        }
        public bool AllCycles
        {
            get
            {
                return cbAllCycles.Checked;
            }
        }
        private Cycle from;
        public frmInterpolateSettings(Cycle From)
        {
            InitializeComponent();
            from = From;
            UpdateStats();
        }
        private void UpdateStats()
        {
            var tmin = from.Datapoints.Select(s => s.Time).Min();
            var tmax = from.Datapoints.Select(s => s.Time).Max();
            if(cbFromScanrate.Checked == false)
            {
                lblStats.Text = "nPoints: " + (tmax - tmin) / dt;
            } else
            {
                var tstep = dt / from.Scanrate;
                lblStats.Text = "nPoints: " + (tmax - tmin) / tstep;
            }
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            label1.Text = !cbFromScanrate.Checked ? "Delta t" : "Factor";
            UpdateStats();
        }

        private void frmInterpolateSettings_Load(object sender, EventArgs e)
        {

        }

        private void numdt_ValueChanged(object sender, EventArgs e)
        {
            UpdateStats();
        }
    }
}
