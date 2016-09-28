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
            return FromText(input, settings, "Somewhere far beyond...");
        }
        public static List<CV> FromText(string input, Import_Settings.ImportSettings settings, string DataSource)
        {
            var start = 0;
            var rx = settings.GetDelimiterRegex();
            int ie = settings.VoltColumn;
            int ii = settings.CurrentColumn;
            int it = settings.TimeColumn;
            
            var splitcols = settings.GetSplitColumns();
            var dosplit = !settings.DontSplit;
            int maxcols = 0;
            List<CV> res = new List<CV>();
            var lines = input.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            do
            {
                var e = new List<double>();
                var i = new List<double>();
                var t = new List<double>();
                var splits = new List<string>();
                
                 foreach (string line in lines)
                {
                    var parts = rx.Split(line); //line.Split(new string[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Count() < settings.ColumnsPerCV+start) continue;
                    maxcols = Math.Max(maxcols, parts.Count());
                    double thise, thisi;
                    if (!Double.TryParse(parts[start+ie-1], out thise) || !double.TryParse(parts[start+ii-1], out thisi)) continue;
                    e.Add(thise * settings.VoltageMulti);
                    if(i.Count() > 0)
                    {
                        if (thisi == i.Last()) thisi = thisi + thisi / 1e14;
                    }
                    i.Add(thisi*settings.CurrentMulti);
                    double thist;
                    if(it>0 && it<=settings.ColumnsPerCV && double.TryParse(parts[start+it-1],out thist))
                    {
                        t.Add(thist * settings.TimeMulti);
                    } else
                    {
                        t.Add(double.NaN);
                    }
                    var split = "";
                    for(int ic=0;ic < splitcols.Count();ic++) 
                    {
                        var c = splitcols[ic]-1;
                        if (c >= 0 && c < settings.ColumnsPerCV) split += (ic>0 ? ";;" : "") + parts[start + c ];
                    }
                    splits.Add(split);
                }
                var cv = FromData(e, i, t, splits, dosplit, DataSource, settings.IgnorePoints);
                if (cv != null) res.Add(cv);
                start += settings.ColumnsPerCV;
            } while (maxcols >= start+settings.ColumnsPerCV);
            return res;
        }

        public static CV FromData(IEnumerable<double> Voltage,IEnumerable<double> Current, IEnumerable<double> Time,
            IEnumerable<string> SplitBy, bool DoSplit, string Datasource, int IgnorePoints)
        {
            if (Voltage.Count() == 0 || Current.Count() == 0 || Current.Count() != Voltage.Count()) return null;
            double[] volt = Voltage.ToArray();
            double[] currs = Current.ToArray();
            double[] times = Time.ToArray();
            string[] splits = SplitBy.ToArray();
            var issplittable = splits.Any(x => String.IsNullOrEmpty(x) == false);
            CV res = new CV();
             if(DoSplit && !issplittable)
            {
                List<Cycle> cycles = new List<Cycle>();
                double startvalue = volt[0];
                int startDeriv = Math.Sign(volt[1] - volt[0]);
                if (startDeriv == 0)
                    startDeriv = 1;
                int count = 0;
                List<List<double>> thesecols = new List<List<double>>();
                do
                {
                    Cycle thisCycle = new Cycle(res);
                    int skipped = 0;
                    bool isOriginCross = false;
                    do
                    {
                        if (skipped < IgnorePoints)
                        {
                            skipped++;
                            count++;
                            continue;
                        }
                        var thise = volt[count];
                        var thisi = currs[count];
                        var dp = new Datapoint(thisCycle);
                        dp.Current = thisi;
                        dp.Volt = thise;
                        dp.Time = times[count];
                        dp.Index = thisCycle.Datapoints.Count;
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
                            dp1.Time = times[count];
                            dp1.Index = thisCycle.Datapoints.Count;
                            thisCycle.Datapoints.Add(dp1);
                        }
                        isOriginCross = ((thisCycle.Datapoints.Count > 10) & isOriginCross) | count >= volt.Count() - 1;
                    } while (!(isOriginCross));
                    thisCycle.Number = res.Cycles.Count + 1;
                    res.Cycles.Add(thisCycle);
                    count += 1;
                } while (!(count >= volt.Count()));
            }
            else if (DoSplit && issplittable)
            {
                //Split by splitvalue
                var cycles = new Dictionary<string, Cycle>();
                var skipped = new Dictionary<string, int>();
                for (int i = 0;i<volt.Count();i++)
                {
                    if(!cycles.ContainsKey(splits[i]))
                    {
                        Cycle thisCycle = new Cycle(res);
                        thisCycle.Split = splits[i];
                        thisCycle.Number = cycles.Count() + 1;
                        cycles.Add(splits[i], thisCycle);
                        skipped.Add(splits[i], 0);
                    }
                    if(skipped[splits[i]]<IgnorePoints)
                    {
                        skipped[splits[i]]++;
                    } else
                    {
                        var dp1 = new Datapoint(cycles[splits[i]]);
                        dp1.Current = currs[i];
                        dp1.Volt = volt[i];
                        dp1.Time = times[i];
                        dp1.Index = cycles[splits[i]].Datapoints.Count;
                        cycles[splits[i]].Datapoints.Add(dp1);
                    }

                }

                ////Further subdivide
                //foreach (Cycle cyc in cycles.Values)
                //{
                //     double startvalue = cyc.Datapoints[0].Volt;
                //    int startDeriv = Math.Sign(cyc.Datapoints[1].Volt - cyc.Datapoints[0].Volt);
                //    if (startDeriv == 0)
                //        startDeriv = 1;
                //    int count = 0;
                //    do
                //    {
                //        bool isOriginCross = false;
                //        Cycle newcyc = new Cycle(res);
                //        newcyc.Split = cyc.Split;
                //        do
                //        {
                //            var thise = cyc.Datapoints[count].Volt;
                //            var thisi = cyc.Datapoints[count].Current;
                //            var dp = new Datapoint(newcyc);
                //            dp.Current = thisi;
                //            dp.Volt = thise;
                //            dp.Time = cyc.Datapoints[count].Time;
                //            dp.Index = newcyc.Datapoints.Count;
                //            newcyc.Datapoints.Add(dp);
                //            count += 1;
                //            if (count < cyc.Datapoints.Count() - 1)
                //            {
                //                if (startDeriv == -1)
                //                {
                //                    isOriginCross = cyc.Datapoints[count - 1].Volt > startvalue && cyc.Datapoints[count + 1].Volt < startvalue;
                //                }
                //                else
                //                {
                //                    isOriginCross = cyc.Datapoints[count - 1].Volt < startvalue && cyc.Datapoints[count + 1].Volt > startvalue;
                //                }
                //            }
                //            else if(count<cyc.Datapoints.Count())
                //            {
                //                var dp1 = new Datapoint(newcyc);
                //                dp1.Current = cyc.Datapoints[count].Current;
                //                dp1.Volt = cyc.Datapoints[count].Volt;
                //                dp1.Time = cyc.Datapoints[count].Time;
                //                dp1.Index = newcyc.Datapoints.Count;
                //                newcyc.Datapoints.Add(dp1);
                //            }
                //            isOriginCross = ((newcyc.Datapoints.Count > 10) & isOriginCross) | count >= cyc.Datapoints.Count() - 1;
                //        } while (!isOriginCross);
                //        res.Cycles.Add(newcyc);
                //    } while (!(count >= cyc.Datapoints.Count()-10));

                //}
                foreach (Cycle cyc in cycles.Values)
                {
                    res.Cycles.Add(cyc);
                }
            }
            else
            {
                //Dont split
                Cycle thisCycle = new Cycle(res);
                thisCycle.Split = "";
                thisCycle.Number = 1;
                for (int i = 0; i < volt.Count(); i++)
                {
                    if (i < IgnorePoints) continue;
                    var dp1 = new Datapoint(thisCycle);
                    dp1.Current = currs[i];
                    dp1.Volt = volt[i];
                    dp1.Time = times[i];
                    dp1.Index = thisCycle.Datapoints.Count;
                    thisCycle.Datapoints.Add(dp1);
                }
                res.Cycles.Add(thisCycle);
            }
            res.Datasource = Datasource;
            return res;

        }
        public string ExportPeaks()
        {
            StringBuilder res = new StringBuilder();
            foreach(var cyc in this.Cycles)
            {
                var cols = new List<string>();
                var splitparts = cyc.Split.Split(new string[] { ";;" }, StringSplitOptions.None);
                cols.Add(cyc.Number.ToString());
                cols.Add(cyc.Scanrate.ToString());
                cols.AddRange(splitparts);
                foreach(var peak in cyc.Peaks)
                {
                    cols.Add(peak.PeakPosition.ToString());
                    cols.Add(peak.PeakHeight.ToString());
                }
                var row = string.Join("\t", cols);
                res.AppendLine(row);
            }
            return res.ToString();
        }
    }
}
