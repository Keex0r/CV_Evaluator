using System.ComponentModel;

namespace CV_Evaluator.Matsuda
{
    public class MatsudaSettings : SerializableSettings<MatsudaSettings>
    {
        public MatsudaSettings()
        {
            ProcessName = "Process 1";
            CycleNumber = 1;
            Temperature = 293;
            ElectrodeArea = 1;
            z = 1;
            ScanrateUnit = enScanrateUnit.mVs;
            Concentration = 5;
            ConcentrationUnit = enConcentrationUnit.molm3;
            DRed = 1e-11;
            DOx = 1e-11;
            alpha = 0.5;
        }
        [TypeConverter(typeof(Tools.EnumStringConverter))]
        public enum enScanrateUnit
        {
            [Description("V/s")]
            Vs,
            [Description("mV/s")]
            mVs
        }
        [TypeConverter(typeof(Tools.EnumStringConverter))]
        public enum enAreaUnit
        {
            [Description("m²")]
            m2,
            [Description("cm²")]
            cm2,
            [Description("mm²")]
            mm2
        }
        [TypeConverter(typeof(Tools.EnumStringConverter))]
        public enum enConcentrationUnit
        {
            [Description("mol/m³ = mmol/L")]
            molm3,
            [Description("mol/L")]
            molL,
        }
        [TypeConverter(typeof(Tools.EnumStringConverter))]
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
        public double DRed { get; set; }
        public double DOx { get; set; }
        public double alpha { get; set; }


        public double GetScanRate(double Scanrate)
        {
            if (ScanrateUnit == enScanrateUnit.Vs)
            {
                return Scanrate;
            }
            else
            {
                return Scanrate / 1000;
            }
        }
        public double GetArea()
        {
            if (ElectrodeAreaUnit == enAreaUnit.m2)
            {
                return ElectrodeArea;
            }
            else if (ElectrodeAreaUnit == enAreaUnit.cm2)
            {
                return ElectrodeArea / 10000;
            }
            else
            {
                return ElectrodeArea / 1e6;
            }
        }
        public double GetConcentration()
        {
            if (ConcentrationUnit == enConcentrationUnit.molm3)
            {
                return Concentration;
            }
            else
            {
                return Concentration * 1000;
            }
        }
        public double GetTemperature()
        {
            if (TemperatureUnit == enTemperatureUnit.K)
            {
                return Temperature;
            }
            else
            {
                return Temperature + 273.15;
            }


        }
    }
}
