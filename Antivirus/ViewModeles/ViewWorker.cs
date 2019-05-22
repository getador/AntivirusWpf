using Antivirus.Modeles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
            ServerIp = "127.0.0.1";
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
            
            CreteAntiwirusWorker();
            PropertyOfSettings();
            CreateVirusPage();
            CreateDangerProcessPage();
        }

        private void PropertyOfSettings()
        {
            CountOfThread = worker.SettingsLoaded.CountOfThread.ToString();
            AutoVirusDelete = worker.SettingsLoaded.AutoVirusDelete;
            EvrizmM = worker.SettingsLoaded.EvrizmM;
            SignatureM = worker.SettingsLoaded.SignatureM;
            Sound = worker.SettingsLoaded.Sound;
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
            DangerFileButtonText = worker.UsedLanguage.DangerFileButtonText;
            ExceptionFileButtonText = worker.UsedLanguage.ExceptionFileButtonText;
            DangerProcessButtonText = worker.UsedLanguage.DangerProcessButtonText;
            StopButtonContext = worker.UsedLanguage.StopButtonContext;
            DeleteButtonContext = worker.UsedLanguage.DeleteButtonContext;
            AddInExceptionButtonContext = worker.UsedLanguage.AddInExceptionButtonContext;
            KillProcessButtonContext = worker.UsedLanguage.KillProcessButtonContext;
            ScanCotalogButtonContext = worker.UsedLanguage.ScanCotalogButtonContext;
            ScanFileButtonContext = worker.UsedLanguage.ScanFileButtonContext;
            AddCotalogInExceptionButtonContext = worker.UsedLanguage.AddCotalogInExceptionButtonContext;
            AddFileInExceptionButtonContext = worker.UsedLanguage. AddFileInExceptionButtonContext;
            DeleteAllButtonContext = worker.UsedLanguage.DeleteAllButtonContext;
            ScanSettingsText = worker.UsedLanguage.ScanSettingsText;
            AutoDeleteVirusFileSetttingsText = worker.UsedLanguage.AutoDeleteVirusFileSetttingsText;
            SignatureSettingsText = worker.UsedLanguage.SignatureSettingsText;
            EvrizmSettingsText = worker.UsedLanguage.EvrizmSettingsText;
            CountThreadSettingsText = worker.UsedLanguage.CountThreadSettingsText;
            SoundSettingsText = worker.UsedLanguage.SoundSettingsText;
            SendVirusContent = worker.UsedLanguage.SendVirusContent;
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
                ((Pages.DangerProcessPage)Pages[Pages.Count - 1]).AddFileInExceptionEvent += GetUpdateException;
                ((Pages.DangerProcessPage)Pages[Pages.Count - 1]).KillProcessEvent += KillProcess;
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

        private string dangerFileButtonText;
        public string DangerFileButtonText
        {
            get { return dangerFileButtonText; }
            set
            {
                if (value == dangerFileButtonText)
                    return;
                dangerFileButtonText = value;
                OnPropertyChanged("DangerFileButtonText");
            }
        }

        private string exceptionFileButtonText;
        public string ExceptionFileButtonText
        {
            get { return exceptionFileButtonText; }
            set
            {
                if (value == exceptionFileButtonText)
                    return;
                exceptionFileButtonText = value;
                OnPropertyChanged("ExceptionFileButtonText");
            }
        }

        private string dangerProcessButtonText;
        public string DangerProcessButtonText
        {
            get { return dangerProcessButtonText; }
            set
            {
                if (value == dangerProcessButtonText)
                    return;
                dangerProcessButtonText = value;
                OnPropertyChanged("DangerProcessButtonText");
            }
        }

        private string stopButtonContext;
        public string StopButtonContext
        {
            get { return stopButtonContext; }
            set
            {
                if (value == stopButtonContext)
                    return;
                stopButtonContext = value;
                OnPropertyChanged("StopButtonContext");
            }
        }

        private string deleteButtonContext;
        public string DeleteButtonContext
        {
            get { return deleteButtonContext; }
            set
            {
                if (value == deleteButtonContext)
                    return;
                deleteButtonContext = value;
                OnPropertyChanged("DeleteButtonContext");
            }
        }

        private string addInExceptionButtonContext;
        public string AddInExceptionButtonContext
        {
            get { return addInExceptionButtonContext; }
            set
            {
                if (value == addInExceptionButtonContext)
                    return;
                addInExceptionButtonContext = value;
                OnPropertyChanged("AddInExceptionButtonContext");
            }
        }

        private string killProcessButtonContext;
        public string KillProcessButtonContext
        {
            get { return killProcessButtonContext; }
            set
            {
                if (value == killProcessButtonContext)
                    return;
                killProcessButtonContext = value;
                OnPropertyChanged("KillProcessButtonContext");
            }
        }

        private string scanCotalogButtonContext;
        public string ScanCotalogButtonContext
        {
            get { return scanCotalogButtonContext; }
            set
            {
                if (value == scanCotalogButtonContext)
                    return;
                scanCotalogButtonContext = value;
                OnPropertyChanged("ScanCotalogButtonContext");
            }
        }

        private string scanFileButtonContext;
        public string ScanFileButtonContext
        {
            get { return scanFileButtonContext; }
            set
            {
                if (value == scanFileButtonContext)
                    return;
                scanFileButtonContext = value;
                OnPropertyChanged("ScanFileButtonContext");
            }
        }

        private string addCotalogInExceptionButtonContext;
        public string AddCotalogInExceptionButtonContext
        {
            get { return addCotalogInExceptionButtonContext; }
            set
            {
                if (value == addCotalogInExceptionButtonContext)
                    return;
                addCotalogInExceptionButtonContext = value;
                OnPropertyChanged("AddCotalogInExceptionButtonContext");
            }
        }

        private string addFileInExceptionButtonContext;
        public string AddFileInExceptionButtonContext
        {
            get { return addFileInExceptionButtonContext; }
            set
            {
                if (value == addFileInExceptionButtonContext)
                    return;
                addFileInExceptionButtonContext = value;
                OnPropertyChanged("AddFileInExceptionButtonContext");
            }
        }

        private string deleteAllButtonContext;
        public string DeleteAllButtonContext
        {
            get { return deleteAllButtonContext; }
            set
            {
                if (value == deleteAllButtonContext)
                    return;
                deleteAllButtonContext = value;
                OnPropertyChanged("DeleteAllButtonContext");
            }
        }


        private string scanSettingsText;
        public string ScanSettingsText
        {
            get { return scanSettingsText; }
            set
            {
                if (value == scanSettingsText)
                    return;
                scanSettingsText = value;
                OnPropertyChanged("ScanSettingsText");
            }
        }

        private string autoDeleteVirusFileSetttingsText;
        public string AutoDeleteVirusFileSetttingsText
        {
            get { return autoDeleteVirusFileSetttingsText; }
            set
            {
                if (value == autoDeleteVirusFileSetttingsText)
                    return;
                autoDeleteVirusFileSetttingsText = value;
                OnPropertyChanged("AutoDeleteVirusFileSetttingsText");
            }
        }

        private string signatureSettingsText;
        public string SignatureSettingsText
        {
            get { return signatureSettingsText; }
            set
            {
                if (value == signatureSettingsText)
                    return;
                signatureSettingsText = value;
                OnPropertyChanged("SignatureSettingsText");
            }
        }

        private string evrizmSettingsText;
        public string EvrizmSettingsText
        {
            get { return evrizmSettingsText; }
            set
            {
                if (value == evrizmSettingsText)
                    return;
                evrizmSettingsText = value;
                OnPropertyChanged("EvrizmSettingsText");
            }
        }

        private string countThreadSettingsText;
        public string CountThreadSettingsText
        {
            get { return countThreadSettingsText; }
            set
            {
                if (value == countThreadSettingsText)
                    return;
                countThreadSettingsText = value;
                OnPropertyChanged("CountThreadSettingsText");
            }
        }

        private string soundSettingsText;
        public string SoundSettingsText
        {
            get { return soundSettingsText; }
            set
            {
                if (value == soundSettingsText)
                    return;
                soundSettingsText = value;
                OnPropertyChanged("SoundSettingsText");
            }
        }

        private string sendVirusContent;
        public string SendVirusContent
        {
            get { return sendVirusContent; }
            set
            {
                if (value == sendVirusContent)
                    return;
                sendVirusContent = value;
                OnPropertyChanged("SendVirusContent");
            }
        }
        #region LanguageStopScan
        private string titleOfMessage;
        public string TitleOfMessage
        {
            get { return titleOfMessage; }
            set
            {
                if (value == titleOfMessage)
                    return;
                titleOfMessage = value;
                OnPropertyChanged("TitleOfMessage");
            }
        }

        private string messageInMes;
        public string MessageInMes
        {
            get { return messageInMes; }
            set
            {
                if (value == messageInMes)
                    return;
                messageInMes = value;
                OnPropertyChanged("MessageInMes");
            }
        }

        private string firstButtonContext;
        public string FirstButtonContext
        {
            get { return firstButtonContext; }
            set
            {
                if (value == firstButtonContext)
                    return;
                firstButtonContext = value;
                OnPropertyChanged("FirstButtonContext");
            }
        }

        private string secondButtonContext;
        public string SecondButtonContext
        {
            get { return secondButtonContext; }
            set
            {
                if (value == secondButtonContext)
                    return;
                secondButtonContext = value;
                OnPropertyChanged("SecondButtonContext");
            }
        }
        #endregion

        #endregion

        #region settingsValue
        private bool autoVirusDelete;
        public bool AutoVirusDelete
        {
            get { return autoVirusDelete; }
            set
            {
                autoVirusDelete = value;
                antivirusWorker.AutoDeleteVirus = autoVirusDelete;
                worker.SettingsLoaded.AutoVirusDelete = autoVirusDelete;
                worker.SettingsLoaded.SaveSettings(Environment.CurrentDirectory + @"\config.cfg");
                OnPropertyChanged("AutoVirusDelete");
            }
        }

        private bool signatureM;
        public bool SignatureM
        {
            get { return signatureM; }
            set
            {
                signatureM = value;
                antivirusWorker.SignatureM = signatureM;
                worker.SettingsLoaded.SignatureM = signatureM;
                worker.SettingsLoaded.SaveSettings(Environment.CurrentDirectory + @"\config.cfg");
                OnPropertyChanged("SignatureM");
            }
        }

        private bool evrizmM;
        public bool EvrizmM
        {
            get { return evrizmM; }
            set
            {
                evrizmM = value;
                antivirusWorker.EvrizmM = evrizmM;
                worker.SettingsLoaded.EvrizmM = evrizmM;
                worker.SettingsLoaded.SaveSettings(Environment.CurrentDirectory + @"\config.cfg");
                OnPropertyChanged("EvrizmM");
            }
        }

        private string countOfThread;
        public string CountOfThread
        {
            get { return countOfThread; }
            set
            {
                countOfThread = value;
                antivirusWorker.CountThread = Convert.ToInt32(countOfThread);
                worker.SettingsLoaded.CountOfThread = Convert.ToInt32(countOfThread);
                worker.SettingsLoaded.SaveSettings(Environment.CurrentDirectory + @"\config.cfg");
                OnPropertyChanged("CountOfThread");
            }
        }

        private bool sound;
        public bool Sound
        {
            get { return sound; }
            set
            {
                sound = value;
                antivirusWorker.SoundTurn = sound;
                worker.SettingsLoaded.Sound = sound;
                worker.SettingsLoaded.SaveSettings(Environment.CurrentDirectory + @"\config.cfg");
                OnPropertyChanged("Sound");
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
                                antivirusWorker.ScanFiles(browserDialog.SelectedPath, "*.*");//"C:\\Users\\Слава\\Desktop\\Учеба\\Delphi\\ОАиП\\лр14(2)\\Win32\\Debug", "*.*");
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
                                antivirusWorker.ScanFile(openFileDialog.FileName, true);
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

        public ICommand StopScan
        {
            get
            {
                return new ButtonViewCommand((obj) =>
                {
                    if (antivirusWorker.Workers != null && !antivirusWorker.Workers.All(x=>x.CheckFileThread.Status!=TaskStatus.Running))
                    {
                        if (new CustomMessageBox().ShopMessage("Отмена", "Отменить сканирование?", "Да", "Нет") == 1)
                        {

                            foreach (AntivirusLibrary.Workers.VirusWorker worker in antivirusWorker.Workers.Where(x => x.CheckFileThread.Status == TaskStatus.Running).ToArray())
                            {
                                worker.StopScan();
                            }
                            antivirusWorker.Counter.MaxValue = statusValue;

                        }
                    }
                });
            }
        }
        public ICommand DeleteAllException
        {
            get
            {
                return new ButtonViewCommand((obj) =>
                {
                    worker.ExceptionsWork.ExceptionFiles.RemoveAll(x=>x.Path!=null);
                    worker.ExceptionsWork.SaveException(Environment.CurrentDirectory + @"\exception.cfg");
                    UpdateExceptionFiles(false);
                });
            }
        }

        public ICommand DeleteAllVirusFile
        {
            get
            {
                return new ButtonViewCommand((obj) =>
                {
                    foreach (var item in antivirusWorker.DangerFiles)
                    {
                        item.DeleteFile();
                    }
                    antivirusWorker.DangerFiles = antivirusWorker.DangerFiles.Where(x => x.Path != null).ToList();
                    GetUpdateFile(this, new AntivirusLibrary.Events.FileCheckEventArgs(false));
                });
            }
        }

        public ICommand SetIp
        {
            get
            {
                return new ButtonViewCommand((obj) =>
                {
                    Task.Run(() => ServerIp = (string)obj);
                    //try
                    //{
                    //    socketForUpdate = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    //    socketForAddVirus = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    //    socketForUpdate.Connect(serverIp, 5555);
                    //    socketForAddVirus.Connect(serverIp, 5556);
                    //    Task.Run(() => GetFromServer());
                    //}
                    //catch (Exception)
                    //{
                    //}
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

        public void KillProcess(object sender,AntivirusLibrary.Events.AddFileInExceptionEventArgs e)
        {
            antivirusWorker.KillProcess(antivirusWorker.DangerProcess.Where(x => x.Process.ProcessName == e.Path).First());
            UpdateDangerProcessList(new AntivirusLibrary.Events.AddDangerProcessEventArgs(false));
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

        #region Server work
        private string serverIp;
        public string ServerIp
        {
            get { return serverIp; }
            set
            {
                serverIp = value;
                try
                {
                    socketForUpdate = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socketForAddVirus = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socketForUpdate.Connect(serverIp, 5555);
                    socketForAddVirus.Connect(serverIp, 5556);
                    Task.Run(() => GetFromServer());
                }
                catch (Exception)
                {
                }
                OnPropertyChanged("ServerIp");
            }
        }

        public Socket socketForUpdate = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public Socket socketForAddVirus = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private bool openToWrite = false;
        private void GetFromServer()
        {
            while (socketForUpdate.Connected)
            {
                if (antivirusWorker != null)
                {
                    try
                    {
                        byte[] buffer = Encoding.UTF8.GetBytes("#U");
                        socketForUpdate.Send(buffer);
                        int amount_of_received_data = 0;
                        buffer = new byte[50];
                        socketForUpdate.Receive(buffer);
                        int length = int.Parse(Encoding.UTF8.GetString(buffer));
                        byte[] bufferWithHash = new byte[length];
                        if (buffer.All(x => x == 0))
                        {
                            throw new Exception();
                        }
                        else
                        {
                            byte[] callNewBuffer = Encoding.UTF8.GetBytes("#NE");
                            socketForUpdate.Send(callNewBuffer);
                            while (amount_of_received_data < length)
                            {
                                int received = socketForUpdate.Receive(bufferWithHash, amount_of_received_data, length - amount_of_received_data, SocketFlags.None);
                                if (received == 0)
                                {
                                    throw new Exception();
                                }
                                amount_of_received_data += received;
                            }
                            string message = Encoding.UTF8.GetString(bufferWithHash).Replace("\0", "");
                            string[] arrayOfHash = message.Replace("\r", "").Split('\n');
                            string stringWithVirusHash;
                            if (File.Exists(Directory.GetCurrentDirectory() + @"\hashlid.hash"))
                            {
                                stringWithVirusHash = GetSignatureFile(Environment.CurrentDirectory + @"\hashlid.hash");

                                while (openToWrite)
                                {

                                }
                                openToWrite = true;
                                using (StreamWriter stream = new StreamWriter(Directory.GetCurrentDirectory() + @"\hashlid.hash",
                                true,
                                Encoding.UTF8))
                                {
                                    for (int i = 0; i < arrayOfHash.Length; i++)
                                    {
                                        if (!stringWithVirusHash.Contains(arrayOfHash[i]) && arrayOfHash[i] != string.Empty)
                                        {
                                            stream.WriteLine(arrayOfHash[i]);
                                        }
                                    }
                                }
                                openToWrite = false;
                            }
                            else
                            {
                                using (StreamWriter stream = new StreamWriter(Directory.GetCurrentDirectory() + @"\hashlid.hash",
                                true,
                                Encoding.UTF8))
                                {
                                    stream.WriteLine(string.Join("\r\n", arrayOfHash));
                                    //for (int i = 0; i < arrayOfHash.Length; i++)
                                    //{

                                    //    stream.WriteLine(arrayOfHash[i]);
                                    //}
                                }
                            }

                            antivirusWorker.SignatureString = GetSignatureFile(Environment.CurrentDirectory + @"\hashlid.hash");

                        }
                    }
                    catch (Exception)
                    {
                        socketForUpdate.Disconnect(false);
                        socketForAddVirus.Disconnect(false);
                        socketForUpdate.Close();
                        socketForAddVirus.Close();
                    }
                }

                Thread.Sleep(60000);
            }
        }
        public ICommand SendVirusMenu_Click
        {
            get
            {
                return new ButtonViewCommand((obj) =>
                {
                    if (socketForAddVirus.Connected)
                    {
                        OpenFileDialog ofd = new OpenFileDialog();
                        if (ofd.ShowDialog() == DialogResult.OK)
                        {
                            string sendOnServer = string.Empty;

                            var permission = new FileIOPermission(FileIOPermissionAccess.Write, ofd.FileName);
                            var permissionSet = new PermissionSet(PermissionState.None);
                            permissionSet.AddPermission(permission);
                            if (permissionSet.IsSubsetOf(AppDomain.CurrentDomain.PermissionSet))
                            {
                                try
                                {
                                    using (var md5 = MD5.Create())
                                {
                                    using (var stream = File.OpenRead(ofd.FileName))
                                    {
                                        bool find = false;
                                        string hash = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "");
                                        if (antivirusWorker.SignatureString != null)
                                        {
                                            if (antivirusWorker.SignatureString.Contains(hash))
                                            {
                                                //MessageBox.Show("Данная сигнатура уже существует в базе");
                                                find = true;
                                            }
                                        }
                                        if (!find)
                                        {
                                            while (openToWrite)
                                            {

                                            }
                                            openToWrite = true;
                                            using (StreamWriter sw = new StreamWriter(Directory.GetCurrentDirectory() + @"\hashlid.hash",
                                            true,
                                            Encoding.UTF8))
                                            {
                                                sw.WriteLine(hash);
                                            }
                                            openToWrite = false;
                                            sendOnServer = $"#V{hash}";
                                        }
                                    }
                                }
                            }
                                catch (IOException)
                            {

                            }
                            //catch (Exception)
                            //{

                            //}
                        }
                            if (sendOnServer != string.Empty)
                            {
                                byte[] buffer = Encoding.UTF8.GetBytes(sendOnServer);
                                socketForAddVirus.Send(buffer);
                            }
                        }
                    }
                    //else
                        //MessageBox.Show("Необходимо подключение к серверу");
                });
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
