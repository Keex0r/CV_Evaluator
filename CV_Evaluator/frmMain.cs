using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            frmPeakPicking.LoadSettings();
            frmPeakPicking.Owner = this;
            frmPeakPicking.Show();
            jwGraph1.XAxis.Title = "E / V";
            jwGraph1.Y1Axis.Title = "I / A";
            foreach(Control c in this.Controls)
            {
                if (c.GetType() == typeof(DataGridView) || c.GetType() == typeof(jwGraph.jwGraph.jwGraph))
                {
                    c.DragEnter += dgvCVs_DragEnter;
                    c.DragDrop += dgvCVs_DragDrop;
                    c.AllowDrop = true;
                }
                AddHandlers(c.Controls);
            }
            
        }
        private void AddHandlers(System.Windows.Forms.Control.ControlCollection controls)
        {
            foreach(Control c in controls)
            {
                if(c.GetType()==typeof(DataGridView) || c.GetType() == typeof(jwGraph.jwGraph.jwGraph))
                {
                    c.DragEnter += dgvCVs_DragEnter;
                    c.DragDrop += dgvCVs_DragDrop;
                    c.AllowDrop = true;
                }
                AddHandlers(c.Controls);
            }
        }

        private void PlotCV(Cycle cv, jwGraph.jwGraph.jwGraph graph)
        {
            graph.BeginUpdate();
            graph.Series.Clear();
            
            var ser = graph.Series.AddSeries(jwGraph.jwGraph.Series.enumSeriesType.Line, jwGraph.jwGraph.Axis.enumAxisLocation.Primary);
            int count = 0;
            int step = 1;
            if (cv.Datapoints.Count() > 10000)
            {
                step = (int)Math.Round((double)cv.Datapoints.Count() / 1000);
            }
               for(int i=0;i<cv.Datapoints.Count();i=i+step) 
                {
                var d = cv.Datapoints[i];
                ser.AddXY(d.Volt, d.Current);
                count++;
            }
            graph.Tag = cv;
            graph.EndUpdate();
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
        private void ReadFile(string File)
        {
            //try
            //{
                var cvs = CV.FromText(System.IO.File.ReadAllText(File), Program.RuntimeData.ImportSettings);
                foreach (CV cv in cvs)
                {
                    cv.Datasource = File;
                    var rx = new Regex("(?:([0-9]+)(mVs)|([0-9]+)(Vs))", RegexOptions.IgnoreCase);
                    if(rx.IsMatch(File))
                    {
                        var m = rx.Match(File);
                        double fac = 1.0;
                        if (m.Groups[2].Value.ToLower() == "mvs") fac = 1e-3;
                        double scanrate = 0;
                        if(jwGraph.GeneralTools.modGeneralTools.TryGetNumericValue(m.Groups[1].Value,ref scanrate))
                        {
                            scanrate *= fac;
                            cv.ScanRate = scanrate;
                        }
                    }
                    PickPeaksCV(cv);
                    CVs.Add(cv);
                }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Could not load file! " + ex.Message);
            //}
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

        private int BLpointselect = -1;
        private CVPeak workpeak;
        private bool PeakPicker = false;

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (cVPeakBindingSource.Current == null) return;
            if(BLpointselect>-1)
            {
                BLpointselect = -1;
                tlMessage.Text = "";
                return;
            }
            CVPeak peak = (CVPeak)cVPeakBindingSource.Current;
            PeakPicker = false;
            BLpointselect = 1;
            workpeak = peak;
            tlMessage.Text = "Select 1st point.";
        }


        private void jwGraph1_MouseDown(object sender, MouseEventArgs e)
        {
            jwGraph.jwGraph.Series ser = null;
            jwGraph.jwGraph.Datapoint point = null;
            int index = -1;
            var isHit = jwGraph1.PointHitTest(e.Location, ref ser, ref index, ref point);

            if (BLpointselect == -1 && PeakPicker == false) return;

            if (isHit)
            {
                if(PeakPicker)
                {
                    if (cycleBindingSource.Current == null) return;
                    Cycle cyc = (Cycle)cycleBindingSource.Current;
                    var peak = new CVPeak(cyc);

                    peak.PeakCenterIndex = index;

                    if (frmPeakPicking == null) return;
                    var sets = frmPeakPicking.GetSettings();
                    peak.RefinePosition(sets.BaselineStdDevLimit);
                    cyc.Peaks.Add(peak);
                    cVPeakBindingSource.ResetBindings(true);
                    
                    dgvPeaks.Refresh();

                    return;
                }
                if(BLpointselect==1)
                {
                    workpeak.BaselineP1 = index;
                    BLpointselect = 2;
                    tlMessage.Text = "Select 2nd point.";
                } else
                {
                    workpeak.BaselineP2 = index;
                    BLpointselect = -1;
                    dgvPeaks.Refresh();
                    tlMessage.Text = "Done.";
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
            cyc.PickPeaks(sets.Window, sets.MinHeight, sets.SteepnessLimit,sets.BaselineStdDevLimit,sets.JustUseMaxMin);
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

        private void dgvCVs_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.All;
        }

        private void dgvCVs_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (var f in files)
                {
                    ReadFile(f);
                }
            }
        }

        private void tsbPickPeak_Click(object sender, EventArgs e)
        {
            PeakPicker = !PeakPicker;
            BLpointselect = -1;
            if (PeakPicker) tlMessage.Text = "Click Peaks.";
            else tlMessage.Text = "";

        }

        private double RoundSig(double x, int p)
        {
            if (double.IsNaN(x) || double.IsInfinity(x) || x==0) return x;
            var sign = Math.Sign(x);
            x = Math.Abs(x);
            var n = Math.Floor(Math.Log10(x)) + 1 - p;
            return sign * Math.Round(Math.Pow(10, -n) * x) * Math.Pow(10, n);
        }
        private void dgvPeaks_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            double test;
            if(double.TryParse(e.Value.ToString(),out test))
            {
                var d = test;
                e.Value = RoundSig(d, 4).ToString();
                e.FormattingApplied = true;
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            var res = ((CV)cVBindingSource.Current).ExportPeaks();
            Clipboard.SetText(res);
        }

        private async void toolStripButton6_Click(object sender, EventArgs e)
        {
            double progress = 0.0;
            var t = new Timer();
            t.Interval = 500;
            toolStrip1.Enabled = false;
            toolStrip2.Enabled = false;
            
            t.Tick += (s, e1) => tlMessage.Text = (progress * 100).ToString();
            t.Start();
            var conv = await Task.Run(()=>((Cycle)cycleBindingSource.Current).GetConvolution(ref progress));
            t.Stop();
            toolStrip1.Enabled = true;
            toolStrip2.Enabled = true;
            tlMessage.Text = "Done.";
            var cv = new CV();
            var cyc = new Cycle(cv);
            //res[0] = times.ToArray();
            //res[1] = volts.ToArray();
            //res[2] = currents.ToArray();
            //res[3] = convs.ToArray();
            for (int i=0;i<conv[0].Count();i++)
            {
                var d = new Datapoint(cyc);
                d.Index = i;
                d.Time = conv[0][i];
                d.Volt = conv[1][i];
                d.Current = conv[3][i];
                cyc.Datapoints.Add(d);
            }
            cv.Datasource = "Convolution of " + ((CV)cVBindingSource.Current).Datasource;
            cv.Cycles.Add(cyc);
            CVs.Add(cv);
            }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            foreach(Cycle c in ((CV)cVBindingSource.Current).Cycles)
            {
                c.SetScanRateFromSplit(0);
            }
            RefreshAll();
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            CV current = (CV)cVBindingSource.Current;
            for(int i = 1; i < current.Cycles.Count(); i++)
            {
                CV newcv = new CV();
                newcv.Datasource = current.Datasource;
                newcv.Cycles.Add(current.Cycles[i]);
                CVs.Add(newcv);
            }
            for(int i = current.Cycles.Count - 1; i > 0; i--)
            {
                current.Cycles.RemoveAt(i);
            }
            RefreshAll();
        }
        private void Delete10()
        {
            foreach (CV c in CVs)
            {
                foreach (Cycle cyc in c.Cycles)
                {
                    for(int i=1;i<=10;i++)
                    {
                        cyc.Datapoints.RemoveAt(0);
                    }
                }
            }
        }
        private void DestroyFirstCycle()
        {
            var srs = new List<double>(new double[] { 0.025,0.05,0.1,0.15,0.25,0.5 });
            int scount = 0;
            foreach(CV c in CVs)
            {
                c.ScanRate = srs[scount];
                scount++;
                foreach (Cycle cyc in c.Cycles)
                {
                    cyc.PeakConnections.Clear();
                    cyc.Peaks.Clear();
                    //Further subdivide
                    double startvalue = cyc.Datapoints[0].Volt;
                    int startDeriv = Math.Sign(cyc.Datapoints[1].Volt - cyc.Datapoints[0].Volt);
                    if (startDeriv == 0)
                        startDeriv = 1;
                    int count = 0;

                    bool isOriginCross = false;
                    do
                    {
                        var thise = cyc.Datapoints[count].Volt;
                        var thisi = cyc.Datapoints[count].Current;
                        count += 1;
                        if (count < cyc.Datapoints.Count() - 1)
                        {
                            if (startDeriv == -1)
                            {
                                isOriginCross = cyc.Datapoints[count - 1].Volt > startvalue && cyc.Datapoints[count + 1].Volt < startvalue;
                            }
                            else
                            {
                                isOriginCross = cyc.Datapoints[count - 1].Volt < startvalue && cyc.Datapoints[count + 1].Volt > startvalue;
                            }
                        }
                        else if (count < cyc.Datapoints.Count())
                        {
                        }
                        isOriginCross = isOriginCross | count >= cyc.Datapoints.Count() - 1;
                    } while (!isOriginCross);

                    for (int d = 0; d < count+10; d++)
                    {
                        cyc.Datapoints.RemoveAt(0);
                    }
                    for (int d = 0; d < cyc.Datapoints.Count(); d++)
                    {
                        cyc.Datapoints[d].Index = d;
                    }
                }
            }


            RefreshAll();
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            DestroyFirstCycle();
        }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            var curr = (Cycle)cycleBindingSource.Current;
            curr.SetTimeFromScanrate();
        }
    }
  
}

