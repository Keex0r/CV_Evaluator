using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Drawing;

namespace CV_Evaluator
{
    class CVPeak : INotifyPropertyChanged
    {
        public enum enDirection
        {
            Positive,
            Negative,
        }
        public CVPeak(Cycle Parent)
        {
            this.Parent = Parent;
            BaselineP1 = -1;
            BaselineP2 = -1;
            Process = "Misc";
            SteepestRiseIndex = -1;
            ConnectedPeaks = new List<CVPeak>();
        }

        public List<CVPeak> ConnectedPeaks;


        public enDirection PeakDirection { get; set; }
        private double _CenterP;
        /// <summary>
        /// Gleitkommaindex, Position zwischen zwei Indizes, prozentual
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public double CenterP { get { return _CenterP; } set { _CenterP = value; PickSteepestRise(); Notify("PeakPosition"); } }

        private double _SteepestRiseIndex;
        /// <summary>
        /// Gleitkommaindex, Position zwischen zwei Indizes, prozentual
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public double SteepestRiseIndex { get { return _SteepestRiseIndex; } set { _SteepestRiseIndex = value; } }

        public double SteepestRiseVoltage { get { return GetSteepestRisePosition.Item1; } }

        private double _PeakCurrent;
        [System.ComponentModel.Browsable(false)]
        public double PeakCurrent { get { return _PeakCurrent; } set { _PeakCurrent = value; Notify("PeakHeight"); } }

        private int _BaselineP1;
        [System.ComponentModel.Browsable(false)]
        public int BaselineP1 { get { return _BaselineP1; } set { _BaselineP1 = value; Notify("PeakHeight"); } }

        private int _BaselineP2;
        [System.ComponentModel.Browsable(false)]
        public int BaselineP2 { get { return _BaselineP2; } set { _BaselineP2 = value; Notify("PeakHeight"); } }

        public Cycle Parent;

