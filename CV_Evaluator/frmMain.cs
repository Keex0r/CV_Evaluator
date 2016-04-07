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
    public partial class frmMain : Form
    {
        private BindingList<CV> CVs;
        public frmMain()
        {
            InitializeComponent();
            CVs = new BindingList<CV>();
            cVBindingSource.DataSource = CVs;
         
        }

        private void PlotCV(Cycle cv, jwToolLib.jwGraph.jwGraph graph)
        {
            graph.Series.Clear();
                var ser = graph.Series.AddSeries(jwToolLib.jwGraph.Series.enumSeriesType.Line,jwToolLib.jwGraph.Axis.enumAxisLocation.Primary);
                foreach (var d in cv.Datapoints)
                {
                    ser.AddXY(d.Volt, d.Current);
                }
            graph.Tag = cv;

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (!Clipboard.ContainsText()) return;
            var newcv = CV.FromText(Clipboard.GetText(), "\t");
            if(newcv== null)
            {
                MessageBox.Show("Invalid/Incomplete Data!");
                return;
            }
            CVs.Add(newcv);
        }

        private void cVBindingSource_CurrentChanged(object sender, EventArgs e)
        {
            cycleBindingSource.DataSource = ((CV)cVBindingSource.Current).Cycles;
        }

        private void cycleBindingSource_CurrentChanged(object sender, EventArgs e)
        {
            PlotCV((Cycle)cycleBindingSource.Current, jwGraph1);
        }

        private void PaintPeaks(Cycle cv, Graphics g, jwToolLib.jwGraph.jwGraph graph)
        {
            foreach (var peak in cv.Peaks)
            {
                var peakpos = peak.GetCenterPos;
                var peakp = graph.ValuesToPixelposition(peakpos, jwToolLib.jwGraph.Axis.enumAxisLocation.Primary);
                var hasBaseline = peak.BaselineP1 != -1 && peak.BaselineP2 != -1;
                if (!hasBaseline)
                {
                    var zeroy = graph.Y1Axis.ValueToPixelPosition(0) + graph.GraphBorder.Top;
                    var p1 = new PointF(peakp.X, zeroy);
                    g.DrawLine(Pens.Brown, peakp, p1);
                } else
                {
                    var b1 = peak.BaselineValues1;
                    var b2 = peak.BaselineValues2;
                    var bypeak = peak.BaseLineCurrentAtPeak;
                    var ypos = graph.Y1Axis.ValueToPixelPosition(bypeak)+graph.GraphBorder.Top;
                    var p1 = new PointF(peakp.X, ypos);
                    g.DrawLine(Pens.Brown, peakp, p1);
                    g.DrawLine(Pens.Brown, graph.ValuesToPixelposition(b1, jwToolLib.jwGraph.Axis.enumAxisLocation.Primary), p1);
                    g.DrawLine(Pens.Brown, graph.ValuesToPixelposition(b2, jwToolLib.jwGraph.Axis.enumAxisLocation.Primary), p1);
                }
            }
        }
        private void jwGraph1_Paint(object sender, PaintEventArgs e)
        {
            var graph = (jwToolLib.jwGraph.jwGraph)sender;
            if (graph.Tag == null) return;
            Cycle cv = (Cycle)graph.Tag;
            PaintPeaks(cv, e.Graphics, graph);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            CVs.Add(CV.FromText(CV_Evaluator.Properties.Resources.CV, "\t"));
        }
    }
}
