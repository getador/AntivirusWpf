using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntivirusLibrary.Events
{
    public class CounterMaxValueChangeEventArgs:EventArgs
    {
        public double MaxValue { get; set; }

        public CounterMaxValueChangeEventArgs(double maxValue)
        {
            MaxValue = maxValue;
        }
    }
}
