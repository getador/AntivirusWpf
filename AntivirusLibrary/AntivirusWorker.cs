using AntivirusLibrary.Abstracts;
using AntivirusLibrary.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntivirusLibrary.Workers;
using AntivirusLibrary.Events;
using System.Diagnostics;
using System.Threading;
using System.Management.Automation;


namespace AntivirusLibrary
{
    /// <summary>
    /// Класс для работы с антивирусом
    /// </summary>
    public class AntivirusWorker
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ExceptionFiles">Список содержащий загруженные исключения</param>
        public AntivirusWorker(List<ExceptionFile> exceptionFiles,int countThread, string signatureString)
        {
            DangerFiles = new List<FileWithSignature>();
            DangerProcess = new List<ProcessDange>();
            ClearProcess = new List<Process>();
            this.ExceptionFiles = exceptionFiles;
            CountThread = countThread;
            Counter = new CounterWorker();
            SignatureString = signatureString;
            SetDefaultSettings();
        }
        /// <summary>
        /// Сканирование файла
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <param name="clearVirusList">True для удаления всех сохраненных вирусов</param>
        public void ScanFile(string path,bool clearVirusList)
        {
            if (clearVirusList)
            {
                ClearVirusList();
            }
            VirusFile fileForCheack = new VirusFile(path);
            bool findSignature = false;
            Counter.SetMaxValue(1, Enams.ResetStatus.Reset);
            if (SignatureM)
                if (fileForCheack.Signature != null && SignatureString != string.Empty)
                {
                    if (SignatureString.Contains(fileForCheack.Signature))
                    {
                        if (AutoDeleteVirus)
                            fileForCheack.DeleteFile();
                        else
                            DangerFiles.Add(fileForCheack);
                        findSignature = true;
                    }
                }
            if (EvrizmM)
                if (!findSignature)
                {
                    string fileSignature = File.ReadAllText(fileForCheack.Path);
                    foreach (var signature in EvrizmSignature.signatures)
                    {
                        if (fileSignature.Contains(signature))
                        {
                            if (AutoDeleteVirus)
                                fileForCheack.DeleteFile();
                            else
                                AddInDangerFile(this, new FindDangerEventArgs(fileForCheack));
                            //DangerFiles.Add(fileForCheack);
                            break;
                        }
                    }
                }
            Counter.Inc();
        }
        /// <summary>
        /// Получить и сохранить в ступенчатый массив файлы для проверки
        /// </summary>
        /// <param name="path">Путь к каталогу для проверки</param>
        /// <param name="searchPattern">Паттерн для поиска</param>
        public void ScanFiles(string path,string searchPattern)
        {
            ClearVirusList();
            List<string> ListWithPath = FindFilesPath(path, searchPattern).ToList();//.Where(x=>!ExceptionFiles.Select(y=>y.Path).Contains(x)).ToList();
            int listLength = ListWithPath.Count;
            //string[][] stringArraySplite = new string[countThread][];
            //for (int i = 0; i < stringArraySplite.Length - 1; i++)
            //{
            //    stringArraySplite[i] = ListWithPath.GetRange(0, Convert.ToInt32(Convert.ToString(listLength / stringArraySplite.Length))).ToArray();
            //    ListWithPath.RemoveRange(0, int.Parse(Convert.ToString(listLength / stringArraySplite.Length)));
            //}
            //stringArraySplite[stringArraySplite.Length - 1] = ListWithPath.GetRange(0, ListWithPath.Count).ToArray();
            //ListWithPath = null;

            //Counter = new CounterWorker((double)listLength);
            Counter.SetMaxValue((double)listLength, Enams.ResetStatus.Reset);
            SignaturesArray = new FileWithSignature[CountThread][];
            for (int i = 0; i < SignaturesArray.Length-1; i++)
            {
                SignaturesArray[i] = ListWithPath.GetRange(0, listLength / SignaturesArray.Length).Select(x => new VirusFile(x)).ToArray();
                ListWithPath.RemoveRange(0, SignaturesArray[i].Length);

                //SignaturesArray[i] = new FileWithSignature[listLength / SignaturesArray.Length];
                //for (int j = 0; j < SignaturesArray[i].Length; j++)
                //{
                //    SignaturesArray[i][j] = new VirusFile(ListWithPath[j]);
                //}
                //ListWithPath.RemoveRange(0, SignaturesArray[i].Length);
            }
            SignaturesArray[SignaturesArray.Length-1] = ListWithPath.GetRange(0, ListWithPath.Count).Select(x => new VirusFile(x)).ToArray();
            SignaturesArray = SignaturesArray.Select(x => x.Where(y => y.Signature != null).ToArray()).Where(z=>z.Length!=0).ToArray();
            Workers = new VirusWorker[SignaturesArray.Length];
            for (int i = 0; i < SignaturesArray.Length; i++)
            {
                Workers[i] = new VirusWorker(SignatureString,EvrizmM,SignatureM,AutoDeleteVirus, SignaturesArray[i]);
                Workers[i].FindDangerEvent += AddInDangerFile;
                Workers[i].FileCheckedEvent += Counter.ChengeElement;
                Workers[i].Start();
            }
        }
        
