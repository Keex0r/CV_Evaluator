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
            
            var ser = graph.Series.AddSeries(Program.RuntimeData.Style, jwGraph.jwGraph.Axis.enumAxisLocation.Primary);
            ser.MarkerFillColor = Color.Transparent;
            int count = 0;
            int step = 1;
            if (cv.Datapoints.Count() > 10000)
            {
                step = (int)Math.Round((double)cv.Datapoints.Count() / 1000);
            }
               for(int i=0;i<cv.Datapoints.Count();i=i+step) 
                {
                var d = cv.Datapoints[i];
                var point = ser.AddXY(d.Volt, d.Current);
                point.Tag = d;
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
                CVs.Add(cv);
            }
        }
        private void SplitBySplit()
        {
            int index = 0;
            using (var frm = new frmScanFromSplitOption())
            {
                if (frm.ShowDialog(this) == DialogResult.Cancel) return;
                index = frm.WhichSplit;
            }

            SplitBySplit(index);

        }
        private void SplitBySplit(int index)
        {
            var cv = (CV)cVBindingSource.Current;
            var cycs = new Dictionary<string, List<Cycle>>();
            foreach (var c in cv.Cycles)
            {
                var splits = (new Regex(";;")).Split(c.Split);
                if (index < 0 || index > splits.Count() - 1)
                {
                    MessageBox.Show("Invalid split");
                    return;
                }
                var k = splits[index];
                if (!cycs.ContainsKey(k)) cycs.Add(k, new List<Cycle>());
                cycs[k].Add(c);

            }
            foreach (var k in cycs.Keys)
            {
                var newcv = new CV();
                newcv.Datasource = "From Split " + k;

                foreach (var cyc in cycs[k])
                {
                    newcv.Cycles.Add((Cycle)cyc.Clone());
                }
                CVs.Add(newcv);
            }
            RefreshAll();
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
            DoRefreshCurrentCycle();
        }
        private void DoRefreshCurrentCycle()
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
            ImportExample();
        }
        private void ImportExample()
        {
            var Settings = new Import_Settings.ImportSettings();
            Settings.ColumnsPerCV = 10;
            Settings.CurrentColumn = 10;
            Settings.Delimiter = Import_Settings.ImportSettings.enDelimiter.Whitespace;
            Settings.DontSplit = false;
            Settings.TimeColumn = 4;
            Settings.VoltColumn = 5;
            Settings.SplitByColumns = "1,3";
            Settings.nHeaderLines = 5;
            var cvs = CV.FromText(CV_Evaluator.Properties.Resources.CV, Settings);
            foreach (CV cv in cvs)
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
          if(isHit)  index = ((Datapoint)point.Tag).Index;
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
        private void LaunchRandlesSevcik()
        {
            RandlesSevchik.RandlesSevchikSettings settings;
            using (var frmSettings = new RandlesSevchik.frmRandlesSevchikSetup())
            {
                if (frmSettings.ShowDialog(this) == DialogResult.Cancel) return;
                settings = frmSettings.Settings;
            }
            var thiscv = (CV)cVBindingSource.Current;
            using (var frmResult = new RandlesSevchik.frmRandlesSevchikResults(settings, this.CVs, thiscv))
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
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => RefreshAll()));
                return;
            }
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
        private void ExportPeaks()
        {
            var res = ((CV)cVBindingSource.Current).ExportPeaks();
            Clipboard.SetText(res);
        }
        private async Task CalculateConvolution()
        {
            double progress = 0.0;
            var t = new Timer();
            t.Interval = 500;
            toolStrip1.Enabled = false;
            toolStrip2.Enabled = false;

            t.Tick += (s, e1) => tlMessage.Text = (progress * 100).ToString();
            t.Start();
            var conv = await Task.Run(() => ((Cycle)cycleBindingSource.Current).GetConvolution(ref progress));
            t.Stop();
            toolStrip1.Enabled = true;
            toolStrip2.Enabled = true;
            tlMessage.Text = "Done.";
            var cv = new CV();
            var cyc = new Cycle(cv);
            for (int i = 0; i < conv[0].Count(); i++)
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


        private void SetScanrateFromSplit()
        {
            int index = 0;
            using (var frm = new frmScanFromSplitOption())
            {
                if (frm.ShowDialog(this) == DialogResult.Cancel) return;
                index = frm.WhichSplit;
            }
            SetScanrateFromSplit(index);
        }
        private void SetScanrateFromSplit(int index)
        {
            foreach (Cycle c in ((CV)cVBindingSource.Current).Cycles)
            {
                c.SetScanRateFromSplit(index);
            }
            RefreshAll();
        }

        private void SplitCyclestoNewCVs()
        {
            CV current = (CV)cVBindingSource.Current;
            for (int i = 1; i < current.Cycles.Count(); i++)
            {
                CV newcv = new CV();
                newcv.Datasource = current.Datasource;
                newcv.Cycles.Add(current.Cycles[i]);
                CVs.Add(newcv);
            }
            for (int i = current.Cycles.Count - 1; i > 0; i--)
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


        private void SetTimeFromScanrate()
        {
            var curr = (Cycle)cycleBindingSource.Current;
            curr.SetTimeFromScanrate();
        }


        private async Task InterpolateCV()
        {
            double dt = 0.1;
            bool tzero = false;
            int skips = 0;
            var fromscan = false;
            var allcycles = false;
            var InPlace = false;
            using (var frm = new frmInterpolateSettings((Cycle)cycleBindingSource.Current))
            {
                if (frm.ShowDialog(this) == DialogResult.Cancel) return;
                dt = frm.dt;
                tzero = frm.SettZero;
                skips = frm.Skips;
                fromscan = frm.FromScanrate;
                allcycles = frm.AllCycles;
                InPlace = frm.InPlace;
            }
            await InterpolateCV(dt, tzero, skips, fromscan, allcycles, InPlace);
        }
        private async Task InterpolateCV(double dt, bool tzero, int skips, bool fromscan, bool allcycles, bool InPlace)
        {
            if (allcycles)
            {
                var thiscv = (CV)cVBindingSource.Current;
                foreach (var cyc in thiscv.Cycles)
                {
                    var data = await Task.Run(() => cyc.InterpolateTo(dt, tzero, skips, fromscan));
                    if (InPlace)
                    {
                        ReplaceCycleData(cyc, data);
                    }
                    else
                    {
                        var cv = GetNewCV(data);
                        CVs.Add(cv);
                    }

                }
            }
            else
            {
                var data = await Task.Run(() => ((Cycle)cycleBindingSource.Current).InterpolateTo(dt, tzero, skips, fromscan));
                if (InPlace)
                {
                    ReplaceCycleData((Cycle)cycleBindingSource.Current, data);
                }
                else
                {
                    var cv = GetNewCV(data);
                    CVs.Add(cv);
                }
            }
            RefreshAll();
        }
        private CV GetNewCV(double[][] data)
        {
            var cv = new CV();
            var cyc = new Cycle(cv);
            for (int i = 0; i < data[0].Count(); i++)
            {
                var d = new Datapoint(cyc);
                d.Index = i;
                d.Time = data[0][i];
                d.Volt = data[1][i];
                d.Current = data[2][i];
                cyc.Datapoints.Add(d);
            }
            cv.Datasource = "Interpolation of " + ((CV)cVBindingSource.Current).Datasource;
            cv.Cycles.Add(cyc);
            return cv;
        }
        private void ReplaceCycleData(Cycle cyc, double[][] data)
        {
            cyc.PeakConnections.Clear();
            cyc.Peaks.Clear();
            cyc.Datapoints.Clear();
            for (int i = 0; i < data[0].Count(); i++)
            {
                var d = new Datapoint(cyc);
                d.Index = i;
                d.Time = data[0][i];
                d.Volt = data[1][i];
                d.Current = data[2][i];
                cyc.Datapoints.Add(d);
            }
        }

        private void ExportCurrentCycle()
        {
            var cyc = (Cycle)cycleBindingSource.Current;
            Clipboard.SetText(cyc.Export());
        }

        private void CombineEvery2ndCycle()
        {
            var cv = (CV)cVBindingSource.Current;
            List<Cycle> newCycs = new List<Cycle>();
            for (int i = 0; i < cv.Cycles.Count(); i += 2)
            {
                Cycle newcyc = new Cycle(cv);
                newcyc.Datapoints.AddRange(cv.Cycles[i].Datapoints);
                newcyc.Datapoints.AddRange(cv.Cycles[i + 1].Datapoints);
                for (int x = 0; x < newcyc.Datapoints.Count(); x++) newcyc.Datapoints[x].Index = x;
                newcyc.Number = i / 2 + 1;
                newcyc.Split = cv.Cycles[i].Split;
                newCycs.Add(newcyc);
            }
            cv.Cycles.Clear();
            cv.Cycles.AddRange(newCycs);
            RefreshAll();
        }
        private void ExportPeakSeps()
        {
            var sb = new System.Text.StringBuilder();
            foreach (CV c in CVs)
            {
                foreach (Cycle cyc in c.Cycles)
                {
                    foreach (CVPeakConnection con in cyc.PeakConnections)
                    {
                        var diff = Math.Abs(con.Peak1.GetPeakPosition().Item1 - con.Peak2.GetPeakPosition().Item1);
                        sb.AppendLine(cyc.Scanrate.ToString() + "\t" + diff.ToString());
                    }
                }
            }
            Clipboard.SetText(sb.ToString());
        }
        private void RecombineEverySecondPeaks()
        {
            foreach (CV c in CVs)
            {
                foreach (Cycle cyc in c.Cycles)
                {
                    cyc.PeakConnections.Clear();
                    for(int i = 0;i<cyc.Peaks.Count-1;i+=2)
                    {
                        var con = new CVPeakConnection();
                        con.Parent = cyc;
                        con.Peak1 = cyc.Peaks[i];
                        con.Peak2 = cyc.Peaks[i+1];
                        cyc.PeakConnections.Add(con);
                    }
                }
            }
            RefreshAll();
        }
        private void CombineFiles()
        {
            var dlg = new OpenFileDialog();
            dlg.Multiselect = true;
            if (dlg.ShowDialog(this) == DialogResult.Cancel) return;
            var f1 = dlg.FileNames[0];
            var f2 = dlg.FileNames[1];
            using (var sr1 = File.OpenText(f1))
            {
                using (var sr2 = File.OpenText(f2))
                {
                    var newfn = Path.GetFileNameWithoutExtension(f1) + "_Combine.txt";
                    var path = Path.GetDirectoryName(f1);
                    using (var sw=File.CreateText(Path.Combine(path,newfn)))
                    {
                        while(!sr1.EndOfStream && !sr2.EndOfStream)
                        {
                            var l1 = sr1.ReadLine();
                            var l2 = sr2.ReadLine();
                            sw.WriteLine(l1 + "\t" + l2);
                        }
                    }
                }
            }
        }

        private void combineTwoFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CombineFiles();
        }

        private void generateSweepsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var bla = 8.314 * 293 / 96485.0 * Math.Log(2.5 / 7.5);
            var c =new double[] { 10.0, 7.5, 5.0, 2.5, 0.0 };
            var v = new double[] { 10,25,50,100,150,200,300 };
            var Estart = new double[] { -1, bla,0,-bla,1};

            var allc = new List<string>();
            var alle = new List<string>();
            var allv = new List<string>();

            for (int ci=0;ci<c.Count();ci++)
            {
                for(int vi = 0;vi < v.Count();vi++)
                {
                    allc.Add(c[ci].ToString());
                    alle.Add(Estart[ci].ToString());
                    allv.Add(v[vi].ToString());
                }
            }
            var sb = new StringBuilder();
            sb.AppendLine("cBulkFc {" + string.Join(",", allc) + "}[mol/m^3]");
            sb.AppendLine("EStart {" + string.Join(",", alle) + "}[V]");
            sb.AppendLine("v {" + string.Join(",", allv) + "}[mV/s]");
            Clipboard.SetText(sb.ToString());
        }

        private void cVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.RuntimeData.PlotAsCV = true;
            DoRefreshCurrentCycle();
        }

        private void vsTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.RuntimeData.PlotAsCV = false;
            DoRefreshCurrentCycle();
        }

        private void lineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.RuntimeData.Style =  jwGraph.jwGraph.Series.enumSeriesType.Line;
            DoRefreshCurrentCycle();
        }

        private void scatterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.RuntimeData.Style = jwGraph.jwGraph.Series.enumSeriesType.Scatter;
            DoRefreshCurrentCycle();
        }

        private void bothToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.RuntimeData.Style = jwGraph.jwGraph.Series.enumSeriesType.LineScatter;
            DoRefreshCurrentCycle();
        }

        private async Task OneClickComsol()
        {
            SetScanrateFromSplit(1);
            await InterpolateCV(0.006, false, 0, true, true, true);
            SplitBySplit(0);
        }

        private void exportPeaksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportPeaks();
        }

        private void exportPeakSeparationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportPeakSeps();
        }

        private void exportCycleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportCurrentCycle();
        }

        private void scanrateFromSplitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetScanrateFromSplit();
        }

        private void splitCVByASplitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SplitBySplit();
        }

        private void setTimeFromScanrateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetTimeFromScanrate();
        }

        private async void interpolateTheCVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await InterpolateCV();
        }

        private async void calculateConvolutionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await CalculateConvolution();
        }

        private void randlesSevcikEvaluationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LaunchRandlesSevcik();
        }

        private async void clickComsolFixToolStripMenuItem_Click(object sender, EventArgs e)
        {
           await OneClickComsol();
        }

        private void recombineEvery2ndPeakToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RecombineEverySecondPeaks();
        }

        private void destroyTheFirstCycleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DestroyFirstCycle();
        }

        private void splitCyclesToNewCVsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SplitCyclestoNewCVs();
        }
    }
  
}

