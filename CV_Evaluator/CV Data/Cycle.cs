using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CV_Evaluator
{
    [Serializable]
    public class Cycle : INotifyPropertyChanged
    {
        public Cycle() : this(null)
        {
        }
        public Cycle(CV Parent)
        {
            this.Datapoints = new List<Datapoint>();
            Peaks = new List<CVPeak>();
            PeakConnections = new List<CVPeakConnection>();
            this.Number = -1;
            this.Parent = Parent;
            Split = "";
            Scanrate = 0.0;
            Setup();
        }

        public void Setup()
        {
            bdsPeaks = new BindingSource();
            bdsPeaks.DataSource = Peaks;
            bdsDataPoints = new BindingSource();
            bdsDataPoints.DataSource = Datapoints;
        }
        #region "Interface fields"
        [NonSerialized]
        public BindingSource bdsPeaks;
        [NonSerialized]
        public BindingSource bdsDataPoints;
        public CV Parent;
        #endregion  

        #region "Display properties"
        public double Scanrate { get; set; }
        public int Number { get; set; }
        public string Split { get; set; }
        #endregion
      
        #region "Data fields"
        public List<Datapoint> Datapoints;
        public List<CVPeak> Peaks;
        public List<CVPeakConnection> PeakConnections;
        #endregion

        public double GetVoltageStep()
        {
            double sum = 0;
            int count = 0;
            for (int i = 0; i < Datapoints.Count - 1; i++)
            {
                var thisstep= Math.Abs(Datapoints[i + 1].Volt - Datapoints[i].Volt);
                if (!(thisstep==0))
                {
                    sum += thisstep;
                    count += 1;
                }
            }
            return sum / count;
        }

        #region "Interface functions"
        public void SetScanRateFromSplit(int index)
        {
            var splits = (new Regex(";;")).Split(this.Split);
            if (index < 0 || index > Split.Count() - 1) return;
            double d;
            if (!double.TryParse(splits[index], out d)) return;
            this.Scanrate = d;
        }

        public void PickPeaks(double Window, double MinHeightPercent, double SteepnessLimit, double BaselineStdLimit, bool JustUseMinMax)
        {
            if(!JustUseMinMax) { 
                double PosMinHeight = Datapoints.Select(d => d.Current).Max() * MinHeightPercent;
                double NegMinHeight = Datapoints.Select(d => d.Current).Min() * MinHeightPercent;
                this.Peaks.Clear();
                PickPeaksDirection(Window, PosMinHeight,true, SteepnessLimit, BaselineStdLimit);
                PickPeaksDirection(Window, NegMinHeight, false, SteepnessLimit, BaselineStdLimit);
            } else
            {
                PickPeaksMinMax(BaselineStdLimit);
            }
            for(int i = 0;i<this.Peaks.Count;i++)
            {
                Peaks[i].Process = "Process " + (i + 1).ToString();
            }
        }
        #endregion
        #region "Helper functions"
        private void PickPeaksDirection(double WindowSize, double minHeight, bool Larger, double SteepnessLimit, double BaselineStdLimit)
        {
            var ReversalPoints = FindReversalPoints();
            var de = Math.Abs(GetVoltageStep());
            var Window = (int)(WindowSize / de); //Calculate point width for window
            if (Window < 3) Window = 3;
            if (Window % 2 == 0) Window += 1;
            int left = (int)((Window - 1) / 2);
            List<int> Peaks = new List<int>();
            for (int i = left; i <= Datapoints.Count - left - 1; i++)
            {
                if ((Larger && this.Datapoints[i].Current < minHeight) || (!Larger && this.Datapoints[i].Current > minHeight)) continue;
                List<double> Values = new List<double>();
                for (int c = i - left; c <= i + left; c++)
                {
                    if (c != i) Values.Add(Datapoints[c].Current);
                }
                if (Peaks.Where(p => Math.Abs(p - i) <= 1).Count() > 0) continue;
                if (this.Peaks.Where(p => Math.Abs(p.PeakCenterIndex - i) <= 1).Count() > 0) continue;
                if ((Larger && Values.All(d => d <= this.Datapoints[i].Current)) || (!Larger && Values.All(d => d >= this.Datapoints[i].Current))) Peaks.Add(i);

            }
            foreach (int p in Peaks)
            {
                if (ReversalPoints.Any(reversal => Math.Abs(p - reversal) < Window)) return;
                var newp = new CVPeak(this);
                newp.PeakCenterIndex = p;
                newp.PeakDirection = Larger ? CVPeak.enDirection.Positive : CVPeak.enDirection.Negative;
                newp.RefinePosition(BaselineStdLimit);
                if (!newp.IsSteepEnoughPeak(SteepnessLimit)) continue;
                this.Peaks.Add(newp);
            }
        }
        private void PickPeaksMinMax(double BaselineStdLimit)
        {
            var dmin = this.Datapoints.Aggregate((curmin,x) => (curmin == null || x.Current < curmin.Current ? x : curmin));
            var dmax = this.Datapoints.Aggregate((curmax, x) => (curmax == null || x.Current < curmax.Current ? curmax : x));
            var imin = this.Datapoints.IndexOf(dmin);
            var imax = this.Datapoints.IndexOf(dmax);

            var oldmin = this.Peaks.Where(p => Math.Abs(p.PeakCenterIndex - imin) <= 1).FirstOrDefault();
            var oldmax = this.Peaks.Where(p => Math.Abs(p.PeakCenterIndex - imax) <= 1).FirstOrDefault();
            if (oldmin != null) this.Peaks.Remove(oldmin);
            if (oldmax != null) this.Peaks.Remove(oldmax);
            var con = new CVPeakConnection(this);
            if (!(this.Peaks.Where(p => Math.Abs(p.PeakCenterIndex - imin) <= 1).Count() > 0))
            {
                var newpMin = new CVPeak(this);
                newpMin.PeakCenterIndex = imin;
                newpMin.PeakDirection = CVPeak.enDirection.Negative;
                newpMin.RefinePosition(BaselineStdLimit);
                con.Peak1 = newpMin;
                this.Peaks.Add(newpMin);
            }
            if (!(this.Peaks.Where(p => Math.Abs(p.PeakCenterIndex - imax) <= 1).Count() > 0))
            {
                var newpMax = new CVPeak(this);
                newpMax.PeakCenterIndex = imax;
                newpMax.PeakDirection = CVPeak.enDirection.Positive;
                newpMax.RefinePosition(BaselineStdLimit);
                con.Peak2 = newpMax;
                this.Peaks.Add(newpMax);
            }
            con.Title = "Main Process";
            this.PeakConnections.Add(con);
        }
        private List<int> FindReversalPoints()
        {
            int dif = Math.Sign(Datapoints[1].Volt - Datapoints[0].Volt);
            List<int> res = new List<int>();
            for (int i = 1; i <= Datapoints.Count() - 2; i++)
            {
                int thisdif = Math.Sign(Datapoints[i + 1].Volt - Datapoints[i].Volt);
                if (thisdif != dif)
                {
                    res.Add(i);
                    dif = thisdif;
                }
            }
            return res;
        }
        private double Derivative(int index, Func<Datapoint, double> XSelector, Func<Datapoint, double> YSelector)
        {
            var p = Datapoints;
            Datapoint d1, d2;
            if (index < 0 || index > p.Count()) return double.NaN;
            if (index > 0 && index < p.Count - 1)
            {
                // Forward/backward derivative
                d1 = p[index + 1];
                d2 = p[index - 1];
            }
            else if (index > 0)
            {
                //Forward
                d1 = p[index];
                d2 = p[index-1];
            }
            else if (index<p.Count-1)
            {
                //Backward
                d1 = p[index+1];
                d2 = p[index];
            } else
            {
                return 0.0;
            }
            double x1, x2, y1, y2;
            x1 = XSelector(d1);
            x2 = XSelector(d2);
            y1 = YSelector(d1);
            y2 = YSelector(d2);
            return (y2 - y1) / (x2 - x1);
        }
        public double DerivativeIndex(int index)
        {
            return Derivative(index, x => x.Index, x => x.Current);
        }
        public double DerivativeTime(int index)
        {
            return Derivative(index, x => x.Time, x => x.Current);
        }
        public double DerivativeVoltage(int index)
        {
            return Derivative(index, x => x.Volt, x => x.Current);
        }

        #endregion

        #region "INotifyPropertyChanged"
        private void Notify(string PropName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(PropName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        public double[][] GetConvolution()
        {
            var times = this.Datapoints.Select(x => x.Time).ToList();
            var currents = this.Datapoints.Select(x => x.Current).ToList();
            var volts = this.Datapoints.Select(x => x.Volt).ToList();
            var v = this.Scanrate;
            var dt = (volts[1] - volts[0]) / Scanrate;
            double fullvoltage = 0;
            for (int t = 0; t < times.Count()-1; t++)
            {
                fullvoltage += Math.Abs(volts[t + 1] - volts[t]);
            }
           // dt = (fullvoltage / times.Max())/Scanrate;
            for (int t=0;t<times.Count();t++)
            {
                times[t] = t * Math.Abs(dt);
            }
            List<double> convs = new List<double>();
            convs.Add(0);
            for (int t=1; t<times.Count(); t++)
            {
                var thist = new List<double>();
                var thisi = new List<double>();
                var valt = times[t];
                for(int x = 0; x < t; x++)
                {
                    thist.Add(times[x]);
                    thisi.Add(currents[x]/Math.Sqrt(valt-times[x]));
                }
                var value=Tools.Integrate(thist, thisi);
                convs.Add(value);
            }
            double[][] res=new double[4][];
            res[0]= times.ToArray();
            res[1] = volts.ToArray();
            res[2] = currents.ToArray();
            res[3] = convs.ToArray();
            return res;
        }
    }
}
