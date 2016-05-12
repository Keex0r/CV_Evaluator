using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
        public void PickPeaks(double Window, double MinHeightPercent, double SteepnessLimit, double BaselineStdLimit)
        {
            double PosMinHeight = Datapoints.Select(d => d.Current).Max() * MinHeightPercent;
            double NegMinHeight = Datapoints.Select(d => d.Current).Min() * MinHeightPercent;
            this.Peaks.Clear();
            PickPeaksDirection(Window, PosMinHeight,true, SteepnessLimit, BaselineStdLimit);
            PickPeaksDirection(Window, NegMinHeight, false, SteepnessLimit, BaselineStdLimit);
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

                if ((Larger && Values.All(d => d < this.Datapoints[i].Current)) || (!Larger && Values.All(d => d > this.Datapoints[i].Current))) Peaks.Add(i);

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
    }
}
