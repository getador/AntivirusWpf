using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntivirusLibrary.Events
{
    public class AddFileInExceptionEventArgs:EventArgs
    {
        public AddFileInExceptionEventArgs(string path)
        {
            Path = path;
        }

        public string Path { get; set; }
    }
}
