using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntivirusLibrary.Events;

namespace AntivirusLibrary.Workers
{
    public class CounterWorker
    {
        public double Element { get; set; }
        public CounterWorker()
        {
            Element = 0;
        }
        public void Reset()
        {
            Element = 0;
        }
        public void Inc()
        {
            Element++;
        }
        public void Dec()
        {
            Element--;
        }
        public void ChengeElement(object sender, FileCheckEventArgs e)
        {
            if (e.State)
                Inc();
            else
                Dec();
        }
        public override string ToString()
        {
            return Convert.ToString(Element);
        }
    }
}