        public void ScanProcess()
        {
            while (true)
            {
                Process[] processes = Process.GetProcesses().Where(x => !DangerProcess.Select(y => y.Process.ProcessName).ToArray().Contains(x.ProcessName) && !ClearProcess.Select(y => y.ProcessName).Contains(x.ProcessName)).ToArray();

                //ProcessDange[] processWhitchOff = DangerProcess.Where(x => processes.Select(y => y.ProcessName).ToArray().Contains(x.Process.ProcessName)).ToArray();
                ProcessDange[] processWhitchOff = DangerProcess.Where(x => !Process.GetProcesses().Select(y => y.ProcessName).Contains(x.Process.ProcessName)).ToArray();
                if (processWhitchOff.Length != 0)
                {
                    //DangerProcess.RemoveAll(x => !processes.Select(y => y.ProcessName).Contains(x.Process.ProcessName));
                    DangerProcess.RemoveAll(x => processWhitchOff.Select(y=>y.Process.ProcessName).Contains(x.Process.ProcessName));
                    FindDangerProcessEvent?.Invoke(this, new AddDangerProcessEventArgs(false));
                }

                if (processes.Length != 0)
                {
                    foreach (var process in processes)
                    {
                        try
                        {
                            bool notFindInException = true;
                            foreach (var exception in ExceptionFiles)
                            {
                                if (process.MainModule.FileName.Contains(exception.Path))
                                {
                                    notFindInException = false;
                                    DangerProcess.RemoveAll(x => x.Process.ProcessName == process.ProcessName);
                                    FindDangerProcessEvent?.Invoke(this, new AddDangerProcessEventArgs(false));
                                    break;
                                }
                            }

                            //if (DangerProcess.Where(x => x.Path == process.MainModule.FileName).ToArray().Length != 0)
                            //{
                            //    notFindInException = false;
                            //}

                            if (notFindInException && !FileValidater.VerifyAuthenticodeSignature(process.MainModule.FileName))
                            {
                                string fileSignature = File.ReadAllText(process.MainModule.FileName);
                                bool findSignature = false;
                                if (SignatureM)
                                    if (SignatureString.Contains(new ProcessDange(process).Signature))
                                        findSignature = true;
                                if (EvrizmM)
                                    if (!findSignature)
                                        foreach (var signature in EvrizmSignature.signatures)
                                        {
                                            if (fileSignature.Contains(signature))
                                            {
                                                findSignature = true;
                                                break;
                                            }
                                        }

                                if (findSignature)
                                {
                                    //DangerList.Invoke(new Action(() => DangerList.Items.Add(new FileWhichCheked(process.MainModule.FileName))));
                                    //DialogResult dialogResult = MessageBox.Show($"Найдена угроза в процессе {process.ProcessName}.\nНажмите \"Да\" для добавления процесса в иключение \nили нажмите \"Нет\" для его завершения",
                                    //    "Найдена угроза",
                                    //    MessageBoxButtons.YesNo);
                                    //if (dialogResult == DialogResult.Yes)
                                    //{
                                    //    loadedFileException.Add(new FileWhichCheked(process.MainModule.FileName));
                                    //    using (FileStream stream = File.OpenWrite(Directory.GetCurrentDirectory() + "\\ExceptionFile.vih"))
                                    //    {
                                    //        BinaryFormatter formatter = new BinaryFormatter();
                                    //        formatter.Serialize(stream, loadedFileException);
                                    //    }
                                    //}
                                    //else if (dialogResult == DialogResult.No)
                                    //{
                                    //    //process.Kill();
                                    //}
                                    if (CloseProcessTurn)
                                        process.Kill();
                                    else
                                        AddInDangerProcessList(new ProcessDange(process));
                                    if (SoundTurn)
                                        Console.Beep();
                                }
                                else if (signatureM&&evrimM)
                                        ClearProcess.Add(process);
                                    
                            }
                            else if (FileValidater.VerifyAuthenticodeSignature(process.MainModule.FileName))
                            {
                                ClearProcess.Add(process);
                            }
                        }
                        catch (Exception)
                        {
                            ClearProcess.Add(process);
                        }
                    }

                }
                Thread.Sleep(500);
            }

        }

