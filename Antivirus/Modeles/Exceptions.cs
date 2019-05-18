using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using AntivirusLibrary.Files;

namespace Antivirus.Modeles
{
    [Serializable]
    public class Exceptions
    {
        public Exceptions()
        {
            ExceptionFiles = new List<ExceptionFile>();
        }
        public Exceptions(List<ExceptionFile> exceptionFiles)
        {
            ExceptionFiles = exceptionFiles;
        }
        public List<ExceptionFile> ExceptionFiles { get; set; }
        public static Exceptions LoadExeption(string path)
        {
            Exceptions ReadExceptions = new Exceptions();
            if (File.Exists(path))
            {
                Stream stream = new FileStream(path, FileMode.Open);
                XmlSerializer formatter = new XmlSerializer(typeof(Exceptions));
                try
                {
                    ReadExceptions = (Exceptions)formatter.Deserialize(stream);
                    stream.Close();
                }
                catch (Exception)
                {
                    stream.Close();
                    ReadExceptions.SaveException(path);
                    //stream = new FileStream(path, FileMode.Open);
                    //ReadExceptions = (Exceptions)formatter.Deserialize(stream);
                }
            }
            else
            {
                ReadExceptions.SaveException(path);
                //XmlSerializer formatter = new XmlSerializer(typeof(Exceptions));
                //using (Stream stream = new FileStream(path, FileMode.Open))
                //{
                //    ReadExceptions = (Exceptions)formatter.Deserialize(stream);
                //}
            }
            return ReadExceptions;
        }

        public void AddException(string path,string exception)
        {
            ExceptionFiles.Add(new ExceptionFile(exception));
            SaveException(path);
        }

        public void SaveException(string path)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(Exceptions));
            using (Stream stream = new FileStream(path, FileMode.OpenOrCreate))
            {
                formatter.Serialize(stream, this);
            }
        }
    }
}
