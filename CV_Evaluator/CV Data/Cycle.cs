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
    public class Cycle : INotifyPropertyChanged, ICloneable
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
        public void SetTimeFromScanrate()
        {
            if (this.Scanrate <= 0) return;
            var dt = Math.Abs((this.Datapoints[1].Volt - this.Datapoints[0].Volt) / Scanrate);
            for(int i = 0; i < this.Datapoints.Count(); i++)
            {
                this.Datapoints[i].Time = dt * i;
            }
        }
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
            double mincurr = double.PositiveInfinity;
            double maxcurr = double.NegativeInfinity;
            Datapoint dmin, dmax;
            int imin=0, imax=0;
            for(int i = 0; i < Datapoints.Count(); i++)
            {
                if(Datapoints[i].Current>maxcurr)
                {
                    imax = i;
                    maxcurr = Datapoints[i].Current;
                    dmax = Datapoints[i];
                }
                if (Datapoints[i].Current < mincurr)
                {
                    imin = i;
                    mincurr = Datapoints[i].Current;
                    dmin = Datapoints[i];
                }
            }

            //var dmin = this.Datapoints.Aggregate((curmin,x) => (curmin == null || x.Current < curmin.Current ? x : curmin));
            //var dmax = this.Datapoints.Aggregate((curmax, x) => (curmax == null || x.Current < curmax.Current ? curmax : x));
            //var imin = this.Datapoints.IndexOf(dmin);
            //var imax = this.Datapoints.IndexOf(dmax);

            //var oldmin = this.Peaks.Where(p => Math.Abs(p.PeakCenterIndex - imin) <= 1).FirstOrDefault();
            //var oldmax = this.Peaks.Where(p => Math.Abs(p.PeakCenterIndex - imax) <= 1).FirstOrDefault();
            //if (oldmin != null) this.Peaks.Remove(oldmin);
            //if (oldmax != null) this.Peaks.Remove(oldmax);
            this.Peaks.Clear();

            var con = new CVPeakConnection(this);
            if (!(this.Peaks.Where(p => Math.Abs(p.PeakCenterIndex - imax) <= 1).Count() > 0))
            {
                var newpMax = new CVPeak(this);
                newpMax.PeakCenterIndex = imax;
                newpMax.PeakDirection = CVPeak.enDirection.Positive;
                newpMax.RefinePosition(BaselineStdLimit);
                con.Peak2 = newpMax;
                this.Peaks.Add(newpMax);
            }
            if (!(this.Peaks.Where(p => Math.Abs(p.PeakCenterIndex - imin) <= 1).Count() > 0))
            {
                var newpMin = new CVPeak(this);
                newpMin.PeakCenterIndex = imin;
                newpMin.PeakDirection = CVPeak.enDirection.Negative;
                newpMin.RefinePosition(BaselineStdLimit);
                con.Peak1 = newpMin;
                this.Peaks.Add(newpMin);
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
        private List<double> xValues,yValues;
        private double t;
        private double IntegrationFunction(double x)
        {
            return Interpolate(x, xValues, yValues)/Math.Sqrt(t-x);
        }
        public static double Interpolate(double pos, List<double> x, List<double> y)
        {
            int ind = x.BinarySearch(pos);
            if (ind>=0 && ind < x.Count()-1)
            {
                //pos is an actual item in the list
                return y[ind] + (y[ind + 1] - y[ind]) * (pos - x[ind]) / (x[ind + 1] - x[ind]);
            }
            else if (ind >= 0)
            {
                //pos is larger than the largest item in the list?
                return y[ind-1] + (y[ind] - y[ind-1]) * (pos - x[ind-1]) / (x[ind] - x[ind-1]);
            }
            else
            {
                //pos is not in the list
                ind *= -1;
                ind -= 2;
                //ind is now the index of the first item larger than pos -1
                if(ind==x.Count-1)
                {
                    return y[ind-1] + (y[ind] - y[ind-1]) * (pos - x[ind-1]) / (x[ind] - x[ind-1]);
                }
                else if(ind < x.Count()-1) 
                {
                    return y[ind] + (y[ind + 1] - y[ind]) * (pos - x[ind]) / (x[ind + 1] - x[ind]);
                } else if(ind<0)
                {
                    return y.First();
                } else
                {
                    return y.Last();
                }
            }
        }

        public double[][] GetConvolution(ref double progress)
        {
            var tmin = this.Datapoints.Select(x => x.Time).Min();
            var times = this.Datapoints.Select(x => x.Time-tmin).ToList();
            var currents = this.Datapoints.Select(x => x.Current).ToList();
            var volts = this.Datapoints.Select(x => x.Volt).ToList();
            var convs = new List<double>();
            xValues = times;
            yValues = currents;
            var max = times.Count();
            progress = 0;
            for(int i=0;i<times.Count();i++)
            {
                t = times[i];
                var value2 = MathNet.Numerics.Integration.DoubleExponentialTransformation.Integrate(IntegrationFunction, 0, t, 1e-12) / Math.Sqrt(Math.PI);
                convs.Add(value2);
                progress = (double)i / (double)max;
            }
            double[][] res=new double[4][];
            res[0]= times.ToArray();
            res[1] = volts.ToArray();
            res[2] = currents.ToArray();
            res[3] = convs.ToArray();
            return res;
        }

        private List<int> FindVertices()
        {
            var thist = this.Datapoints.Select(x => x.Time).ToList();
            var thisE = this.Datapoints.Select(x => x.Volt).ToList();
            var dedt = (thisE[1] - thisE[0]) / (thist[1] - thist[0]);
            var currsign = Math.Sign(dedt);
            int i = 0;
            List<int> res = new List<int>();
            res.Add(0);
            do
            {
                var thisdedt = (thisE[i + 1] - thisE[i]) / (thist[i + 1] - thist[i]);
                if (!Double.IsNaN(thisdedt) && Math.Sign(thisdedt) != currsign)
                {
                    res.Add(i);
                    currsign = Math.Sign(thisdedt);
                }
                i++;
            } while (i < thist.Count()-1);
            res.Add(thist.Count() - 1);
            return res;
        }

        public double[][] InterpolateTo(double tstep, bool SettZero,int Skips, bool FromScanrate)
        {
            var thist = this.Datapoints.Select(x => x.Time).ToList();
            var thisE = this.Datapoints.Select(x => x.Volt).ToList();
            var thisI = this.Datapoints.Select(x => x.Current).ToList();
            List<double> newEs = new List<double>();
            List<double> newIs = new List<double>();
            List<double> newts = new List<double>();
            double dt;
            if(!FromScanrate)
            {
                dt = tstep;
            } else
            {
                dt = 1/this.Scanrate * tstep;
            }
            //(tmax - tmin) / (nPoints - 1);

            var verts = FindVertices();
            for(int verti = Skips; verti < verts.Count() - 1; verti++)
            {
                var tstart = thist[verts[verti]];
                var tend = thist[verts[verti+1]];
                
                newEs.Add(thisE[verts[verti]]);
                newIs.Add(thisI[verts[verti]]);
                newts.Add(thist[verts[verti]]);
                var t = tstart+dt;
                do
                {
                    var newe = Interpolate(t, thist, thisE);
                    var newi = Interpolate(t, thist, thisI);
                    newEs.Add(newe);
                    newIs.Add(newi);
                    newts.Add(t);
                    t += dt;
                } while (t<tend);
                newEs.Add(thisE[verts[verti + 1]]);
                newIs.Add(thisI[verts[verti + 1]]);
                newts.Add(thist[verts[verti + 1]]);
            }
            if(SettZero)
            {
                var tstart = newts.Min();
                for(int i = 0;i<newts.Count();i++)
                {
                    newts[i] = newts[i] - tstart;
                }
            }
            double[][] res = new double[3][];
            res[0] = newts.ToArray();
            res[1] = newEs.ToArray();
            res[2] = newIs.ToArray();
            return res;
        }
        public string Export()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Index\tTime\tVoltage\tCurrent");
            for (int i=0;i<this.Datapoints.Count();i++)
            {
                var d = this.Datapoints[i];
                sb.AppendLine(d.Index.ToString() + "\t" + d.Time.ToString() + "\t" + d.Volt.ToString() + "\t" + d.Current.ToString());
            }
            return sb.ToString();
        }

        public object Clone()
        {
            var cyc = new Cycle(this.Parent);
            cyc.Number = Number;
            cyc.Scanrate = Scanrate;
            cyc.Split = Split;
            foreach(var d in Datapoints)
            {
                var newd = new Datapoint(cyc);
                newd.Current = d.Current;
                newd.Index = d.Index;
                newd.Time = d.Time;
                newd.Volt = d.Volt;
                cyc.Datapoints.Add(newd);
            }
            foreach(var p in Peaks)
            {
                var newp = new CVPeak(cyc);
                newp.BaselineP1 = p.BaselineP1;
                newp.BaselineP2 = p.BaselineP2;
                newp.PeakCenterIndex = p.PeakCenterIndex;
                newp.PeakDirection = p.PeakDirection;
                newp.Process = p.Process;
                newp.RawPeakCurrent = p.RawPeakCurrent;
                newp.SteepestRiseIndex = p.SteepestRiseIndex;
                cyc.Peaks.Add(newp);
            }
            foreach(var c in PeakConnections)
            {
                var newc = new CVPeakConnection(cyc);
                newc.Peak1 = cyc.Peaks[Peaks.IndexOf(c.Peak1)];
                newc.Peak2 = cyc.Peaks[Peaks.IndexOf(c.Peak2)];
                newc.Title = c.Title;
                cyc.PeakConnections.Add(newc);
            }
            return cyc;
        }
    }
}
