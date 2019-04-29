using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntivirusLibrary.Abstracts;

namespace AntivirusLibrary.Events
{
    public class FindDangerEventArgs:EventArgs
    {
        public FindDangerEventArgs(FileWithSignature dangerFile)
        {
            DangerFile = dangerFile;
        }

        public FileWithSignature DangerFile { get; }
    }
}
