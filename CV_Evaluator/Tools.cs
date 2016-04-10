using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV_Evaluator
{
    class Tools
    {
        public static double stdev(IEnumerable<double> values)
        {
            double mean = values.Average();
            List<double> diffs = new List<double>();
            foreach (var d in values)
            {
                diffs.Add(Math.Abs(d - mean));
            }
            return diffs.Average();
        }
        public static double Interpolate(double value, double OldMin, double OldMax, double NewMin, double NewMax)
        {
            return ((NewMax - NewMin) / (OldMax - OldMin)) * (value - OldMin) + NewMin;
        }

    }
}
