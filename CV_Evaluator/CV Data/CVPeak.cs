using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV_Evaluator
{
    class CVPeak
    {
        public CVPeak(Cycle Parent)
        {
            this.Parent = Parent;
            BaselineP1 = -1;
            BaselineP2 = -1;
            Process = "Misc";
        }
        /// <summary>
        /// Gleitkommaindex, Position zwischen zwei Indizes, prozentual
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public double CenterP { get; set; }
        public double PeakCurrent { get; set; }
        [System.ComponentModel.Browsable(false)]
        public int BaselineP1 { get; set; }
        [System.ComponentModel.Browsable(false)]
        public int BaselineP2 { get; set; }
        [System.ComponentModel.Browsable(false)]
        public Cycle Parent;
        public string Process { get; set; }
        [System.ComponentModel.Browsable(false)]
        public Tuple<double,double> GetCenterPos
        {
            get
            {
                double modu = CenterP % 1;
                double diff = this.Parent.Datapoints[(int)Math.Ceiling(CenterP)].Volt - this.Parent.Datapoints[(int)Math.Floor(CenterP)].Volt;
                var peakx = Parent.Datapoints[(int)Math.Floor(CenterP)].Volt + diff * modu;
                return Tuple.Create(peakx, PeakCurrent);
            }
        }
        [System.ComponentModel.Browsable(false)]
        public double BaseLineCurrentAtPeak
        {
            get
            {
                if (BaselineP1 != -1 && BaselineP2 != -1)
                {
                    List<double> x = new List<double>();
                    List<double> y = new List<double>();
                    for(int i = BaselineP1;i<=BaselineP2;i++)
                    {
                        x.Add(Parent.Datapoints[i].Volt);
                        y.Add(Parent.Datapoints[i].Current);
                    }
                    double[][] vals = { x.ToArray(), y.ToArray() };
                    double[] result = null;
                    Fitting.LinearRegression.GetRegression(vals, ref result);
                    var b1 = BaselineValues1;
                    var b2 = BaselineValues2;
                    var m = (b2.Item2 - b1.Item2) / (b2.Item1 - b1.Item1);
                    var b = b1.Item2 - m * b1.Item1;
                    var bypeak = m * GetCenterPos.Item1 + b;
                    return bypeak;
                } else
                {
                    return 0.0;
                }
            }
        }
        [System.ComponentModel.Browsable(false)]
        public double RealPeakHeight {
            get
            {
                return PeakCurrent - BaseLineCurrentAtPeak;
            }
        }
        [System.ComponentModel.Browsable(false)]
        private Tuple<double,double> GetValues(int Index)
        {
            if (Index != -1)
                return Tuple.Create(Parent.Datapoints[Index].Volt, Parent.Datapoints[Index].Current);
            else
                return Tuple.Create(0.0, 0.0);

        }
        [System.ComponentModel.Browsable(false)]
        public Tuple<Double, Double> BaselineValues1
        {
            get
            {
                return GetValues(BaselineP1);
            }
        }
        [System.ComponentModel.Browsable(false)]
        public Tuple<double, double> BaselineValues2
        {
            get
            {
                return GetValues(BaselineP2);
            }
        }

    }
}
