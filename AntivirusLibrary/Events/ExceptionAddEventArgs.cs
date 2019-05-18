using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntivirusLibrary.Files;

namespace AntivirusLibrary.Events
{
    public class ExceptionAddEventArgs:EventArgs
    {
        public ExceptionAddEventArgs(ExceptionFile fileWithException)
        {
            FileWithException = fileWithException;
        }

        public Files.ExceptionFile FileWithException { get; set; }
    }
}
