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
    public class ProcessDange:ICloneable
    {
        public Process Process { get; set; }
        public string Path { get; set; }
        public string Signature { get; set; }
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
        public override string ToString()
        {
            return Process.ProcessName;
        }
        public ProcessDange(Process process, string path, string signature)
        {
            Process = process;
            Path = path;
            Signature = signature;
        }

        ~ProcessDange()
        {

        }

        public void KillProcess()
        {
            Process.Kill();
        }

        public object Clone()
        {
            return new ProcessDange(Process, Path, Signature);
        }
    }
}
