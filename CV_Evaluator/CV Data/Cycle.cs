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
            PeakConnections = new List<CVPeakConnection>();
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
        [System.ComponentModel.Browsable(false)]
        public List<CVPeakConnection> PeakConnections { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void PickPeaks(int Window, double PosMinHeight,double NegMinHeight)
        {
            this.Peaks.Clear();
            PickPeaksDirection(Window, PosMinHeight,true);
            PickPeaksDirection(Window, NegMinHeight, false);
            for(int i = 0;i<this.Peaks.Count;i++)
            {
                Peaks[i].Process = "Process " + (i + 1).ToString();
            }
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
            foreach(int p in Peaks)
            {
                if (reversals.Any(reversal => Math.Abs(p - reversal) < Window)) continue;
                
                var x1 = Datapoints[p].Time;
                var y1 = Datapoints[p].Current;
                var x2 = Datapoints[p-1].Time;
                var y2 = Datapoints[p-1].Current;
                var x3 = Datapoints[p+1].Time;
                var y3 = Datapoints[p+1].Current;
                var poly = Fitting.LinearRegression.Get2ndOrderPoly(x1, y1, x2, y2, x3, y3);
                //Since Time=Index, the extreme value is always directly the index
                var extreme = Fitting.LinearRegression.Get2ndOrderPolyExtreme(poly[0], poly[1], poly[2]);

                List<double> values = new List<double>();
                for (int i = p - 7; i <= p + 7; i++)
                {
                    values.Add(Datapoints[i].Current);
                }
                var avg = values.Average();
                if (avg / extreme.Item2 > 0.75) continue;

                var newp = new CVPeak(this);
                newp.PeakDirection = Larger ? CVPeak.enDirection.Positive : CVPeak.enDirection.Negative;
                newp.CenterP = extreme.Item1;
                newp.PeakCurrent = extreme.Item2;
                newp.AutoPickBaseline();
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
