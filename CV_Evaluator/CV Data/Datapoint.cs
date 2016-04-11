using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace CV_Evaluator 
{
    [Serializable]
    public class Datapoint : INotifyPropertyChanged
    {
        public Datapoint() : this(null)
        {
        }
        public Datapoint(Cycle Parent)
        {
            this.Parent = Parent;
        }
        public Cycle Parent;
        public double Volt { get; set; }
        public double Current { get; set; }
        public double Time { get; set; }

        private void Notify(string PropName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(PropName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
