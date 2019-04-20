using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntivirusLibrary.Abstracts;

namespace AntivirusLibrary.Files
{
    public class ExceptionFile
    {
        public ExceptionFile(string Path)
        {
            this.Path = Path;
        }
        public string Path { get; set; }
        ~ExceptionFile()
        {

        }
    }
}
