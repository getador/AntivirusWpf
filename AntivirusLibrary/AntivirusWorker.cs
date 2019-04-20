using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntivirusLibrary.Abstracts;
using AntivirusLibrary.Files;

namespace AntivirusLibrary
{
    public class AntivirusWorker
    {
        public AntivirusWorker(List<FileWithSignature> dangerFiles, List<ProcessDange> dangerProcess)
        {
            DangerFiles = dangerFiles;
            DangerProcess = dangerProcess;
        }

        internal List<FileWithSignature> DangerFiles { get; set; }
        internal List<ProcessDange> DangerProcess { get; set; }
    }
}
