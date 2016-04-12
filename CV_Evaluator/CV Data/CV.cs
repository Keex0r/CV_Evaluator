using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CV_Evaluator
{
    [Serializable]
   public class CV
    {
        public CV()
        {
            this.Cycles = new List<Cycle>();
            this.Datasource = "";
            Setup();
        }
        public void Setup()
        {
            bdsCycles = new BindingSource();
            bdsCycles.DataSource = Cycles;
            foreach (var cyc in Cycles) cyc.Setup();
        }
        public Cycle CurrentCycle()
        {
            if (bdsCycles == null) return null;
            return (Cycle)bdsCycles.Current;
        }
        [NonSerialized]
        public BindingSource bdsCycles;

        public double ScanRate
        {
            get
            {
                if(this.Cycles==null || this.Cycles.Count==0)
                {
                    return 0.0;
                } else
                {
                    return (this.Cycles.Select(x => x.Scanrate).Average());
                }
            }
            set
            {
                foreach (var cyc in Cycles) cyc.Scanrate = value;
            }
        }
        public List<Cycle> Cycles { get; set; }

        public string Datasource { get; set; }

        public int nCycles
        {
            get
            {
                if (Cycles == null) return 0;
                return Cycles.Count();
            }
        }


        public static List<CV> FromText(string input, Import_Settings.ImportSettings settings)
        {
            var start = 0;
            var rx = settings.GetDelimiterRegex();
            int ie = settings.VoltColumn;
            int ii = settings.CurrentColumn;
            int it = settings.TimeColumn;
            int maxcols = 0;
            List<CV> res = new List<CV>();
            var lines = input.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            do
            {
                var e = new List<double>();
                var i = new List<double>();
                var t = new List<double>();
                 foreach (string line in lines)
                {
                    var parts = rx.Split(line); //line.Split(new string[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Count() < settings.ColumnsPerCV+start) continue;
                    maxcols = Math.Max(maxcols, parts.Count());
                    double thise, thisi;
                    if (!Double.TryParse(parts[start+ie-1], out thise) || !double.TryParse(parts[start+ii-1], out thisi)) continue;
                    e.Add(thise);
                    i.Add(thisi);
                }
                var cv = FromData(e, i, true);
                res.Add(cv);
                start += settings.ColumnsPerCV;
            } while (maxcols > start+settings.ColumnsPerCV);
            return res;
        }

        public static CV FromData(IEnumerable<double> Voltage,IEnumerable<double> Current, bool ByStartCrossing)
        {
            if (Voltage.Count() == 0 || Current.Count() == 0 || Current.Count() != Voltage.Count()) return null;
            double[] volt = Voltage.ToArray();
            double[] currs = Current.ToArray();
            CV res = new CV();
            List<Cycle> cycles = new List<Cycle>();
            if(ByStartCrossing)
            {
                double startvalue = volt[0];
                int startDeriv = Math.Sign(volt[1] - volt[0]);
                if (startDeriv == 0)
                    startDeriv = 1;
                int count = 0;
                List<List<double>> thesecols = new List<List<double>>();
                do
                {
                    Cycle thisCycle = new Cycle(res);
                    bool isOriginCross = false;
                    do
                    {
                        var thise = volt[count];
                        var thisi = currs[count];
                        var dp = new Datapoint(thisCycle);
                        dp.Current = thisi;
                        dp.Volt = thise;
                        dp.Time = thisCycle.Datapoints.Count;
                        thisCycle.Datapoints.Add(dp);
                        count += 1;
                        if (count < volt.Count() - 1)
                        {
                            if (startDeriv == -1)
                            {
                                isOriginCross = volt[count - 1] > startvalue && volt[count + 1] < startvalue;
                            }
                            else
                            {
                                isOriginCross = volt[count - 1] < startvalue && volt[count + 1] > startvalue;
                            }
                        }
                        else
                        {
                            var dp1 = new Datapoint(thisCycle);
                            dp1.Current = currs[count];
                            dp1.Volt = volt[count];
                            dp1.Time = thisCycle.Datapoints.Count;
                            thisCycle.Datapoints.Add(dp1);
                        }
                        isOriginCross = ((thisCycle.Datapoints.Count > 10) & isOriginCross) | count >= volt.Count() - 1;
                    } while (!(isOriginCross));
                    thisCycle.Number = res.Cycles.Count + 1;
                    res.Cycles.Add(thisCycle);
                    
                    count += 1;
                } while (!(count >= volt.Count()));

            }
            res.Datasource = "Somewhere far beyond...";
            return res;

        }
    }
}
