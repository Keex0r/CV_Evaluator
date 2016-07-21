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

        public static double InterpolateXY(IEnumerable<double> XValues, IEnumerable<double> YValues, double x)
        {
            var Index=-1;
            var xv = XValues.ToArray();
            var yv = YValues.ToArray();
            for (int i = 0;i<XValues.Count()-1;i++)
            {
                if(xv[i]<=x && xv[i+1]>=x)
                {
                    Index = i;
                    break;
                }
            }
            if (Index != -1)
            {
                var x1 = xv[Index];
                var x2 = xv[Index + 1];
                var y1 = yv[Index];
                var y2 = yv[Index + 1];
                return Interpolate(x, x1, x2, y1, y2);
            }else
            {
                var x1 = xv[xv.Count()-2];
                var x2 = xv[xv.Count() - 1];
                var y1 = yv[xv.Count() - 2];
                var y2 = yv[xv.Count() - 1];
                return double.NaN;
            }
        }
        public static double Integrate(IEnumerable<double> XValues, IEnumerable<double> YValues)
        {
            double sum = 0;
            var X = XValues.ToArray();
            var Y = YValues.ToArray();
            for (int i = 0; i <= X.Count() - 2; i++)
            {
                double deltaX = X[i + 1] - X[i];

                double g = X[i];
                double h = X[i + 1];

                double[] linfit = null;
                double[] thisx = { g, h };
                double[] thisy = { Y[i], Y[i + 1] };
                Fitting.LinearRegression.GetRegression(thisx,thisy, ref linfit);

            double m = linfit[0];
            double b = linfit[1];

            double area1 = (-b) * g + b * h - (Math.Pow(g, 2) * m) / 2 + (Math.Pow(h, 2) * m) / 2;

            
            sum += area1;
        }
	return sum;
}


    public static double stdev(IEnumerable<double> values)
        {
            if (values == null || values.Count() == 0) return Double.NaN;
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
