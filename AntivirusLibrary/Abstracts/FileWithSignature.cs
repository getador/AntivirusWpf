using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AntivirusLibrary.Abstracts
{
    /// <summary>
    /// Абстрактный класс файла с путем к файлу и сигнатурой
    /// </summary>
    public abstract class FileWithSignature : IDisposable
    {
        /// <summary>
        /// Конструктор 
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        public FileWithSignature(string path)
        {
            if (File.Exists(path))
            {
                using (MD5 md5 = MD5.Create())
                {
                    using (Stream stream = File.OpenRead(path))
                    {
                        Singature = BitConverter.ToString(md5.ComputeHash(stream));
                        Path = path;
                    }
                }
            }
        }

        ~FileWithSignature()
        {

        }

        public override string ToString()
        {
            return $"{Path} {Singature}";
        }
        /// <summary>
        /// Удаление файла
        /// </summary>
        public void DeleteFile()
        {
            if (File.Exists(Path))
            {
                File.Delete(Path);
            }
            Dispose();
        }
        public void Dispose()
        {
            Path = null;
            Singature = null;
        }

        public string Path { get; set; }
        public string Singature { get; set; }
    }
}