        /// <summary>
        /// Отчистка списка вирусов
        /// </summary>
        public void ClearVirusList()
        {
            DangerFiles = new List<FileWithSignature>();
            FileCheckedEvent?.Invoke(this, new FileCheckEventArgs(true));
        }
        public void AddInDangerProcessList(ProcessDange process)
        {
            DangerProcess.Add((ProcessDange)process.Clone());
            FindDangerProcessEvent?.Invoke(this, new AddDangerProcessEventArgs(true));
        }
        public void KillProcess(Process process)
        {
            process.Kill();
            ClearProcess.RemoveAll(x => x.ProcessName == process.ProcessName);
            FindDangerProcessEvent?.Invoke(this, new AddDangerProcessEventArgs(false));
        }
        public void KillProcess(ProcessDange process)
        {
            string processName = process.Process.ProcessName;
            process.KillProcess();
            DangerProcess.RemoveAll(x => x.Process == null);
            ClearProcess.RemoveAll(x => x.ProcessName == processName);
            FindDangerProcessEvent?.Invoke(this, new AddDangerProcessEventArgs(false));
        }
        public void AddInDangerFile(object sender, FindDangerEventArgs e)
        {
            DangerFiles.Add(e.DangerFile);
            FileCheckedEvent?.Invoke(this, new FileCheckEventArgs(true));
        }

        public void AddInException(object sender, ExceptionAddEventArgs e)
        {
            //ExceptionFiles.Add((ExceptionFile)e.FileWithException.Clone());
            DangerFiles.RemoveAll(x => x.Path == e.FileWithException.Path);
            FileCheckedEvent?.Invoke(this, new FileCheckEventArgs(false));
        }
        #region Settings
            #region ProcessSettings
            public bool SoundTurn { get; set; }
            public bool CloseProcessTurn { get; set; }
            public bool AutoDeleteVirus { get; set; }
            private bool evrimM;
            public bool EvrizmM
            {
                get
                {
                    return evrimM;
                }
                set
                {
                    evrimM = value;
                    DangerProcess = new List<ProcessDange>();
                    FindDangerProcessEvent?.Invoke(this, new AddDangerProcessEventArgs(false));
                }
            }
            private bool signatureM;
            public bool SignatureM
            {
                get
                {
                    return signatureM;
                }
                set
                {
                    signatureM = value;
                    DangerProcess = new List<ProcessDange>();
                    FindDangerProcessEvent?.Invoke(this, new AddDangerProcessEventArgs(false)); 
                }
            }
        #endregion
        private void SetDefaultSettings(bool soundTurn = true, bool closeProcessTurn = false, bool evrizmM = true, bool signatureM = true, bool autoDeleteVirus = false)
        {
            SoundTurn = soundTurn;
            CloseProcessTurn = closeProcessTurn;
            EvrizmM = evrizmM;
            SignatureM = signatureM;
            AutoDeleteVirus = autoDeleteVirus;
        }
        #endregion
        #region Events
        public event EventHandler<FileCheckEventArgs> FileCheckedEvent;
        public event EventHandler<AddDangerProcessEventArgs> FindDangerProcessEvent; 
        #endregion
        public CounterWorker Counter { get; set; }
        public List<ExceptionFile> ExceptionFiles { get; set; }
        public List<FileWithSignature> DangerFiles { get; set; }
        public List<ProcessDange> DangerProcess { get; set; }
        public List<Process> ClearProcess { get; set; } 
        public FileWithSignature[][] SignaturesArray { get; set; }
        public string SignatureString { get; set; }
        public VirusWorker[] Workers { get; set; }
        public int CountThread { get; set; }
        /// <summary>
        /// Получение файлов из каталога и подкаталогов
        /// </summary>
        /// <param name="path">Путь</param>
        /// <param name="searchPattern">Паттерн</param>
        /// <returns></returns>
        private IEnumerable<string> FindFilesPath(string path, string searchPattern)
        {
            string[] files;
            try
            {
                files = Directory.GetFiles(path, searchPattern);
            }
            catch (UnauthorizedAccessException)
            {
                yield break;
            }

            foreach (string file in files)
            {
                bool haveInException = false;
                foreach (ExceptionFile exception in ExceptionFiles)
                {
                    if (file.Contains(exception.Path))
                    {
                        haveInException = true;
                    }
                }
                if (!haveInException)
                {
                    yield return file;
                }
            }

            string[] directories;
            try
            {
                directories = Directory.GetDirectories(path);
            }
            catch (UnauthorizedAccessException)
            {
                yield break;
            }

            foreach (string subdirectory in directories)
            {
                foreach (string file in FindFilesPath(subdirectory, searchPattern))
                {
                    bool haveInException = false;
                    foreach (ExceptionFile exception in ExceptionFiles)
                    {
                        if (file.Contains(exception.Path))
                        {
                            haveInException = true;
                        }
                    }
                    if (!haveInException)
                    {
                        yield return file;
                    }
                }
            }
        }
    }
}
