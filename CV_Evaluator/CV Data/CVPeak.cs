﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Drawing;

namespace CV_Evaluator
{
    [Serializable]
    public class CVPeak : INotifyPropertyChanged
    {
        public CVPeak() :this(null) { }
        public CVPeak(Cycle Parent)
        {
            this.Parent = Parent;
            BaselineP1 = -1;
            BaselineP2 = -1;
            Process = "Misc";
            SteepestRiseIndex = -1;
        }

        public enum enDirection
        {
            Positive,
            Negative,
        }
     

       
        #region "Misc"
        public Cycle Parent;

        #endregion

        #region "Feature indizes"
        private double _PeakCenterIndex;
        /// <summary>
        /// Gleitkommaindex, Position zwischen zwei Indizes, prozentual
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public double PeakCenterIndex { get { return _PeakCenterIndex; } set { _PeakCenterIndex = value; PickSteepestRise(); Notify("PeakPosition"); } }

        private double _SteepestRiseIndex;
        /// <summary>
        /// Gleitkommaindex, Position zwischen zwei Indizes, prozentual
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public double SteepestRiseIndex { get { return _SteepestRiseIndex; } set { _SteepestRiseIndex = value; } }

        private int _BaselineP1;
        [System.ComponentModel.Browsable(false)]
        public int BaselineP1 { get { return _BaselineP1; } set { _BaselineP1 = value; Notify("PeakHeight"); } }

        private int _BaselineP2;
        [System.ComponentModel.Browsable(false)]
        public int BaselineP2 { get { return _BaselineP2; } set { _BaselineP2 = value; Notify("PeakHeight"); } }
        #endregion

        #region "Display properties"
        public enDirection PeakDirection { get; set; }
        public double SteepestRiseVoltage { get { return GetSteepestRisePosition().Item1; } }
        public double PeakHeight
        {
            get
            {
                return RawPeakCurrent - ExtrapolateBaselineToPeak();
            }
        }
        public double PeakPosition
        {
            get
            {
                return GetPeakPosition().Item1;
            }
        }
        #endregion

        #region "Data"

        private double _PeakCurrent;
        public double RawPeakCurrent { get { return _PeakCurrent; } set { _PeakCurrent = value; Notify("PeakHeight"); } }

        private string _Process;
        public string Process { get { return _Process; } set { _Process = value; Notify("Process"); } }

        #endregion

        #region "INotifyPropertyChanged"
        private void Notify(string PropName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) PropertyChanged(this, new PropertyChangedEventArgs(PropName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        private double GetDifferentialValue(double index, Func<Datapoint,double> Selector)
        {
            var data = Parent.Datapoints;
            var lower = (int)Math.Floor(index);
            var upper = (int)Math.Ceiling(index);

            return Tools.Interpolate(index, lower, upper, Selector(data[lower]), Selector(data[upper]));
        }

        public Tuple<double,double> GetPeakPosition()
        {
                var x = GetDifferentialValue(PeakCenterIndex, d => d.Volt);
                var y = RawPeakCurrent;
                return Tuple.Create(x, y);
        }

        public Tuple<double, double> GetSteepestRisePosition()
        {
                var x = GetDifferentialValue(SteepestRiseIndex, d => d.Volt);
                var y = GetDifferentialValue(SteepestRiseIndex, d => d.Current); 
                return Tuple.Create(x,y);
        }

        private double ExtrapolateBaselineToPeak()
        {
              if (BaselineP1 != -1 && BaselineP2 != -1)
                {
                List<double> x = new List<double>();
                List<double> y = new List<double>();
                for (int i = BaselineP1; i <= BaselineP2; i++)
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
                var bypeak = m * GetPeakPosition().Item1 + b;
                return bypeak;
            } else
                {
                return 0.0;
            }
        }


        private Tuple<double,double> GetValues(int Index)
        {
            if (Index != -1)
                return Tuple.Create(Parent.Datapoints[Index].Volt, Parent.Datapoints[Index].Current);
            else
                return Tuple.Create(0.0, 0.0);

        }
        private Tuple<Double, Double> BaselineValues1
        {
            get
            {
                return GetValues(BaselineP1);
            }
        }
        private Tuple<double, double> BaselineValues2
        {
            get
            {
                return GetValues(BaselineP2);
            }
        }

        public void DrawYourself(Graphics g, jwGraph.jwGraph.jwGraph graph)
        {
            var peakpos = this.GetPeakPosition();
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
                var bypeak = this.ExtrapolateBaselineToPeak();
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
                var p = graph.ValuesToPixelposition(GetSteepestRisePosition(), jwGraph.jwGraph.Axis.enumAxisLocation.Primary);
                g.DrawLine(Pens.Blue, p.X, p.Y - 20, p.X, p.Y + 20);
                g.DrawLine(Pens.Blue, p.X - 20, p.Y, p.X + 20, p.Y);
            }
        }

        public bool IsSteepEnoughPeak(double height)
        {
            var data = Parent.Datapoints;
            var index = (int)PeakCenterIndex;
            List<double> values = new List<double>();
            for (int i = index - 7; i <= index + 7; i++)
            {
                values.Add(data[i].Current);
            }
            var avg = values.Average();
            return (avg / RawPeakCurrent < 0.75);

        }
        public void RefinePosition()
        {
            if (PeakCenterIndex == -1) return;
            var index = (int)PeakCenterIndex;
            var data = Parent.Datapoints;
            var x1 = data[index].Time;
            var y1 = data[index].Current;
            var x2 = data[index - 1].Time;
            var y2 = data[index - 1].Current;
            var x3 = data[index + 1].Time;
            var y3 = data[index + 1].Current;
            var poly = Fitting.LinearRegression.Get2ndOrderPoly(x1, y1, x2, y2, x3, y3);
            //Since Time=Index, the extreme value is always directly the index
            var extreme = Fitting.LinearRegression.Get2ndOrderPolyExtreme(poly[0], poly[1], poly[2]);

            PeakCenterIndex = extreme.Item1;
            RawPeakCurrent = extreme.Item2;
            AutoPickBaseline();
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

        private void PickSteepestRise()
        {
            var data = this.Parent.Datapoints;
            int start = (int)PeakCenterIndex;
            //int dif = Math.Sign(data[1].Volt-data[0].Volt);
            //Iteriere in Richtung -dif bis die nächste und übernächste Ableitung kleiner ist als die aktuelle
            double thisdiv;
            double nextdiv;
            double nextnextdiv;
            do
            {
                start--;
                thisdiv = Math.Abs(Derivative(start));
                nextdiv = Math.Abs(Derivative(start - 1));
                nextnextdiv = Math.Abs(Derivative(start - 2));
            } while (start > 2 && thisdiv < nextdiv);// || thisdiv < nextnextdiv);
            if (start <= 2) return;
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

        
        private void AutoPickBaseline()
        {
            //Start at steepest rise, go back
            //Find 6? points with standard deviation <5e-9
            //Set Baseline to these extremes
            //When a zero crossing accurs, use this point (and adjecent) for the baseline
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
                deriv = Tools.stdev(values);
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

