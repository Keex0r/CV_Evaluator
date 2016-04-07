using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV_Evaluator
{
    class Datapoint
    {
        public Datapoint(Cycle Parent)
        {
            this.Parent = Parent;
        }
        public Cycle Parent;
        public double Volt { get; set; }
        public double Current { get; set; }
        public double Time { get; set; }

    }
}
