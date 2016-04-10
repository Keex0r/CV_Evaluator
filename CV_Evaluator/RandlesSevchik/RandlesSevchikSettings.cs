﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CV_Evaluator.RandlesSevchik
{
   public class RandlesSevchikSettings
    {
        public RandlesSevchikSettings()
        {
            ProcessName = "Process 1";
            CycleNumber = 1;
            Temperature = 298.15;
            ElectrodeArea = 1;
            z = 1;
            ScanrateUnit = enScanrateUnit.mVs;
            Concentration = 5;
            ConcentrationUnit = enConcentrationUnit.molm3;
        }

        public enum enScanrateUnit
        {
            [Description("V/s")]
            Vs,
            [Description("mV/s")]
            mVs
        }
        public enum enAreaUnit
        {
            [Description("m²")]
            m2,
            [Description("cm²")]
            cm2,
            [Description("mm²")]
            mm2
        }
        public enum enConcentrationUnit
        {
            [Description("mol/m³ = mmol/L")]
            molm3,
            [Description("mol/L")]
            molL,
        }
        public enum enTemperatureUnit
        {
            [Description("K")]
            K,
            [Description("°C")]
            C,
        }

        public string ProcessName { get; set; }
        public int CycleNumber { get; set; }
        public enScanrateUnit ScanrateUnit { get; set; }
        public double Temperature { get; set; }
        public enTemperatureUnit TemperatureUnit { get; set; }
        public double ElectrodeArea { get; set; }
        public enAreaUnit ElectrodeAreaUnit { get; set; }
        public double Concentration { get; set; }
        public enConcentrationUnit ConcentrationUnit { get; set; }
        public double z { get; set; }

        public double GetScanRate(double Scanrate)
        {
            if(ScanrateUnit == enScanrateUnit.Vs)
            {
                return Scanrate;
            } else
            {
                return Scanrate / 1000;
            }
        }
        public double GetArea()
        {
            if(ElectrodeAreaUnit== enAreaUnit.m2)
            {
                return ElectrodeArea;
            } else if (ElectrodeAreaUnit == enAreaUnit.cm2)
            {
                return ElectrodeArea / 10000;
            } else
            {
                return ElectrodeArea / 1e6;
            }
        }
        public double GetConcentration()
        {
            if (ConcentrationUnit ==  enConcentrationUnit.molm3)
            {
                return Concentration;
            }
            else
            {
                return Concentration*1000;
            }
        }
        public double GetTemperature()
        {
            if (TemperatureUnit ==  enTemperatureUnit.K)
            {
                return Temperature;
            }
            else
            {
                return Temperature+273.15;
            }
        }

        public string GetXML()
        {
            var ser = new System.Xml.Serialization.XmlSerializer(typeof(RandlesSevchikSettings));
            using (StringWriter sw = new StringWriter())
            {
                ser.Serialize(sw, this);
                return sw.ToString(); 
            }
        }
        public static RandlesSevchikSettings FromXML(string xml)
        {
            var ser = new System.Xml.Serialization.XmlSerializer(typeof(RandlesSevchikSettings));
            using (StringReader sr = new StringReader(xml))
            {
                try
                {
                    var set = ser.Deserialize(sr);
                    return (RandlesSevchikSettings)set;
                } catch
                {
                    return null;
                }
               

            }
        }
    }
}
