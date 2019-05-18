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
        public AntivirusWorker(List<ExceptionFile> ExceptionFiles,int CountThread, string signatureString)
        {
            DangerFiles = new List<FileWithSignature>();
            DangerProcess = new List<ProcessDange>();
            this.ExceptionFiles = ExceptionFiles;
            countThread = CountThread;
            Counter = new CounterWorker();
            SignatureString = signatureString;
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
            if (fileForCheack.Signature != null && SignatureString!=string.Empty)
            {
                if (SignatureString.Contains(fileForCheack.Signature))
                {
                    DangerFiles.Add(fileForCheack);
                    findSignature = true;
                }
            }
            if (!findSignature)
            {
                string fileSignature = File.ReadAllText(fileForCheack.Path);
                foreach (var signature in EvrizmSignature.signatures)
                {
                    if (fileSignature.Contains(signature))
                    {
                        AddInDangerFile(this,new FindDangerEventArgs(fileForCheack));
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
            SignaturesArray = new FileWithSignature[countThread][];
            for (int i = 0; i < SignaturesArray.Length; i++)
            {
                SignaturesArray[i] = new FileWithSignature[listLength/ SignaturesArray.Length];
                for (int j = 0; j < SignaturesArray[i].Length; j++)
                {
                    SignaturesArray[i][j] = new VirusFile(ListWithPath[j]);
                }
                ListWithPath.RemoveRange(0,SignaturesArray[i].Length);
            }
            SignaturesArray = SignaturesArray.Select(x => x.Where(y => y.Signature != null).ToArray()).ToArray();
            Workers = new VirusWorker[SignaturesArray.Length];
            for (int i = 0; i < SignaturesArray.Length; i++)
            {
                Workers[i] = new VirusWorker(SignatureString, SignaturesArray[i]);
                Workers[i].FindDangerEvent += AddInDangerFile;
                Workers[i].FileCheckedEvent += Counter.ChengeElement;
                Workers[i].Start();
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
        public void AddInDangerFile(object sender, FindDangerEventArgs e)
        {
            DangerFiles.Add(e.DangerFile);
            FileCheckedEvent?.Invoke(this, new FileCheckEventArgs(true));
        }

        public void AddInException(object sender, ExceptionAddEventArgs e)
        {
            ExceptionFiles.Add((ExceptionFile)e.FileWithException.Clone());
            DangerFiles.RemoveAll(x => x.Path == e.FileWithException.Path);
            FileCheckedEvent?.Invoke(this, new FileCheckEventArgs(false));
        }
        #region Events
        public event EventHandler<FileCheckEventArgs> FileCheckedEvent;
        #endregion
        public CounterWorker Counter { get; set; }
        public List<ExceptionFile> ExceptionFiles { get; set; }
        public List<FileWithSignature> DangerFiles { get; set; }
        public List<ProcessDange> DangerProcess { get; set; }
        public FileWithSignature[][] SignaturesArray { get; set; }
        public string SignatureString { get; set; }
        private VirusWorker[] Workers;
        private int countThread;
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
