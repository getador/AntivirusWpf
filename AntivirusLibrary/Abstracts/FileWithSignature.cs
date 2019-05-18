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
    [Serializable]
    public abstract class FileWithSignature : IDisposable,ICloneable
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
                        Signature = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-","");
                        Path = path;
                    }
                }
            }
            else
            {
                Signature = null;
            }
        }

        public FileWithSignature(string path, string signature)
        {
            Path = path;
            Signature = signature;
        }

        ~FileWithSignature()
        {

        }

        public override string ToString()
        {
            return $"{Path}";
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
            Signature = null;
        }

        public abstract object Clone();

        public string Path { get; set; }
        public string Signature { get; set; }
    }
}
