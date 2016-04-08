using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CV_Evaluator
{
    class Cycle : INotifyPropertyChanged
    {
        public Cycle(CV Parent)
        {
            this.Datapoints = new List<Datapoint>();
            Peaks = new List<CVPeak>();
            this.Number = -1;
            this.Parent = Parent;
            Scanrate = 0.0;
            bdsPeaks = new BindingSource();
            bdsPeaks.DataSource = Peaks;
            bdsDataPoints = new BindingSource();
            bdsDataPoints.DataSource = Datapoints;
        }

        public BindingSource bdsPeaks;
        public BindingSource bdsDataPoints;

        private void Notify(string PropName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(PropName));
        }

        public int nPeaks { get; set; }
        public double Scanrate { get; set; }
        [System.ComponentModel.Browsable(false)]
        public CV Parent;
        [System.ComponentModel.Browsable(false)]
        public int Number { get; set; }
        [System.ComponentModel.Browsable(false)]
        public List<Datapoint> Datapoints { get; set; }
        [System.ComponentModel.Browsable(false)]
        public List<CVPeak> Peaks { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void PickPeaks(int Window, double PosMinHeight,double NegMinHeight)
        {
            this.Peaks.Clear();
            PickPeaksDirection(Window, PosMinHeight,true);
            PickPeaksDirection(Window, NegMinHeight, false);
        }
        public void PickPeaksDirection(int Window, double minHeight, bool Larger)
        {
            var reversals = FindReversalPoints();
            if (Window % 2 == 0) Window += 1;
            int left = (int)((Window-1) / 2);
            List<int> Peaks = new List<int>();
            for (int i = left; i <= Datapoints.Count-left-1;i++)
            {
                if ((Larger && this.Datapoints[i].Current < minHeight) || (!Larger && this.Datapoints[i].Current > minHeight)) continue;
                List<double> Values = new List<double>();
                for(int c=i-left;c<=i+left;c++)
                {
                    if (c != i) Values.Add(Datapoints[c].Current);
                }

                if ((Larger && Values.All(d => d < this.Datapoints[i].Current)) || (!Larger && Values.All(d => d > this.Datapoints[i].Current))) Peaks.Add(i);

            }
            foreach(var p in Peaks)
            {
                if (reversals.Any(reversal => Math.Abs(p - reversal) < Window)) continue;

                var x1 = Datapoints[p].Volt;
                var y1 = Datapoints[p].Current;
                var x2 = Datapoints[p-1].Volt;
                var y2 = Datapoints[p-1].Current;
                var x3 = Datapoints[p+1].Volt;
                var y3 = Datapoints[p+1].Current;
                var poly = Fitting.LinearRegression.Get2ndOrderPoly(x1, y1, x2, y2, x3, y3);
                var extreme = Fitting.LinearRegression.Get2ndOrderPolyExtreme(poly[0], poly[1], poly[2]);
                bool dif = Datapoints[p].Volt - Datapoints[p - 1].Volt >= 0 ? true : false;
                double point;
                if ((dif && extreme.Item1 >= Datapoints[p].Volt) || (!dif && extreme.Item1 <= Datapoints[p].Volt))
                {
                    var dx = Datapoints[p+1].Volt - Datapoints[p].Volt;
                    var de = extreme.Item1- Datapoints[p].Volt;
                    var fract = de / dx;
                    point = p + fract;
                } else
                {
                    var dx = Datapoints[p].Volt - Datapoints[p-1].Volt;
                    var de = extreme.Item1 - Datapoints[p-1].Volt;
                    var fract = de / dx;
                    point = (p-1) + fract;
                }

                var newp = new CVPeak(this);
                newp.PeakDirection = Larger ? CVPeak.enDirection.Positive : CVPeak.enDirection.Negative;
                newp.CenterP = point;
                newp.PeakCurrent = extreme.Item2;
                this.Peaks.Add(newp);
            }
        }
     
        private List<int> FindReversalPoints()
        {
            int dif = Math.Sign(Datapoints[1].Volt - Datapoints[0].Volt);
            List<int> res = new List<int>();
            for(int i=1;i<=Datapoints.Count()-2;i++)
            {
                int thisdif = Math.Sign(Datapoints[i+1].Volt - Datapoints[i].Volt);
                if (thisdif != dif)
                {
                    res.Add(i);
                    dif = thisdif;
                }
            }
            return res;
        }

        private void FindSteepestRise(CVPeak peak)
        {

        }
    }
}
