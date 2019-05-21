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
        private double element;
        public double Element
        {
            get
            {
                return element;
            }
            set
            {
                element = value;
                CounterChangeEvent?.Invoke(this, new CounterChangeEventArgs(element));
            }
        }
        private double maxValue;
        public double MaxValue
        {
            get { return maxValue; }
            set
            {
                maxValue = value;
                MaxValueChangeEvent?.Invoke(this, new CounterMaxValueChangeEventArgs(maxValue));
            }
        }
        public event EventHandler<CounterChangeEventArgs> CounterChangeEvent;
        public event EventHandler<CounterMaxValueChangeEventArgs> MaxValueChangeEvent;
        public CounterWorker()
        {
            Element = 0;
            MaxValue = 1;
        }
        public CounterWorker(double maxCount)
        {
            Element = 0;
            MaxValue = maxCount;
        }
        public void Reset()
        {
            Element = 0;
            MaxValue = 1;
        }
        public void SetMaxValue(double maxValue, Enams.ResetStatus resetStatus)
        {
            MaxValue = maxValue;
            if (resetStatus == Enams.ResetStatus.Reset)
                Element = 0;
        }
        public void Inc()
        {
            Element=element+1;
        }
        public void Dec()
        {
            Element=element-1;
        }
        public void ChengeElement(object sender, FileCheckEventArgs e)
        {
            if (element>670)
            {
                int a = 5;
            }
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
