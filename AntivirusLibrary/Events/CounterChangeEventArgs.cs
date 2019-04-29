using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntivirusLibrary.Events
{
    public class CounterChangeEventArgs:EventArgs
    {
        public double Count { get; set; }
        public CounterChangeEventArgs(double count)
        {
            Count = count;
        }
    }
}