        private void Notify(string PropName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
           if(handler != null) PropertyChanged(this, new PropertyChangedEventArgs(PropName));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private string _Process;
        public string Process { get { return _Process; } set { _Process = value; Notify("Process"); } }

        private double Interpolate(double index, Double ValuePrev, double ValueNext)
        {
            double modu = index % 1;
            double diff = ValueNext - ValuePrev;
            var peakx = ValuePrev + diff * modu;
            return peakx;
        }
        private double GetDifferentialPoint(double index)
        {
            return Interpolate(index, this.Parent.Datapoints[(int)Math.Floor(index)].Volt , this.Parent.Datapoints[(int)Math.Ceiling(index)].Volt);
        }

        [System.ComponentModel.Browsable(false)]
        public Tuple<double,double> GetCenterPos
        {
            get
            {
                return Tuple.Create(GetDifferentialPoint(CenterP), PeakCurrent); ;
            }
        }
        [System.ComponentModel.Browsable(false)]
        public Tuple<double, double> GetSteepestRisePosition
        {
            get
            {
                var x = GetDifferentialPoint(SteepestRiseIndex);
                var y = Interpolate(SteepestRiseIndex, this.Parent.Datapoints[(int)Math.Floor(SteepestRiseIndex)].Current, this.Parent.Datapoints[(int)Math.Ceiling(SteepestRiseIndex)].Current);

                return Tuple.Create(x,y) ;
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

        public double PeakHeight {
            get
            {
                return PeakCurrent - BaseLineCurrentAtPeak;
            }
        }
        public double PeakPosition
        {
            get
            {
                return GetCenterPos.Item1;
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

        public void DrawYourself(Graphics g, jwGraph.jwGraph.jwGraph graph)
        {
            var peakpos = this.GetCenterPos;
            var peakp = graph.ValuesToPixelposition(peakpos, jwGraph.jwGraph.Axis.enumAxisLocation.Primary);
            var hasBaseline = this.BaselineP1 != -1 && this.BaselineP2 != -1;
            if (!hasBaseline)
            {
                var zeroy = graph.Y1Axis.ValueToPixelPosition(0) + graph.GraphBorder.Top;
                var p1 = new PointF(peakp.X, zeroy);
                using (Pen p = new Pen(Brushes.Brown, 3))
                {
                    g.DrawLine(p, peakp, p1);
                }
            }
            else
            {
                var b1 = this.BaselineValues1;
                var b2 = this.BaselineValues2;
                var bypeak = this.BaseLineCurrentAtPeak;
                var ypos = graph.Y1Axis.ValueToPixelPosition(bypeak) + graph.GraphBorder.Top;
                var p1 = new PointF(peakp.X, ypos);
                using (Pen p = new Pen(Brushes.Brown, 3))
                {
                    g.DrawLine(p, peakp, p1);
                    g.DrawLine(p, graph.ValuesToPixelposition(b1, jwGraph.jwGraph.Axis.enumAxisLocation.Primary), p1);
                    g.DrawLine(p, graph.ValuesToPixelposition(b2, jwGraph.jwGraph.Axis.enumAxisLocation.Primary), p1);
                }
            }
            //Draw steepest rise
            if(SteepestRiseIndex>-1)
            {
                var p = graph.ValuesToPixelposition(GetSteepestRisePosition, jwGraph.jwGraph.Axis.enumAxisLocation.Primary);
                g.DrawLine(Pens.Blue, p.X, p.Y - 20, p.X, p.Y + 20);
                g.DrawLine(Pens.Blue, p.X - 20, p.Y, p.X + 20, p.Y);
            }
        }
        private double Derivative(int index)
        {
            var p = this.Parent.Datapoints;
            if (index>0 && index < p.Count-1)
            {
                // Forward/backward derivative
                return (p[index + 1].Current - p[index - 1].Current) / (p[index + 1].Time - p[index - 1].Time);
            } else if (index > 0)
            {
                //Forward
                return (p[index + 1].Current - p[index].Current) / (p[index + 1].Time - p[index].Time);
            } else
            {
                //Backward
                return (p[index].Current - p[index - 1].Current) / (p[index].Time - p[index - 1].Time);
            }
        }
        public void PickSteepestRise()
        {
            var data = this.Parent.Datapoints;
            int start = (int)CenterP;
            //int dif = Math.Sign(data[1].Volt-data[0].Volt);
            //Iteriere in Richtung -dif bis die nächste und übernächste Ableitung kleiner ist als die aktuelle
            double thisdiv;
            double nextdiv;
            double nextnextdiv;
            do
            {
                start--;
                thisdiv = Derivative(start);
                nextdiv = Derivative(start - 1);
                nextnextdiv = Derivative(start - 2);
            } while (PeakDirection == enDirection.Positive ? thisdiv < nextdiv : thisdiv > nextdiv);// || thisdiv < nextnextdiv);
            var p = start;
            var x1 = Parent.Datapoints[p].Time;
            var y1 = Derivative(p);
            var x2 = Parent.Datapoints[p - 1].Time;
            var y2 = Derivative(p - 1);
            var x3 = Parent.Datapoints[p + 1].Time;
            var y3 = Derivative(p + 1);
            var poly = Fitting.LinearRegression.Get2ndOrderPoly(x1, y1, x2, y2, x3, y3);
            var extreme = Fitting.LinearRegression.Get2ndOrderPolyExtreme(poly[0], poly[1], poly[2]);

            SteepestRiseIndex =  extreme.Item1;
        }
        private double stdev(IEnumerable<double> values)
        {
            double mean = values.Average();
            List<double> diffs = new List<double>();
            foreach(var d in values)
            {
                diffs.Add(Math.Abs(d - mean));
            }
            return diffs.Average();
        }
        public void AutoPickBaseline()
        {
            //Start at steepest rise, go back
            //Find 6? points with standard deviation <5e-9
            //Set Baseline to these extremes
            int start = (int)SteepestRiseIndex;
            double deriv;
            bool OK = false;
            bool isZeroCross = false;
            int window = 5;
            do
            {
                start--;
                List<double> values = new List<double>();
                double lastd = 0;
                for (int i = 0; i < window; i++)
                {
                    var thisd = Derivative(start - i);
                    if(i>0 && Math.Sign(lastd) != Math.Sign(thisd))
                    {
                        isZeroCross = true;
                        start = start - i;
                        break;
                    }
                    values.Add(thisd);
                    lastd = thisd;
                }
                deriv = stdev(values);
                if (isZeroCross || deriv <= 5e-9) OK = true;
            } while (!OK && start > (int)SteepestRiseIndex - 30);
            if(OK)
            {
                if(!isZeroCross)
                {
                    BaselineP1 = start;
                    BaselineP2 = start - window + 1;
                } else
                {
                    BaselineP1 = start + 2;
                    BaselineP2 = start - 2;
                }
                
            }
            }
        } 
    }

