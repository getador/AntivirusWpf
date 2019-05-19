using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntivirusLibrary.Abstracts;

namespace AntivirusLibrary.Files
{
    [Serializable]
    public class ExceptionFile:ICloneable
    {
        public ExceptionFile()
        {

        }
        public ExceptionFile(string Path)
        {
            this.Path = Path;
        }
        public override string ToString()
        {
            return Path; 
        }
        public string Path { get; set; }
        ~ExceptionFile()
        {

        }

        public object Clone()
        {
            return new ExceptionFile(Path);
        }
    }
}
