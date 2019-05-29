using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Xml.Serialization;

namespace Antivirus.Modeles
{
    [Serializable]
    public class Settings
    {
        private string interfaceLanguage;
        public string InterfaceLanguage {
            get
            {
                return interfaceLanguage;
            }
            set
            {
                interfaceLanguage = value;
            }
        }



        public bool AutoVirusDelete { get;set; }
        public bool SignatureM { get; set; }
        public bool EvrizmM { get; set; }
        public int CountOfThread { get; set; }
        public bool Sound { get; set; }

        public Settings()
        {

        }

        public Settings(string path)
        {         
            if (File.Exists(path))
            {
                Settings ReadSettingCFG = LoadSettings(path);
                interfaceLanguage = ReadSettingCFG.InterfaceLanguage;
                AutoVirusDelete = ReadSettingCFG.AutoVirusDelete;
                SignatureM = ReadSettingCFG.SignatureM;
                EvrizmM = ReadSettingCFG.EvrizmM;
                CountOfThread = ReadSettingCFG.CountOfThread;
                Sound = ReadSettingCFG.Sound;
            }
            else
                CreateNewSettings();
        }

        public static Settings LoadSettings(string path)
        {
            Stream stream = new FileStream(path, FileMode.Open);
            XmlSerializer formatter = new XmlSerializer(typeof(Settings));
            Settings ReadSettingsCFG = new Settings();
            try
            {
                ReadSettingsCFG = (Settings)formatter.Deserialize(stream);
                stream.Close();
            }
            catch (Exception)
            {
                stream.Close();
                ReadSettingsCFG.CreateNewSettings();
                ReadSettingsCFG.SaveSettings(path);
            }
            return ReadSettingsCFG;
        }


        public void SaveSettings(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
            XmlSerializer formatter = new XmlSerializer(typeof(Settings));
            using (Stream stream = new FileStream(path, FileMode.OpenOrCreate))
            {
                formatter.Serialize(stream, this);
            }
        }

        private void CreateNewSettings()
        {
            InterfaceLanguage = "En";
            AutoVirusDelete = false;
            SignatureM = true;
            EvrizmM = true;
            CountOfThread = 1;
            Sound = true;
        }
    }
}
