using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Antivirus.Modeles
{
    class SettingsWorker
    {
        public List<string> SaveFileName { get; set; }
        public List<string> SaveFilePath { get; set; }

        public Settings SettingsLoaded { get; set; }
        public Language UsedLanguage { get; set; }
        public List<Language> LoadedLanguage { get; set; }

        public SettingsWorker()
        {
            ReadLangFile();
            SettingsLoaded = new Settings(Environment.CurrentDirectory + @"\config.cfg");          
            Language.SaveLanguageExample();
            for (int i = 0; i < LoadedLanguage.Count; i++)
            {
                if (LoadedLanguage[i].LanguageName == SettingsLoaded.InterfaceLanguage)
                {
                    UsedLanguage = LoadedLanguage[i];
                    break;
                }
            }            
        }

        public void ChangeLanguage()
        {
            foreach (var Language in LoadedLanguage)
            {
                if (Language.LanguageName == SettingsLoaded.InterfaceLanguage)
                {
                    UsedLanguage = Language;
                    SettingsLoaded.SaveSettings(Environment.CurrentDirectory+ @"\config.cfg");
                    break;
                }
            }
        }

        public string GetPathLanguageFile(string Name)
        {
            for (int i = 0; i < SaveFilePath.Count; i++)
            {
                if (SaveFileName[i] == Name)
                {
                    return SaveFilePath[i];
                }
            }
            return null;
        }

        public string this[int index]
        {
            get { return SaveFilePath[index]; }
        }

        public void ReadLangFile()
        {
            LoadedLanguage = new List<Language>();
            SaveFilePath = new List<string>();
            SaveFileName = new List<string>();
            if (Directory.GetDirectories(Environment.CurrentDirectory).Contains(Environment.CurrentDirectory+@"\Language"))
            {
                SaveFilePath = Directory.GetFiles(Environment.CurrentDirectory + @"\Language", "*.lang").ToList();
                if (SaveFilePath.Count > 0)
                {
                    for (int i = 0; i < SaveFilePath.Count; i++)
                    {
                        LoadedLanguage.Add(Language.LoadLanguageFile(SaveFilePath[i]));
                        SaveFileName.Add(GetFileName(SaveFilePath[i]));
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + @"\Language");
            }
            if (LoadedLanguage.Where(x=>x!=null && x.LanguageName=="En").ToArray().Length==0)
            {
                Language.SaveLanguageEn();
                ReadLangFile();
            }
            if (!SaveFileName.Contains("Ru"))
            {
                Language.SaveLanguageRu();
                ReadLangFile();
            }

        }

        private string GetFileName(string path)
        {
            string regexString = @"([^\\]+)([.]+\w+$)";
            Regex reg = new Regex(regexString);
            MatchCollection me = reg.Matches(path);
            return me[0].Groups[1].Value;
        }
    }
}
