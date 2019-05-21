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
        public string DangerFileButtonText { get; set; }
        public string ExceptionFileButtonText { get; set; }
        public string DangerProcessButtonText { get; set; }
        public string StopButtonContext { get; set; }
        public string DeleteButtonContext { get; set; }
        public string AddInExceptionButtonContext { get; set; }
        public string KillProcessButtonContext { get; set; }
        public string ScanCotalogButtonContext { get; set; }
        public string ScanFileButtonContext { get; set; }
        public string AddCotalogInExceptionButtonContext { get; set; }
        public string AddFileInExceptionButtonContext { get; set; }
        public string DeleteAllButtonContext { get; set; }
        public string TitleOfMessage { get; set; }
        public string MessageInMes { get; set; }
        public string FirstButtonContext { get; set; }
        public string SecondButtonContext { get; set; }
        public string ScanSettingsText { get; set; }
        public string AutoDeleteVirusFileSetttingsText { get; set; }
        public string SignatureSettingsText { get; set; }
        public string EvrizmSettingsText { get; set; }
        public string CountThreadSettingsText { get; set; }
        public string SoundSettingsText { get; set; }
        public string SendVirusContent { get; set; }
        public Language()
        {

        }

        public Language(string LanguageName)
        {
            this.LanguageName = LanguageName;
        }

        public Language(string languageName,
                        string settingsLanguageText,
                        string dangerFileButtonText,
                        string exceptionFileButtonText,
                        string dangerProcessButtonText,
                        string stopButtonContext,
                        string deleteButtonContext,
                        string addInExceptionButtonContext,
                        string killProcessButtonContext,
                        string scanCotalogButtonContext,
                        string scanFileButtonContext,
                        string addCotalogInExceptionButtonContext,
                        string addFileInExceptionButtonContext,
                        string deleteAllButtonContext,
                        string titleOfMessage,
                        string messageInMes,
                        string firstButtonContext,
                        string secondButtonContext,
                        string scanSettingsText,
                        string autoDeleteVirusFileSetttingsText,
                        string signatureSettingsText,
                        string evrizmSettingsText,
                        string countThreadSettingsText,
                        string soundSettingsText, 
                        string sendVirusContent)
        {
            LanguageName = languageName;
            SettingsLanguageText = settingsLanguageText;
            DangerFileButtonText = dangerFileButtonText;
            ExceptionFileButtonText = exceptionFileButtonText;
            DangerProcessButtonText = dangerProcessButtonText;
            StopButtonContext = stopButtonContext;
            DeleteButtonContext = deleteButtonContext;
            AddInExceptionButtonContext = addInExceptionButtonContext;
            KillProcessButtonContext = killProcessButtonContext;
            ScanCotalogButtonContext = scanCotalogButtonContext;
            ScanFileButtonContext = scanFileButtonContext;
            AddCotalogInExceptionButtonContext = addCotalogInExceptionButtonContext;
            AddFileInExceptionButtonContext = addFileInExceptionButtonContext;
            DeleteAllButtonContext = deleteAllButtonContext;
            TitleOfMessage = titleOfMessage;
            MessageInMes = messageInMes;
            FirstButtonContext = firstButtonContext;
            SecondButtonContext = secondButtonContext;
            ScanSettingsText = scanSettingsText;
            AutoDeleteVirusFileSetttingsText = autoDeleteVirusFileSetttingsText;
            SignatureSettingsText = signatureSettingsText;
            EvrizmSettingsText = evrizmSettingsText;
            CountThreadSettingsText = countThreadSettingsText;
            SoundSettingsText = soundSettingsText;
            SendVirusContent = sendVirusContent;
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
            Language ExampleLanguage = new Language("En","Language","Virus","Exceptions","Process","Stop","Delete","Add in exception", "Kill process","Scan catalog","Scan file","Add catalog", "Add file","Delete all","Cansel scaning","Cansel scaning?","Yes","No","File scaning","Auto delete file","Signature method","Evrizm method","Count of thread for scaning","Sound","Send virus");
            XmlSerializer formatter = new XmlSerializer(typeof(Language));
            using (Stream stream = new FileStream(Environment.CurrentDirectory + @"\Language\"+"En.lang", FileMode.OpenOrCreate))
            {
                formatter.Serialize(stream, ExampleLanguage);
            }
        }

        public static void SaveLanguageRu()
        {
            Language ExampleLanguage = new Language("Ru","Язык","Вирусы","Исключения","Процессы","Остановить","Удалить","Добавить в исключение","Завершить","Проверка каталога","Проверка файла","Добавить каталог", "Добавить файл","Удалить все", "Отмена сканирования", "Отменить сканирование?","Да","Нет", "Сканирование файлов", "Авто удаление файл", "Сигнатурная методика", "Эвристическая методика", "Количество потоков на выполнение", "Воспроизведение звука","Отправить вирус");
            XmlSerializer formatter = new XmlSerializer(typeof(Language));
            using (Stream stream = new FileStream(Environment.CurrentDirectory + @"\Language\" + "Ru.lang", FileMode.OpenOrCreate))
            {
                formatter.Serialize(stream, ExampleLanguage);
            }
        }

        public static void SaveLanguageExample()
        {
            Language ExampleLanguage = new Language("Language Name","Language label text","Context for button whitch open virus page","Context for button whitch open exception page"
                ,"Context for button whitch open danger process","Stop scaning button","Delete element on list","Add element on list", "Kill element on list","Context for scan catalog button",
                "Context for scan file button","Context for add cotalog in exception","Context for add file in exception","Delete all element in list","Title for messageBox",
                "MessageBox message","MessageBox first button context","MessageBox second button context","Text for scan block settings","Text for auto delete settings","Text for signature serch settings",
                "Text for evrizm serch settings","Text for count of thread settings","Text for sound settings","Context for button whitch send virus on server");
            XmlSerializer formatter = new XmlSerializer(typeof(Language));
            using (Stream stream = new FileStream(Environment.CurrentDirectory+@"\Lenguage settings example.txt", FileMode.OpenOrCreate))
            {
                formatter.Serialize(stream, ExampleLanguage);
            }
        }
    }
}
