using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AntivirusLibrary
{
    public class ProcessDange
    {
        Process Process { get; set; }
        string Path { get; set; }
        string Signature { get; set; }
        public ProcessDange(Process Process)
        {
            this.Process = Process;           
            Path = Process.MainModule.FileName;
            using (MD5 md5 = MD5.Create())
            {
                using (Stream stream = File.OpenRead(Path))
                {
                    Signature = BitConverter.ToString(md5.ComputeHash(stream));
                }
            }          
        }

        ~ProcessDange()
        {

        }

        public void KillProcess()
        {
            Process.Kill();
            Path = null;
            Signature = null;
            Process = null;
        }
    }
}
