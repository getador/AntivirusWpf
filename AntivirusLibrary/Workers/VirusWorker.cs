using AntivirusLibrary.Abstracts;
using AntivirusLibrary.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AntivirusLibrary.Workers
{
    /// <summary>
    /// Класс для сканирования вирусов
    /// </summary>
    public class VirusWorker
    {
        public VirusWorker(string signatureString,params FileWithSignature[] filesArray)
        {
            SignatureString = signatureString;
            VirusList = new List<VirusFile>();
            FilesArray = filesArray;
            CheckFileThread = new Task(() => CheckFiles());
            CheckFileThread.Start();
        }
        public void StopScan()
        {
            CheckFileThread.Dispose();
        }
        public FileWithSignature[] FilesArray { get; set; }
        public List<VirusFile> VirusList { get; set; }
        public string SignatureString { get; set; }
        private void CheckFiles()
        {
            for (int i = 0; i < FilesArray.Length; i++)
            {
                bool findSignature = false;
                if (File.Exists(FilesArray[i].Path))
                {
                    if (SignatureString.Contains(FilesArray[i].Singature))
                    {
                        VirusList.Add((VirusFile)FilesArray[i]);
                        findSignature = true;
                    }
                    if (!findSignature)
                    {
                        string fileSignature = File.ReadAllText(FilesArray[i].Path);
                        foreach (var signature in EvrizmSignature.signatures)
                        {
                            if (fileSignature.Contains(signature))
                            {
                                VirusList.Add((VirusFile)FilesArray[i]);
                                break;
                            }
                        }
                    }
                }
            }
        }
        private Task CheckFileThread;
    }
}
