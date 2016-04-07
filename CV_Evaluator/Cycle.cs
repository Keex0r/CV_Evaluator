using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV_Evaluator
{
    class Cycle
    {
        public Cycle(CV Parent)
        {
            this.Datapoints = new List<Datapoint>();
            Peaks = new List<CVPeak>();
            this.Number = -1;
            this.Parent = Parent;
            Scanrate = 0.0;
            CVPeak example = new CVPeak(this);
            example.CenterP = 116;
            example.BaselineP1 = 91;
            example.BaselineP2 = 102;
            example.PeakCurrent = -8.106e-6;
            this.Peaks.Add(example);
        }
        public double Scanrate { get; set; }
        public CV Parent { get; set; }
        public int Number { get; set; }
        public List<Datapoint> Datapoints { get; set; }
        public List<CVPeak> Peaks { get; set; }

    }
}
