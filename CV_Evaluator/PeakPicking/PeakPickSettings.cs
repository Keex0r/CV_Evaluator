using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV_Evaluator.PeakPicking
{
    public class PeakPickSettings : SerializableSettings<PeakPickSettings>
    {
        public double SteepnessLimit { get; set; }
        public double MinHeight { get; set; }
        public double Window { get; set; }
        public double BaselineStdDevLimit { get; set; }
        public bool JustUseMaxMin { get; set; }
    }
}
