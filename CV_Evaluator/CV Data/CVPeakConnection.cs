using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV_Evaluator
{
    public class CVPeakConnection : IEquatable<CVPeakConnection>
    {
        public CVPeakConnection(Cycle Parent)
        {
            Title = "";
            this.Parent = Parent;
        }
        public string Title { get; set; }
        public CVPeak Peak1;
        public CVPeak Peak2;
        public Cycle Parent;
        public bool Valid {
            get
            {
                return (Peak1 != null && Peak2 != null && Parent.Peaks.Contains(Peak1) && Parent.Peaks.Contains(Peak2));
            }
        }

        public bool Equals(CVPeakConnection other)
        {
            if (this.Parent != other.Parent) return false;
            if ((this.Peak1 == other.Peak1 && this.Peak2 == other.Peak2) || (this.Peak1 == other.Peak2 && this.Peak2 == other.Peak1)) return true;
            return false;

        }
        public void DrawYourself(Graphics g, jwGraph.jwGraph.jwGraph graph)
        {
            var p1AbsPos = Peak1.GetPeakPosition();
            var p1GraphPos = graph.ValuesToPixelposition(p1AbsPos, jwGraph.jwGraph.Axis.enumAxisLocation.Primary);
            var p2AbsPos = Peak2.GetPeakPosition();
            var p2GraphPos = graph.ValuesToPixelposition(p2AbsPos, jwGraph.jwGraph.Axis.enumAxisLocation.Primary);

            using (Pen p = new Pen(Brushes.Red, 1))
            {
                p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                float top;
                if (Peak1.PeakDirection == CVPeak.enDirection.Positive || Peak2.PeakDirection == CVPeak.enDirection.Positive)
                {
                    top = Math.Min(p1GraphPos.Y, p2GraphPos.Y) - 20;
                }
                else
                {
                    top = Math.Max(p1GraphPos.Y, p2GraphPos.Y) + 20;
                }
                g.DrawLine(p, p1GraphPos.X, p1GraphPos.Y, p1GraphPos.X, top);
                g.DrawLine(p, p2GraphPos.X, p2GraphPos.Y, p2GraphPos.X, top);
                g.DrawLine(p, p1GraphPos.X, top, p2GraphPos.X, top);
            }
        }
    }
}
