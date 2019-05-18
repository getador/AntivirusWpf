using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntivirusLibrary.Abstracts;

namespace AntivirusLibrary.Files
{
    public class VirusFile:FileWithSignature
    {
        public VirusFile(string path):base(path)
        {

        }
        public VirusFile(string path, string signature):base(path,signature)
        {

        }
        ~VirusFile()
        {
            
        }

        public override object Clone()
        {
            return new VirusFile(Path, Signature);
        }
    }
}
