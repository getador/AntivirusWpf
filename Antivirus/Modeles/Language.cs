using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Antivirus.Modeles
{
    [Serializable]
    public class Language
    {
        public string LanguageName { get; set; }
        public string SettingsLanguageText { get; set; }

        public Language()
        {

        }

        public Language(string LanguageName)
        {
            this.LanguageName = LanguageName;
        }

        public static Language LoadLanguageFile(string path)
        {

            try
            {
                XmlSerializer formatter = new XmlSerializer(typeof(Language));
                using (Stream stream = new FileStream(path, FileMode.OpenOrCreate))
                return (Language)formatter.Deserialize(stream);

            }
            catch (Exception)
            {
                return null;
            }
      

            //Language ReadLanguage;
            //XmlSerializer formatter = new XmlSerializer(typeof(Language));
            //Stream stream = new FileStream(path, FileMode.OpenOrCreate);
            //    try
            //    {
            //        ReadLanguage = (Language)formatter.Deserialize(stream);
            //        SettingsLanguageText = ReadLanguage.SettingsLanguageText;
            //    }
            //    catch (InvalidOperationException)
            //    {
            //        stream.Close();
            //        SaveLanguageEn();
            //        LoadLanguageFile(Environment.CurrentDirectory + @"\Language\" + "En.lang");
            //    }
            //stream.Close();
        }

        public static void SaveLanguageEn()
        {
            Language ExampleLanguage = new Language("En");
            ExampleLanguage.SettingsLanguageText = "Language";
            XmlSerializer formatter = new XmlSerializer(typeof(Language));
            using (Stream stream = new FileStream(Environment.CurrentDirectory + @"\Language\"+"En.lang", FileMode.OpenOrCreate))
            {
                formatter.Serialize(stream, ExampleLanguage);
            }
        }

        public static void SaveLanguageRu()
        {
            Language ExampleLanguage = new Language("Ru");
            ExampleLanguage.SettingsLanguageText = "Язык";
            XmlSerializer formatter = new XmlSerializer(typeof(Language));
            using (Stream stream = new FileStream(Environment.CurrentDirectory + @"\Language\" + "Ru.lang", FileMode.OpenOrCreate))
            {
                formatter.Serialize(stream, ExampleLanguage);
            }
        }

        public static void SaveLanguageExample()
        {
            Language ExampleLanguage = new Language("Language Name");
            ExampleLanguage.SettingsLanguageText = "Language label text";
            XmlSerializer formatter = new XmlSerializer(typeof(Language));
            using (Stream stream = new FileStream(Environment.CurrentDirectory+@"\Lenguage settings example.txt", FileMode.OpenOrCreate))
            {
                formatter.Serialize(stream, ExampleLanguage);
            }
        }
    }
}
