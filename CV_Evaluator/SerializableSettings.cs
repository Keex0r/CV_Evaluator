using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV_Evaluator
{
    public abstract class SerializableSettings<T>
    {
        public string GetXML()
        {
            var ser = new System.Xml.Serialization.XmlSerializer(typeof(T));
            using (StringWriter sw = new StringWriter())
            {
                ser.Serialize(sw, this);
                return sw.ToString();
            }
        }
        public static T FromXML(string xml)
        {
            var ser = new System.Xml.Serialization.XmlSerializer(typeof(T));
            using (StringReader sr = new StringReader(xml))
            {
                try
                {
                    var set = ser.Deserialize(sr);
                    return (T)set;
                }
                catch
                {
                    return default(T);
                }
            }
        }
    }
}
