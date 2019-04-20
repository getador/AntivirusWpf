using Antivirus.Modeles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Antivirus.ViewModeles
{
    internal class ViewWorker : DependencyObject
    {
        private LangWorker worker;
        public ViewWorker()
        {
            worker = new LangWorker();

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

        #region Language settings

        ////public string SettingsLanguageText
        ////{
        ////    get { return worker.UsedLanguage.SettingsLanguageText; }
        ////}



        public string SettingsLanguageText
        {
            get { return (string)GetValue(SettingsLanguageTextProperty); }
            set { SetValue(SettingsLanguageTextProperty, worker.UsedLanguage.SettingsLanguageText); }
        }

        // Using a DependencyProperty as the backing store for SettingsLanguageText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SettingsLanguageTextProperty =
            DependencyProperty.Register("SettingsLanguageText", typeof(string), typeof(ViewWorker), new PropertyMetadata(""));



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
        #endregion
    }
}
