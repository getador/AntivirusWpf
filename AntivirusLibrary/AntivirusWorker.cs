using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntivirusLibrary.Abstracts;
using AntivirusLibrary.Files;

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
        public AntivirusWorker(List<ExceptionFile> ExceptionFiles)
        {
            DangerFiles = new List<FileWithSignature>();
            DangerProcess = new List<ProcessDange>();
            this.ExceptionFiles = ExceptionFiles;
        }
        public List<ExceptionFile> ExceptionFiles { get; set; }
        internal List<FileWithSignature> DangerFiles { get; set; }
        internal List<ProcessDange> DangerProcess { get; set; }

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
