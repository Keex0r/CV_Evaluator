using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Data.Linq;
using System.Linq;

    namespace Fitting
    {
        public class LinearRegression
        {
            private static Tuple<double, double> getmeanxy(double[][] data)
            {
                double gesamtx = 0;
                double gesamty = 0;
                int count = 0;
                count = 0;
                gesamtx = 0;
                gesamty = 0;
                int i;
                for (i = 0; i <= data[0].Count() - 1; i++)
                {
                    gesamtx += data[0][i];
                    gesamty += data[1][i];
                    count += 1;
                }
                Tuple<double, double> res = new Tuple<double, double>(gesamtx / count, gesamty / count);
                return res;
            }
            public static double RSquare(double[][] data, double[] result)
            {
                Tuple<double, double> means = getmeanxy(data);
                double variance = 0;
                double ess = 0;
                int i;
                for (i = 0; i <= data[0].Count() - 1; i++)
                {
                    variance += Math.Pow((data[1][i] - means.Item2), 2);
                    ess += Math.Pow((data[1][i] - (result[0] * data[0][i] + result[1])), 2);
                }
                return 1 - ess / variance;
            }

            /// <summary>
            /// Result will contain Slope as Result(0) and Intercept as Result(1)
            /// </summary>
            /// <param name="data">{X,Y}</param>
            /// <param name="ResultParams">{Slope, Intercept}</param>
            public static void GetRegression(double[][] data, ref double[] ResultParams)
            {
                Tuple<double, double> means = getmeanxy(data);
                double zaehler = 0;
                double nenner = 0;
                zaehler = 0;
                nenner = 0;
                int i;
                for (i = 0; i <= data[0].Count() - 1; i++)
                {
                    zaehler += (data[0][i] - means.Item1) * (data[1][i] - means.Item2);
                    nenner += Math.Pow((data[0][i] - means.Item1), 2);
                }
                double Steigung = 0;
                bool nan = false;
                if (nenner != 0)
                {
                    Steigung = zaehler / nenner;
                }
                else if (zaehler != 0 & nenner == 0)
                {
                    Steigung = 0;
                }
                else if (zaehler == 0 & nenner == 0)
                {
                    nan = true;
                    Steigung = double.PositiveInfinity;
                }
                double[] res = new double[2];
                if (nan == false)
                {
                    double b = means.Item2 - Steigung * means.Item1;
                    res[0] = Steigung;
                    res[1] = b;
                }
                else
                {
                    res[0] = double.NaN;
                    res[1] = double.NaN;
                }
                ResultParams = res;
            }
        }
    }
