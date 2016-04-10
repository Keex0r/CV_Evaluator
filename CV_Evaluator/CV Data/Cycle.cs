using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CV_Evaluator
{
    public class Cycle : INotifyPropertyChanged
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

        #region "Interface fields"
        public BindingSource bdsPeaks;
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

        #region "Interface functions"
        public void PickPeaks(int Window, double MinHeightPercent, double SteepnessLimit)
        {
            double PosMinHeight = Datapoints.Select(d => d.Current).Max() * MinHeightPercent;
            double NegMinHeight = Datapoints.Select(d => d.Current).Min() * MinHeightPercent;
            this.Peaks.Clear();
            PickPeaksDirection(Window, PosMinHeight,true, SteepnessLimit);
            PickPeaksDirection(Window, NegMinHeight, false, SteepnessLimit);
            for(int i = 0;i<this.Peaks.Count;i++)
            {
                Peaks[i].Process = "Process " + (i + 1).ToString();
            }
        }
        #endregion
        #region "Helper functions"
        private void PickPeaksDirection(int Window, double minHeight, bool Larger, double SteepnessLimit)
        {
            var ReversalPoints = FindReversalPoints();
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
                newp.RefinePosition();
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
