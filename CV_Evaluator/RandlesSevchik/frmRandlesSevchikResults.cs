﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CV_Evaluator.RandlesSevchik
{
    public partial class frmRandlesSevchikResults : Form
    {
        public RandlesSevchikSettings Settings;
        public IEnumerable<CV> CVs;
        public RandlesSevchikResults Results;
        private CV CurrentCV;
        public frmRandlesSevchikResults(RandlesSevchikSettings Settings, IEnumerable<CV> CVs, CV CurrentCV)
        {
            InitializeComponent();
            this.CVs = CVs;
            this.CurrentCV = CurrentCV;
            this.Settings = Settings;
            var values = GetValues();
            var result = GetResult(values);
            tbResults.Clear();
            PlotResults(result, values);
            Results = result;
            if (result == null) return;
            tbResults.AppendText("Intercept included:\n");
            tbResults.AppendText("-------------------\n");
            tbResults.AppendText("Diffusion Coefficient: " + result.DiffusionCoefficient.ToString() + " m²/s\n");
            tbResults.AppendText("Slope                : " + result.Slope.ToString() + " A/m²*(V/s)^-0.5/s\n");
            tbResults.AppendText("Intercept            : " + result.Intercept.ToString() + " A/m^2\n");
            tbResults.AppendText("Intercept ignored:\n");
            tbResults.AppendText("------------------\n");
            tbResults.AppendText("Diffusion Coefficient: " + result.DiffCoeffNoIntercept.ToString() + " m²/s\n");
            tbResults.AppendText("Slope                : " + result.SlopeNoIntercept.ToString() + " A/m²*(V/s)^-0.5/s\n");

        }
        public List<Tuple<double,double>> GetValues()
        {
            List<Tuple<double, double>> res = new List<Tuple<double, double>>();
            if (Settings == null || CVs == null || CurrentCV==null || CVs.Count() == 0) return res;
            if(!Settings.UseCycles)
            {
                foreach (var CV in CVs)
                {
                    var cycle = CV.Cycles[Settings.CycleNumber - 1];
                    if (cycle.Scanrate == 0) continue;
                    double x = Math.Sqrt(Settings.GetScanRate(cycle.Scanrate));
                    var peak = cycle.Peaks.Where(peak1 => peak1.Process == Settings.ProcessName).Select(peak1 => peak1).FirstOrDefault();
                    if (peak == null) continue;
                    var y = peak.PeakHeight / Settings.GetArea(); ;
                    res.Add(Tuple.Create(x, y));
                }
            } else
            {
                foreach(Cycle cyc in CurrentCV.Cycles)
                {
                    if (cyc.Scanrate == 0) continue;
                    double x = Math.Sqrt(Settings.GetScanRate(cyc.Scanrate));
                    var peak = cyc.Peaks.Where(peak1 => peak1.Process == Settings.ProcessName).Select(peak1 => peak1).FirstOrDefault();
                    if (peak == null) continue;
                    var y = peak.PeakHeight / Settings.GetArea(); ;
                    res.Add(Tuple.Create(x, y));
                }
            }
            

            return res;
        }
        public RandlesSevchikResults GetResult(List<Tuple<double, double>> Values)
        {
            if (Values == null || Values.Count == 0) return null;
            double[][] reg = new double[2][];
            reg[0] = Values.Select(x => x.Item1).ToArray();
            reg[1] = Values.Select(x => x.Item2).ToArray();
            double[] res = null;
            Fitting.LinearRegression.GetRegression(reg, ref res);
            var slopenoninter = Fitting.LinearRegression.GetRegressionNoIntercept(reg);
            var result = new RandlesSevchikResults();
            result.Slope = res[0];
            result.Intercept = res[1];
            result.SlopeNoIntercept = slopenoninter;
            //D=Pow(m/(0.4463*nFAC*Sqrt(nF/RT)),2)
            var m = res[0];
            var n = Settings.z;
            var c = Settings.GetConcentration();
            const double F = 96485;
            const double R = 8.314;
            var T = Settings.GetTemperature();
            var D = Math.Pow(m / (0.4463 * n * F * c * Math.Sqrt(n * F / R / T)), 2);
            var DnoInter = Math.Pow(slopenoninter / (0.4463 * n * F * c * Math.Sqrt(n * F / R / T)), 2);
            result.DiffusionCoefficient = D;
            result.DiffCoeffNoIntercept = DnoInter;
            return result;
        }
        private void PlotResults(RandlesSevchikResults Results, List<Tuple<double, double>> Values)
        {
            jwgResults.Series.Clear();
            jwgResults.XAxis.Title = "Sqrt(v) / (V/s)^0.5";
            jwgResults.Y1Axis.Title = "j / A/m²";
            jwgResults.LegendPosition = Results.Slope > 0 ? jwGraph.jwGraph.jwGraph.enumLegendPosition.TopLeft : jwGraph.jwGraph.jwGraph.enumLegendPosition.TopRight;
            if (Results == null || Values == null) return;
            var serData = jwgResults.Series.AddSeries(jwGraph.jwGraph.Series.enumSeriesType.Scatter, jwGraph.jwGraph.Axis.enumAxisLocation.Primary);
            serData.LegendText = "Data";
            var serFit = jwgResults.Series.AddSeries(jwGraph.jwGraph.Series.enumSeriesType.Line, jwGraph.jwGraph.Axis.enumAxisLocation.Primary);
            serFit.LegendText = "Fit (with Intercept)";
            var serFitNoInter = jwgResults.Series.AddSeries(jwGraph.jwGraph.Series.enumSeriesType.Line, jwGraph.jwGraph.Axis.enumAxisLocation.Primary);
            serFitNoInter.LegendText = "Fit (without Intercept)";
            serFit.AddXY(0, Results.Intercept);
            serFitNoInter.AddXY(0, 0);
            foreach (var v in Values)
            {
                serData.AddXY(v.Item1, v.Item2);
                serFit.AddXY(v.Item1, Results.Slope * v.Item1 + Results.Intercept);
                serFitNoInter.AddXY(v.Item1, Results.SlopeNoIntercept * v.Item1);
            }
        }

    }
}
