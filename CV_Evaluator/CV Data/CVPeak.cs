﻿using System;
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
            ConnectedPeaks = new List<CVPeak>();
        }

        public List<CVPeak> ConnectedPeaks;


        public enDirection PeakDirection { get; set; }
        private double _CenterP;
        /// <summary>
        /// Gleitkommaindex, Position zwischen zwei Indizes, prozentual
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public double CenterP { get { return _CenterP; } set { _CenterP = value; Notify("PeakPosition"); } }

        private double _SteepestRiseIndex;
        /// <summary>
        /// Gleitkommaindex, Position zwischen zwei Indizes, prozentual
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public double SteepestRiseIndex { get { return _SteepestRiseIndex; } set { _SteepestRiseIndex = value; } }

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
        }
    }
}
