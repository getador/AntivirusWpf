using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntivirusLibrary.Events
{
    public class AddDangerProcessEventArgs:EventArgs
    {
        public AddDangerProcessEventArgs(bool status)
        {
            Status = status;
        }

        public bool Status { get; set; }
    }
}
