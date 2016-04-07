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

        private void PlotCV(Cycle cv, jwGraph.jwGraph.jwGraph graph)
        {
            graph.Series.Clear();
                var ser = graph.Series.AddSeries(jwGraph.jwGraph.Series.enumSeriesType.Line, jwGraph.jwGraph.Axis.enumAxisLocation.Primary);
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
            cVPeakBindingSource.DataSource = ((Cycle)cycleBindingSource.Current).Peaks;
            PlotCV((Cycle)cycleBindingSource.Current, jwGraph1);
        }

        private void PaintPeaks(Cycle cv, Graphics g, jwGraph.jwGraph.jwGraph graph)
        {
            var rect = graph.InnerChartArea;
            rect.Inflate(new Size(4, 4));
            g.SetClip(rect);
            foreach (var peak in cv.Peaks)
            {
                var peakpos = peak.GetCenterPos;
                var peakp = graph.ValuesToPixelposition(peakpos, jwGraph.jwGraph.Axis.enumAxisLocation.Primary);
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
                    g.DrawLine(Pens.Brown, graph.ValuesToPixelposition(b1, jwGraph.jwGraph.Axis.enumAxisLocation.Primary), p1);
                    g.DrawLine(Pens.Brown, graph.ValuesToPixelposition(b2, jwGraph.jwGraph.Axis.enumAxisLocation.Primary), p1);
                }
            }
            g.ResetClip();
        }
        private void jwGraph1_Paint(object sender, PaintEventArgs e)
        {
            var graph = (jwGraph.jwGraph.jwGraph)sender;
            if (graph.Tag == null) return;
            Cycle cv = (Cycle)graph.Tag;
            PaintPeaks(cv, e.Graphics, graph);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            CVs.Add(CV.FromText(CV_Evaluator.Properties.Resources.CV, "\t"));
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
           var cyc = (Cycle)cycleBindingSource.Current;
           cyc.PickPeaks(10,cyc.Datapoints.Select((d) => d.Current).Max() * 0.25, cyc.Datapoints.Select((d) => d.Current).Min() * 0.25);
            cVPeakBindingSource.ResetBindings(true);
            jwGraph1.Invalidate();
        }

        private int pointselect = -1;
        private CVPeak workpeak;

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (cVPeakBindingSource.Current == null) return;
            CVPeak peak = (CVPeak)cVPeakBindingSource.Current;
            pointselect = 1;
            workpeak = peak;
            toolStripStatusLabel1.Text = "Select 1st point.";
        }

        private void jwGraph1_MouseDown(object sender, MouseEventArgs e)
        {
            if (pointselect == -1) return;
            jwGraph.jwGraph.Series ser=null;
            jwGraph.jwGraph.Datapoint point = null;
            int index=-1;
            if (jwGraph1.PointHitTest(e.Location, ref ser, ref index, ref point))
            {
                if(pointselect==1)
                {
                    workpeak.BaselineP1 = index;
                    pointselect = 2;
                    toolStripStatusLabel1.Text = "Select 2nd point.";
                } else
                {
                    workpeak.BaselineP2 = index;
                    pointselect = -1;
                    toolStripStatusLabel1.Text = "Done.";
                    jwGraph1.Invalidate();
                }
                

            }
        }
    }
}
