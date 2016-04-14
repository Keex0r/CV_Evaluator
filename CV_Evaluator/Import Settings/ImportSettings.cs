using System;
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
        public int VoltColumn { get; set; }
        public int CurrentColumn { get; set; }
        public int TimeColumn { get; set; }
        public enDelimiter Delimiter { get; set; }

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
    }
}
