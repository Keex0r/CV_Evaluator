using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV_Evaluator.Import_Settings
{
    public class ImportSettings
    {
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
        public string GetXML()
        {
            var ser = new System.Xml.Serialization.XmlSerializer(typeof(ImportSettings));
            using (StringWriter sw = new StringWriter())
            {
                ser.Serialize(sw, this);
                return sw.ToString();
            }
        }
        public static ImportSettings FromXML(string xml)
        {
            var ser = new System.Xml.Serialization.XmlSerializer(typeof(ImportSettings));
            using (StringReader sr = new StringReader(xml))
            {
                try
                {
                    var set = ser.Deserialize(sr);
                    return (ImportSettings)set;
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
