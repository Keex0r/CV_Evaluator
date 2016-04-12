using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CV_Evaluator
{
    public partial class frmMain : Form
    {
        private PeakPicking.frmPeakPickingSetup frmPeakPicking;
        private BindingList<CV> CVs;
        public frmMain()
        {
            InitializeComponent();
            CVs = new BindingList<CV>();
            cVBindingSource.DataSource = CVs;
            frmPeakPicking = new PeakPicking.frmPeakPickingSetup();
            frmPeakPicking.Owner = this;
            frmPeakPicking.Show();
            jwGraph1.XAxis.Title = "E / V";
            jwGraph1.Y1Axis.Title = "I / A";
        }

        private void PlotCV(Cycle cv, jwGraph.jwGraph.jwGraph graph)
        {
            graph.Series.Clear();
            var ser = graph.Series.AddSeries(jwGraph.jwGraph.Series.enumSeriesType.Line, jwGraph.jwGraph.Axis.enumAxisLocation.Primary);
            int count = 0;
               foreach (var d in cv.Datapoints)
                {
                ser.AddXY(d.Volt, d.Current);
                count++;
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
            var newcvs = CV.FromText(Clipboard.GetText(), Program.RuntimeData.ImportSettings);
            if(newcvs== null ||newcvs.Count==0)
            {
                MessageBox.Show("Invalid/Incomplete Data!");
                return;
            }
            foreach(CV cv in newcvs)
            {
                PickPeaksCV(cv);
                CVs.Add(cv);
            }
            
        }

        private void cVBindingSource_CurrentChanged(object sender, EventArgs e)
        {
            if (cVBindingSource.Current == null)
            {
                cycleBindingSource.DataSource = null;
                return;
            }
            cycleBindingSource.DataSource = ((CV)cVBindingSource.Current).bdsCycles;
        }

        private Cycle LastCycle;
        private void cycleBindingSource_CurrentChanged(object sender, EventArgs e)
        {
            if (LastCycle != null) LastCycle.bdsPeaks.ListChanged -= RefreshCurrentCycle;
            if(cycleBindingSource.Current==null)
            {
                cVPeakBindingSource.DataSource = null;
                return;
            }
            cVPeakBindingSource.DataSource = ((Cycle)cycleBindingSource.Current).bdsPeaks;
            ((Cycle)cycleBindingSource.Current).bdsPeaks.ListChanged += RefreshCurrentCycle;
            LastCycle = ((Cycle)cycleBindingSource.Current);
            PlotCV((Cycle)cycleBindingSource.Current, jwGraph1);
        }
        private void RefreshCurrentCycle(object sender, EventArgs e)
        {
            jwGraph1.Series.Clear();
            if (cycleBindingSource.Current == null) return;
            PlotCV((Cycle)cycleBindingSource.Current, jwGraph1);
        }

        private void PaintPeaks(Cycle cv, Graphics g, jwGraph.jwGraph.jwGraph graph)
        {
            var rect = graph.InnerChartArea;
            rect.Inflate(new Size(4, 4));
            g.SetClip(rect);
            foreach (var peak in cv.Peaks)
            {
                peak.DrawYourself(g, graph);
            }
            foreach (var PeakCon in cv.PeakConnections)
            {
                PeakCon.DrawYourself(g, graph);
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
            var cvs = CV.FromText(CV_Evaluator.Properties.Resources.CV, Program.RuntimeData.ImportSettings);
            foreach(CV cv in cvs)
            {
                PickPeaksCV(cv);
                CVs.Add(cv);
            }
            
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
                    dgvPeaks.Refresh();
                    toolStripStatusLabel1.Text = "Done.";
                    jwGraph1.Invalidate();
                }
                

            }
        }


        private void ConnectSelectedPeaks()
        {
            if (cycleBindingSource.Current == null) return;
            if (dgvPeaks.SelectedRows.Count < 2) return;
            Cycle cyc = (Cycle)cycleBindingSource.Current;
            foreach (DataGridViewRow r in dgvPeaks.SelectedRows)
            {
                var thispeak = (CVPeak)r.DataBoundItem;
                foreach (DataGridViewRow r2 in dgvPeaks.SelectedRows)
                {
                    if (r2 == r) continue;
                    var otherpeak = (CVPeak)r2.DataBoundItem;
                    CVPeakConnection newc = new CVPeakConnection(cyc);
                    newc.Peak1 = thispeak;
                    newc.Peak2 = otherpeak;
                    if (!cyc.PeakConnections.Any(c => c == newc)) cyc.PeakConnections.Add(newc);
                }
            }
            jwGraph1.Invalidate();
        }
        private void ClearPeakConnections()
        {
            if (cycleBindingSource.Current == null) return;
            if (cVPeakBindingSource.Current == null) return;
            Cycle cyc = (Cycle)cycleBindingSource.Current;
            CVPeak peak = (CVPeak)cVPeakBindingSource.Current;
            cyc.PeakConnections.RemoveAll(p => p.Peak1 == peak || p.Peak2 == peak);
            jwGraph1.Invalidate();
        }

        private void connectSelectedPeaksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConnectSelectedPeaks();
        }

        private void clearConnectionsOfSelectedPeakToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearPeakConnections();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            RandlesSevchik.RandlesSevchikSettings settings;
            using (var frmSettings = new RandlesSevchik.frmRandlesSevchikSetup())
            {
                if (frmSettings.ShowDialog(this) == DialogResult.Cancel) return;
                settings = frmSettings.Settings;
            }
            using (var frmResult = new RandlesSevchik.frmRandlesSevchikResults(settings,this.CVs))
            {
                frmResult.ShowDialog(this);
            }
        }
        private string LastSaveName;
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var ser1 = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                using (var ms = new MemoryStream())
                {
                    using (var sfd = new SaveFileDialog())
                    {
                        if (!string.IsNullOrEmpty(LastSaveName)) sfd.FileName = LastSaveName;
                        sfd.Filter = "CV Evaluator File|*.cve";
                        sfd.AddExtension = true;
                        if (sfd.ShowDialog() == DialogResult.Cancel) return;
                        ser1.Serialize(ms, this.CVs.ToList());
                        File.WriteAllBytes(sfd.FileName, ms.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured while saving: " + ex.Message);
                throw ex;
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<CV> PreviousCVs = new List<CV>();
            PreviousCVs.AddRange(CVs);
            try {
                var ser1 = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                using (var ofd = new OpenFileDialog())
                {
                    ofd.Filter = "CV Evaluator File|*.cve";
                    ofd.Multiselect = false;
                    if (ofd.ShowDialog() == DialogResult.Cancel) return;
                    LastSaveName = ofd.FileName;
                    var data = File.ReadAllBytes(ofd.FileName);
                    using (var ms = new MemoryStream(data))
                    {
                        var values = (List<CV>)ser1.Deserialize(ms);
                        this.CVs.Clear();
                        foreach (var cv in values)
                        {
                            cv.Setup();
                            CVs.Add(cv);
                        }
                    }

                }
            } catch(Exception ex)
            {
                MessageBox.Show("An error occured while loading: " + ex.Message);
                //Restore previous state state
                CVs.Clear();
                foreach (var cv in PreviousCVs) CVs.Add(cv);
            }
        }

        private void SplitToNew(List<Cycle> cycles)
        {
            //Only for the same parent CV
            if (cycles.Select(x => x.Parent).Distinct().Count() > 1) return;
            var parent = cycles[0].Parent;
            var newcv = new CV();
            newcv.Datasource = "From Split";
            foreach(var cyc in cycles)
            {
                parent.Cycles.Remove(cyc);
                cyc.Parent=newcv;
                cyc.Number = newcv.Cycles.Count + 1;
                newcv.Cycles.Add(cyc);
            }
            newcv.Setup();
            CVs.Add(newcv);
            dgvCVs.ClearSelection();
            
            var row=dgvCVs.Rows.Cast<DataGridViewRow>().Where(x => x.DataBoundItem == newcv).Select(x => x).First();
            row.Selected = true;
            dgvCVs.CurrentCell = row.Cells[0];
        }
    
        private void tsbSplitCycles_Click(object sender, EventArgs e)
        {
            List<Cycle> cycles = new List<Cycle>();
            foreach (DataGridViewRow row in dgvCycles.SelectedRows)
            {
                cycles.Add((Cycle)row.DataBoundItem);
            }
            SplitToNew(cycles);
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you really want to discard all current items? All unsaved progress will be lost.", "Please confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No) return;
            CVs.Clear();
        }

        private void cVBindingSource_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (cVBindingSource.Current == null)
            {
                cycleBindingSource.DataSource = null;
                return;
            }
            cycleBindingSource.DataSource = ((CV)cVBindingSource.Current).bdsCycles;

        }

        private void peakPickingSetupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (frmPeakPicking == null || frmPeakPicking.IsDisposed) frmPeakPicking = new PeakPicking.frmPeakPickingSetup();
            frmPeakPicking.Owner = this;
            frmPeakPicking.Show();
        }
        private void RefreshAll()
        {
            cVBindingSource.ResetBindings(true);
            cycleBindingSource.ResetBindings(true);
            cVPeakBindingSource.ResetBindings(true);
            if(!(cycleBindingSource.Current==null))
            {
                PlotCV((Cycle)cycleBindingSource.Current, jwGraph1);
            }
        }
        private void thisCycleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (cycleBindingSource.Current == null) return;
            if (frmPeakPicking == null) return;
            var cyc = (Cycle)cycleBindingSource.Current;
            PickPeaksCycle(cyc);
            RefreshAll();
        }

        private void thisCVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (cVBindingSource.Current == null) return;
            if (frmPeakPicking == null) return;
            var cv = (CV)cVBindingSource.Current;
            PickPeaksCV(cv);
            RefreshAll();
        }
        private void PickPeaksCycle(Cycle cyc)
        {
            if (frmPeakPicking == null) return;
            var sets = frmPeakPicking.GetSettings();
            cyc.PickPeaks(sets.Window, sets.MinHeight, sets.SteepnessLimit,sets.BaselineStdDevLimit);

        }
        private void PickPeaksCV(CV cv)
        {
            foreach(var cyc in cv.Cycles)
            {
                PickPeaksCycle(cyc);
            }
        }
        private void allCVsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var cv in CVs)
            {
                PickPeaksCV(cv);
            }
            RefreshAll();
        }

        private void importSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var frm = new Import_Settings.frmImportSettings())
            {
                frm.SetSetting(Program.RuntimeData.ImportSettings);
                if(frm.ShowDialog(this)==DialogResult.OK)
                {
                    Program.RuntimeData.ImportSettings = frm.Settings;
                }

            }
        }
    }
}
