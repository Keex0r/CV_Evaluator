using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV_Evaluator.RandlesSevchik
{
   public class RandlesSevchikResults
    {
        public double DiffusionCoefficient { get; set; }
        public double Slope { get; set; }
        public double Intercept { get; set; }
        public double SlopeNoIntercept { get; set; }
        public double DiffCoeffNoIntercept { get; set; }
    }
}
