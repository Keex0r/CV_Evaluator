﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV_Evaluator
{
    class CV
    {
        public CV()
        {
            this.Cycles = new List<Cycle>();
            this.Datasource = "";
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
        public static CV FromText(string input, string delimiter)
        {
            var e = new List<double>();
            var i = new List<double>();
            var lines = input.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach(string line in lines)
            {
                var parts = line.Split(new string[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Count() < 2) continue;
                double thise, thisi;
                if (!Double.TryParse(parts[0], out thise) || !double.TryParse(parts[1], out thisi)) continue;
                e.Add(thise);
                i.Add(thisi);
            }
            return FromData(e, i, true);

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
                            dp.Current = currs[count];
                            dp.Volt = volt[count];
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
