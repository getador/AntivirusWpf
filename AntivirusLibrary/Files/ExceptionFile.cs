using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntivirusLibrary.Abstracts;

namespace AntivirusLibrary.Files
{
    class ExceptionFile:FileWithSignature
    {
        public ExceptionFile(string path):base(path)
        {

        }

        ~ExceptionFile()
        {

        }
    }
}
