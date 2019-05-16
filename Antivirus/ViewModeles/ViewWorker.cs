using Antivirus.Modeles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;

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
            antivirusWorker = new AntivirusLibrary.AntivirusWorker(worker.ExceptionsWork.ExceptionFiles,1);
            antivirusWorker.FileCheckedEvent += GetUpdateFile;
            CreateVirusPage();
            VirusList.Add(new AntivirusLibrary.Files.VirusFile(@"C:\Users\Слава\Desktop\Новый текстовый документ (2).txt"));
            VirusList.Add(new AntivirusLibrary.Files.VirusFile(@"C:\Users\Слава\Desktop\Новый текстовый документ (2).txt"));
            SettingsLanguageText = worker.UsedLanguage.SettingsLanguageText;
            ChangeInterfaceLanguage();
            indexLangItem = -1;
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
                            antivirusWorker.ScanFiles(browserDialog.SelectedPath, "*.exe");
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
                            antivirusWorker.ScanFile(openFileDialog.FileName, false);
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
                if (value == virusList)
                    return;
                virusList = value;
                OnPropertyChanged("VirusList");
            }
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

        public void GetUpdateFile(object sender, AntivirusLibrary.Events.FileCheckEventArgs e)
        {
            VirusList = ((AntivirusLibrary.AntivirusWorker)sender).DangerFiles;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
