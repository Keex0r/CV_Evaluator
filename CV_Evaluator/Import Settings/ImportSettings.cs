﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
        }
        public enum enDelimiter
        {
            Tab,
            Whitespace,
            Comma,
            Dot,
            Semicolon
        }
        public int ColumnsPerCV { get; set; }
        public int IgnorePoints { get; set; }
        public int VoltColumn { get; set; }
        public int CurrentColumn { get; set; }
        public int TimeColumn { get; set; }
        public enDelimiter Delimiter { get; set; }
        public string SplitByColumns { get; set; }
        public bool DontSplit { get; set; }
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
