using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.ComponentModel;

namespace CV_Evaluator.Import_Settings
{
    public class ImportSettings : SerializableSettings<ImportSettings>
    {
        public ImportSettings()
        {
            ColumnsPerCV = 2;
            VoltColumn = 1;
            CurrentColumn = 2;
            TimeColumn = -1;
            IgnorePoints = 0;
            SplitByColumns = "";
            DontSplit = false;
            Delimiter = enDelimiter.Tab;
            CurrentMulti = 1.0;
            VoltageMulti = 1.0;
            TimeMulti = 1.0;
            InterpolateToNPoints = -1;
            nHeaderLines = 0;
        }
        public enum enDelimiter
        {
            Tab,
            Whitespace,
            Comma,
            Dot,
            Semicolon
        }
        public void SetFrom(ImportSettings sets)
        {
            CurrentMulti = sets.CurrentMulti;
            VoltageMulti = sets.VoltageMulti;
            TimeMulti = sets.TimeMulti;
            ColumnsPerCV = sets.ColumnsPerCV;
            IgnorePoints = sets.IgnorePoints;
            VoltColumn = sets.VoltColumn;
            CurrentColumn = sets.CurrentColumn;
            TimeColumn = sets.TimeColumn;
            Delimiter = sets.Delimiter;
            SplitByColumns = sets.SplitByColumns;
            DontSplit = sets.DontSplit;
            InterpolateToNPoints = sets.InterpolateToNPoints;
        }
        [Category("General file structure")]
        public int InterpolateToNPoints { get; set; }
        [Category("Multiplier")]
        public double CurrentMulti { get; set; }
        [Category("Multiplier")]
        public double VoltageMulti { get; set; }
        [Category("Multiplier")]
        public double TimeMulti { get; set; }
        [Category("General file structure")]
        public int ColumnsPerCV { get; set; }
        [Category("General file structure")]
        public int IgnorePoints { get; set; }
        [Category("Column Designations")]
        public int VoltColumn { get; set; }
        [Category("Column Designations")]
        public int CurrentColumn { get; set; }
        [Category("Column Designations")]
        public int TimeColumn { get; set; }
        [Category("General file structure")]
        public enDelimiter Delimiter { get; set; }
        [Category("General file structure")]
        public string SplitByColumns { get; set; }
        [Category("General file structure")]
        public bool DontSplit { get; set; }
        [Category("General file structure")]
        public int nHeaderLines { get; set; }
        public Regex GetDelimiterRegex()
        {
            if(Delimiter==enDelimiter.Comma)
            {
                return new Regex(",");
            } else if(Delimiter==enDelimiter.Dot)
            {
                return new Regex("\\.{1,}");
            }
            else if (Delimiter == enDelimiter.Semicolon)
            {
                return new Regex(";");
            }
            else if (Delimiter == enDelimiter.Tab)
            {
                return new Regex("\\t");
            }
            else //Whitespace
            {
                return new Regex("\\s{1,}");
            }
        }
        public List<int> GetSplitColumns()
        {
            var res = new List<int>();
            if (string.IsNullOrEmpty(SplitByColumns)) return res;
            var parts = SplitByColumns.Split(',');
            int col;
            if (parts.Any(c => !int.TryParse(c, out col))) return res;
            foreach (string c in parts) res.Add(int.Parse(c));
            return res;
        }
    }
}
