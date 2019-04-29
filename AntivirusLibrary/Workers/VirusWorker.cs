using AntivirusLibrary.Abstracts;
using AntivirusLibrary.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AntivirusLibrary.Events;

namespace AntivirusLibrary.Workers
{
    /// <summary>
    /// Класс для сканирования вирусов
    /// </summary>
    public class VirusWorker
    {
        public VirusWorker(string signatureString, params FileWithSignature[] filesArray)
        {
            SignatureString = signatureString;
            VirusList = new List<VirusFile>();
            FilesArray = filesArray;
            checkFileThread = new Task(() => CheckFiles());
        }
        public void StopScan()
        {
            checkFileThread.Dispose();
        }
        public void Start()
        {
            checkFileThread.Start();
        }
        public event EventHandler<FindDangerEventArgs> FindDangerEvent;
        public event EventHandler<FileCheckEventArgs> FileChecked;
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
                    if (SignatureString.Contains(FilesArray[i].Signature))
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
                                FindDangerEvent?.Invoke(this, new FindDangerEventArgs(FilesArray[i]));
                                FileChecked?.Invoke(this, new FileCheckEventArgs(true));
                                VirusList.Add((VirusFile)FilesArray[i]);
                                break;
                            }
                        }
                    }
                }
            }
        }
        private Task checkFileThread;
    }
}
