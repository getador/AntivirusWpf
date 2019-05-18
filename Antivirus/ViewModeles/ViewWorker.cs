using Antivirus.Modeles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Antivirus.ViewModeles
{
    internal class ViewWorker: INotifyPropertyChanged
    {
        private List<Page> Pages;
        private SettingsWorker worker;
        private AntivirusLibrary.AntivirusWorker antivirusWorker;
        public ViewWorker()
        {
            virusList = new List<AntivirusLibrary.Abstracts.FileWithSignature>();
            worker = new SettingsWorker();
            Pages = new List<Page>();
            CreteAntiwirusWorker();
            CreateVirusPage();
            //VirusList.Add(new AntivirusLibrary.Files.VirusFile(@"C:\Users\Слава\Desktop\Новый текстовый документ (2).txt"));
            //VirusList.Add(new AntivirusLibrary.Files.VirusFile(@"C:\Users\Слава\Desktop\Новый текстовый документ (2).txt"));
            //SettingsLanguageText = worker.UsedLanguage.SettingsLanguageText;
            ChangeInterfaceLanguage();
            indexLangItem = -1;
            ExceptionFiles = worker.ExceptionsWork.ExceptionFiles;
            for (int i = 0; i < ItemsFile.Count; i++)
            {
                if (ItemsFile[i] == worker.SettingsLoaded.InterfaceLanguage)
                {
                    IndexLangItem = i;
                    break;
                }
            }
            //ItemsFile = worker.SaveFileName;
            //IndexLangItem = -1;

        }

        private void CreteAntiwirusWorker()
        {
            antivirusWorker = new AntivirusLibrary.AntivirusWorker(worker.ExceptionsWork.ExceptionFiles, 1, GetSignatureFile(Environment.CurrentDirectory + @"\hashlid.hash"));
            antivirusWorker.FileCheckedEvent += GetUpdateFile;
            antivirusWorker.Counter.CounterChangeEvent += GetCount;
            antivirusWorker.Counter.MaxValueChangeEvent += GetMaxValue;
            antivirusWorker.Counter.Reset();
        }

        private string GetSignatureFile(string path)
        {
            if (File.Exists(path))
            {
                using (StreamReader stream = new StreamReader(path))
                {
                    return stream.ReadToEnd().Replace("\r\n", "||");
                }
            }
            else
            {
                using (Stream stream = new FileStream(path,FileMode.Create))
                {
                    return string.Empty;
                }
            }
        }

        public void ChangeInterfaceLanguage()
        {
            SettingsLanguageText = worker.UsedLanguage.SettingsLanguageText;
        }

        #region CreatePages
        private void CreateVirusPage()
        {
            if (Pages.Where(x => x.Name == "VirusPageW").ToArray().Length == 0)
            {
                Pages.Add(new Pages.VirusPage());
                ((Pages.VirusPage)Pages[Pages.Count - 1]).AddFileInExceptionEvent += GetUpdateException;
                SetDataContext();
            }
        }

        private void SetDataContext()
        {
            foreach (Page page in Pages)
            {
                page.DataContext = this;
            }
        }
        #endregion

        #region Language settings

        ////public string SettingsLanguageText
        ////{
        ////    get { return worker.UsedLanguage.SettingsLanguageText; }
        ////}


        private string settingsLanguageText;
        public string SettingsLanguageText
        {
            get { return settingsLanguageText; }
            set
            {
                if (value == settingsLanguageText)
                    return;
                settingsLanguageText = value;
                OnPropertyChanged("SettingsLanguageText");
            }
        }

        #endregion

        #region Language index
        public List<string> ItemsFilePath
        {
            get { return worker.SaveFilePath; }
        }
        public List<string> ItemsFile
        {
            get { return worker.SaveFileName; }
        }

        private int indexLangItem;

        

        public int IndexLangItem
        {
            get { return indexLangItem; }
            set
            {
                indexLangItem = value;
                if (indexLangItem >= 0)
                {
                    worker.SettingsLoaded.InterfaceLanguage = ItemsFile[indexLangItem];
                    worker.ChangeLanguage();
                    ChangeInterfaceLanguage();
                }
            }
        }

        //public string[] ItemsFile
        //{
        //    get { return (string[])GetValue(ItemsFileProperty); }
        //    set { SetValue(ItemsFileProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for ItemsFile.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty ItemsFileProperty =
        //    DependencyProperty.Register("ItemsFile", typeof(string[]), typeof(ViewWorker), new PropertyMetadata(null));

        //public int IndexLangItem
        //{
        //    get { return (int)GetValue(IndexLangItemProperty); }
        //    set
        //    {
        //        SetValue(IndexLangItemProperty, value);

        //    }
        //}

        //// Using a DependencyProperty as the backing store for IndexLangItem.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty IndexLangItemProperty =
        //    DependencyProperty.Register("IndexLangItem", typeof(int), typeof(ViewWorker), new PropertyMetadata(-1));


        #endregion

        #region ButtonCommand
        public ICommand Exit_Click
        {
            get { return new ButtonViewCommand((obj) => Environment.Exit(0)); }
        }
        public ICommand Minimalize_Window
        {
            get
            {
                return new ButtonViewCommand((obj) =>
                    MainWindow.window.WindowState = WindowState.Minimized);
            }
        }

        public ICommand OpenVirusPage
        {
            get
            {
                return new ButtonViewCommand((obj) =>
                {
                    if (Pages.Where(x => x.Name == "VirusPageW").ToArray().Length > 0)
                        CurrentPage = Pages[Pages.Select((x, i) => new { element = x, index = i }).First(x => x.element.Name == "VirusPageW").index];
                    else
                    {
                        CreateVirusPage();
                        CurrentPage = Pages[Pages.Select((x, i) => new { element = x, index = i }).First(x => x.element.Name == "VirusPageW").index];
                    }
                });
            }
        }

        public ICommand SearchInCotalog
        {
            get
            {
                return new ButtonViewCommand((obj) =>
                {
                    using (FolderBrowserDialog browserDialog = new FolderBrowserDialog())
                    {
                        if (browserDialog.ShowDialog() == DialogResult.OK)
                        {
                            Task.Run(() =>
                            {
                                antivirusWorker.ScanFiles(browserDialog.SelectedPath, "*.*");
                            });

                        }
                    }
                });
            }

        }

        public ICommand SearchInFile
        {
            get
            {
                return new ButtonViewCommand((obj) =>
                {
                    using (OpenFileDialog openFileDialog = new OpenFileDialog())
                    {
                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            Task.Run(() =>
                            {
                                antivirusWorker.ScanFile(openFileDialog.FileName, false);
                            });
                        }
                    }
                });
            }
        }

        public ICommand AddInException
        {
            get
            {
                return new ButtonViewCommand((obj) =>
                {
                });
            }
        }
        #endregion

        private List<AntivirusLibrary.Abstracts.FileWithSignature> virusList;
        public List<AntivirusLibrary.Abstracts.FileWithSignature> VirusList
        {
            get { return virusList; }
            set
            {
                virusList = value;
                OnPropertyChanged("VirusList");
            }
        }

        private List<AntivirusLibrary.Files.ExceptionFile> exceptionFiles;
        public List<AntivirusLibrary.Files.ExceptionFile> ExceptionFiles
        {
            get { return exceptionFiles; }
            set
            {
                exceptionFiles = value;
                OnPropertyChanged("ExceptionFiles");
            }
        }

        private void UpdateExceptionFiles()
        {
            ExceptionFiles = worker.ExceptionsWork.ExceptionFiles;
        }

        private Page currentPage;
        public Page CurrentPage
        {
            get { return currentPage; }
            set
            {
                if (value == currentPage)
                    return;
                currentPage = value;
                OnPropertyChanged("CurrentPage");
            }
        }
        #region Event callback

        public void GetCount(object sender, AntivirusLibrary.Events.CounterChangeEventArgs e)
        {
            StatusValue = (int)e.Count;
        }

        public void GetMaxValue(object sender, AntivirusLibrary.Events.CounterMaxValueChangeEventArgs e)
        {
            MaxStatusValue = (int)e.MaxValue;
        }

        public void GetUpdateFile(object sender, AntivirusLibrary.Events.FileCheckEventArgs e)
        {
            //AntivirusLibrary.AntivirusWorker work = (AntivirusLibrary.AntivirusWorker)sender;
            //VirusList = work.DangerFiles;


            ////List<AntivirusLibrary.Abstracts.FileWithSignature> newList = new List<AntivirusLibrary.Abstracts.FileWithSignature>();
            ////if (antivirusWorker.DangerFiles.Count != 0)
            ////{
            ////    for (int i = 0; i < antivirusWorker.DangerFiles.Count; i++)
            ////    {
            ////        newList.Add((AntivirusLibrary.Abstracts.FileWithSignature)antivirusWorker.DangerFiles[i].Clone());
            ////    }
            ////}

            ////VirusList = newList;
            List<AntivirusLibrary.Abstracts.FileWithSignature> newList;
            if (e.State)
                newList = new List<AntivirusLibrary.Abstracts.FileWithSignature>(virusList.Select(x => (AntivirusLibrary.Abstracts.FileWithSignature)x.Clone()));
            else
                newList = new List<AntivirusLibrary.Abstracts.FileWithSignature>();

            if (antivirusWorker.DangerFiles.Count != 0)
            {
                AntivirusLibrary.Abstracts.FileWithSignature[] copyDangerFile = antivirusWorker.DangerFiles.Where(x => !newList.Select(y=>y.Path).ToArray().Contains(x.Path)).ToArray();
                for (int i = 0; i < copyDangerFile.Length; i++)
                {
                    newList.Add((AntivirusLibrary.Abstracts.FileWithSignature)copyDangerFile[i].Clone());
                }
            }
            else
            {
                newList = new List<AntivirusLibrary.Abstracts.FileWithSignature>();
            }

            VirusList = newList;

            //VirusList = new List<AntivirusLibrary.Abstracts.FileWithSignature>()
            //{
            //   new AntivirusLibrary.Files.VirusFile(@"C:\Users\Слава\Desktop\Новый текстовый документ (2).txt"),
            //   new AntivirusLibrary.Files.VirusFile(@"C:\Users\Слава\Desktop\Новый текстовый документ (2).txt")
            //};
        }


        public void GetUpdateException(object sender, AntivirusLibrary.Events.AddFileInExceptionEventArgs e)
        {
            if (!worker.ExceptionsWork.ExceptionFiles.Select(x=>x.Path).Contains(e.Path))
            {
                worker.ExceptionsWork.AddException(Environment.CurrentDirectory + @"\exception.cfg", e.Path);
                antivirusWorker.AddInException(this, new AntivirusLibrary.Events.ExceptionAddEventArgs(new AntivirusLibrary.Files.ExceptionFile(e.Path)));
                UpdateExceptionFiles();
            }
        }
        #endregion

        #region Status
        private int statusValue;
        public int StatusValue
        {
            get { return statusValue; }
            set
            {
                statusValue = value;
                OnPropertyChanged("StatusValue");
            }
        }

        private int maxStatusValue;
        public int MaxStatusValue
        {
            get { return maxStatusValue; }
            set
            {
                maxStatusValue = value;
                OnPropertyChanged("MaxStatusValue");
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
