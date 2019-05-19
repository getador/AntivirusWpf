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
            
            //VirusList.Add(new AntivirusLibrary.Files.VirusFile(@"C:\Users\Слава\Desktop\Новый текстовый документ (2).txt"));
            //VirusList.Add(new AntivirusLibrary.Files.VirusFile(@"C:\Users\Слава\Desktop\Новый текстовый документ (2).txt"));
            //SettingsLanguageText = worker.UsedLanguage.SettingsLanguageText;
            ChangeInterfaceLanguage();
            indexLangItem = -1;
            ExceptionList = worker.ExceptionsWork.ExceptionFiles;
            DangerProcessList = new List<AntivirusLibrary.ProcessDange>();
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
            CreteAntiwirusWorker();
            CreateVirusPage();
            CreateDangerProcessPage();
        }

        private void CreteAntiwirusWorker()
        {
            antivirusWorker = new AntivirusLibrary.AntivirusWorker(worker.ExceptionsWork.ExceptionFiles, 1, GetSignatureFile(Environment.CurrentDirectory + @"\hashlid.hash"));
            antivirusWorker.FileCheckedEvent += GetUpdateFile;
            antivirusWorker.Counter.CounterChangeEvent += GetCount;
            antivirusWorker.Counter.MaxValueChangeEvent += GetMaxValue;
            antivirusWorker.FindDangerProcessEvent += UpdateDangerFiles;
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
                ((Pages.VirusPage)Pages[Pages.Count - 1]).DeleteFileEvent += DeleteDangerFile;
                SetDataContext();
            }
        }

        private void CreateExceptionPage()
        {
            if (Pages.Where(x => x.Name == "ExceptionPageW").ToArray().Length == 0)
            {
                Pages.Add(new Pages.ExceptionPage());
                ((Pages.ExceptionPage)Pages[Pages.Count - 1]).DeleteExceptionEvent += RemoveException;
                //((Pages.ExceptionPage)Pages[Pages.Count - 1]).AddFileInExceptionEvent += GetUpdateException;
                SetDataContext();
            }
        }

        private void CreateDangerProcessPage()
        {
            if (Pages.Where(x => x.Name == "DangerProcessPageW").ToArray().Length == 0)
            {
                Pages.Add(new Pages.DangerProcessPage());
                //((Pages.ExceptionPage)Pages[Pages.Count - 1]).AddFileInExceptionEvent += GetUpdateException;
                SetDataContext();
                Task.Run(() => antivirusWorker.ScanProcess());
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

        public ICommand OpenExceptionPage
        {
            get
            {
                return new ButtonViewCommand((obj) =>
                {
                    if (Pages.Where(x => x.Name == "ExceptionPageW").ToArray().Length > 0)
                        CurrentPage = Pages[Pages.Select((x, i) => new { element = x, index = i }).First(x => x.element.Name == "ExceptionPageW").index];
                    else
                    {
                        CreateExceptionPage();
                        CurrentPage = Pages[Pages.Select((x, i) => new { element = x, index = i }).First(x => x.element.Name == "ExceptionPageW").index];
                    }
                });
            }
        }
        public ICommand OpenDangerPricessPage
        {
            get
            {
                return new ButtonViewCommand((obj) =>
                {
                    if (Pages.Where(x => x.Name == "DangerProcessPageW").ToArray().Length > 0)
                        CurrentPage = Pages[Pages.Select((x, i) => new { element = x, index = i }).First(x => x.element.Name == "DangerProcessPageW").index];
                    else
                    {
                        CreateDangerProcessPage();
                        CurrentPage = Pages[Pages.Select((x, i) => new { element = x, index = i }).First(x => x.element.Name == "DangerProcessPageW").index];
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

        public ICommand AddCotalogException
        {
            get
            {
                return new ButtonViewCommand((obj) =>
                {
                    using (FolderBrowserDialog browserDialog = new FolderBrowserDialog())
                    {
                        if (browserDialog.ShowDialog() == DialogResult.OK)
                        {
                            if (!worker.ExceptionsWork.ExceptionFiles.Select(x => x.Path).Contains(browserDialog.SelectedPath))
                            {
                                worker.ExceptionsWork.AddException(Environment.CurrentDirectory + @"\exception.cfg", browserDialog.SelectedPath);
                                antivirusWorker.AddInException(this, new AntivirusLibrary.Events.ExceptionAddEventArgs(new AntivirusLibrary.Files.ExceptionFile(browserDialog.SelectedPath)));
                                UpdateExceptionFiles(true);
                            }
                        }
                    }
                });
            }
        }

        public ICommand AddFileException
        {
            get
            {
                return new ButtonViewCommand((obj) =>
                {
                    using (OpenFileDialog openFileDialog = new OpenFileDialog())
                    {
                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            if (!worker.ExceptionsWork.ExceptionFiles.Select(x => x.Path).Contains(openFileDialog.FileName))
                            {
                                worker.ExceptionsWork.AddException(Environment.CurrentDirectory + @"\exception.cfg", openFileDialog.FileName);
                                antivirusWorker.AddInException(this, new AntivirusLibrary.Events.ExceptionAddEventArgs(new AntivirusLibrary.Files.ExceptionFile(openFileDialog.FileName)));
                                UpdateExceptionFiles(true);
                            }
                        }
                    }
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

        private List<AntivirusLibrary.Files.ExceptionFile> exceptionList;
        public List<AntivirusLibrary.Files.ExceptionFile> ExceptionList
        {
            get { return exceptionList; }
            set
            {
                exceptionList = value;
                OnPropertyChanged("ExceptionList");
            }
        }

        private List<AntivirusLibrary.ProcessDange> dangerProcessList;
        public List<AntivirusLibrary.ProcessDange> DangerProcessList
        {
            get { return dangerProcessList; }
            set
            {
                dangerProcessList = value;
                OnPropertyChanged("DangerProcessList");
            }
        }

        private void UpdateExceptionFiles(bool state)
        {
            List<AntivirusLibrary.Files.ExceptionFile> newList;
            if (state)
                newList = new List<AntivirusLibrary.Files.ExceptionFile>(exceptionList.Select(x => (AntivirusLibrary.Files.ExceptionFile)x.Clone()));
            else
                newList = new List<AntivirusLibrary.Files.ExceptionFile>();

            if (worker.ExceptionsWork.ExceptionFiles.Count != 0)
            {
                AntivirusLibrary.Files.ExceptionFile[] copyDangerFile = worker.ExceptionsWork.ExceptionFiles.Where(x => !newList.Select(y => y.Path).ToArray().Contains(x.Path)).ToArray();
                for (int i = 0; i < copyDangerFile.Length; i++)
                {
                    newList.Add((AntivirusLibrary.Files.ExceptionFile)copyDangerFile[i].Clone());
                }
            }
            else
            {
                newList = new List<AntivirusLibrary.Files.ExceptionFile>();
            }

            ExceptionList = newList;

            if (dangerProcessList.Where(x=>exceptionList.Select(y=>y.Path).Contains(x.Path)).ToArray().Length!=0)
            {
                antivirusWorker.DangerProcess.RemoveAll(x => exceptionList.Select(y => y.Path).Contains(x.Path));
                UpdateDangerProcessList(new AntivirusLibrary.Events.AddDangerProcessEventArgs(false));
            }
        }

        private void UpdateDangerProcessList(AntivirusLibrary.Events.AddDangerProcessEventArgs e)
        {
            List<AntivirusLibrary.ProcessDange> newList;
            if (e.Status)
                newList = new List<AntivirusLibrary.ProcessDange>(dangerProcessList.Select(x => (AntivirusLibrary.ProcessDange)x.Clone()));
            else
                newList = new List<AntivirusLibrary.ProcessDange>();

            if (antivirusWorker.DangerProcess.Count != 0)
            {
                AntivirusLibrary.ProcessDange[] copyDangerFile = antivirusWorker.DangerProcess.Where(x => !newList.Select(y => y.Process.ProcessName).ToArray().Contains(x.Process.ProcessName)).ToArray();
                for (int i = 0; i < copyDangerFile.Length; i++)
                {
                    newList.Add((AntivirusLibrary.ProcessDange)copyDangerFile[i].Clone());
                }
            }
            else
            {
                newList = new List<AntivirusLibrary.ProcessDange>();
            }

            DangerProcessList = newList;
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
                UpdateExceptionFiles(true);
            }
        }

        public void RemoveException(object sender, AntivirusLibrary.Events.AddFileInExceptionEventArgs e)
        {
            worker.ExceptionsWork.RemoveException(Environment.CurrentDirectory + @"\exception.cfg", e.Path);
            UpdateExceptionFiles(false);
        }

        public void DeleteDangerFile(object sender,AntivirusLibrary.Events.AddFileInExceptionEventArgs e)
        {
            antivirusWorker.DangerFiles.FirstOrDefault(x => x.Path == e.Path).DeleteFile();
            antivirusWorker.DangerFiles = antivirusWorker.DangerFiles.Where(x => x.Path != null).ToList();
            GetUpdateFile(this, new AntivirusLibrary.Events.FileCheckEventArgs(false));
        }

        public void UpdateDangerFiles(object sender, AntivirusLibrary.Events.AddDangerProcessEventArgs e)
        {
            UpdateDangerProcessList(e);
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
