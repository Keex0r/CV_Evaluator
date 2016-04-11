using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CV_Evaluator
{
    class Tools
    {
        public static double stdev(IEnumerable<double> values)
        {
            double mean = values.Average();
            List<double> diffs = new List<double>();
            foreach (var d in values)
            {
                diffs.Add(Math.Abs(d - mean));
            }
            return diffs.Average();
        }
        public static double Interpolate(double value, double OldMin, double OldMax, double NewMin, double NewMax)
        {
            return ((NewMax - NewMin) / (OldMax - OldMin)) * (value - OldMin) + NewMin;
        }

        //https://stackoverflow.com/questions/7422685/edit-the-display-name-of-enumeration-members-in-a-propertygrid
        //By LarsTech
        //Original by:
        // Using PropertyGrid in .NET
        //By zaletskiy yura on Jan 04, 2008 
        //http://www.c-sharpcorner.com/uploadfile/witnes/using-propertygrid-in-net/
        public class EnumStringConverter : System.ComponentModel.EnumConverter
        {
            private Type _enumType;

            public EnumStringConverter(Type type) : base(type) {
                _enumType = type;
            }

            public override bool CanConvertTo(ITypeDescriptorContext context, Type destType)
            {
                return destType == typeof(string);
            }

            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType)
            {
                FieldInfo fi = _enumType.GetField(Enum.GetName(_enumType, value));
                DescriptionAttribute dna = (DescriptionAttribute)Attribute.GetCustomAttribute(fi, typeof(DescriptionAttribute));
                if (dna != null)
                    return dna.Description;
                else
                    return value.ToString();
            }

            public override bool CanConvertFrom(ITypeDescriptorContext context, Type srcType)
            {
                return srcType == typeof(string);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                foreach (FieldInfo fi in _enumType.GetFields())
                {
                    DescriptionAttribute dna = (DescriptionAttribute)Attribute.GetCustomAttribute(fi, typeof(DescriptionAttribute));
                    if ((dna != null) && ((string)value == dna.Description))
                        return Enum.Parse(_enumType, fi.Name);
                }
                return Enum.Parse(_enumType, (string)value);
            }
        }
    }
}
