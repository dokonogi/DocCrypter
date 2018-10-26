using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using DocCrypter.Model;
using System.Windows.Input;
using Microsoft.Win32;
using System.Windows;
using System.IO;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace DocCrypter.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;

        /// <summary>
        /// 開くファイルへのパスの文字列
        /// </summary>
        private string _strOpenFileName;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService)
        {
            _dataService = dataService;
            _dataService.GetData(
                (item, error) =>
                {
                    if (error != null)
                    {
                        // Report error here
                        return;
                    }

                    _strOpenFileName = string.Empty;
                });
        }

        /// <summary>
        /// 開くファイルへのパスをViewに公開
        /// </summary>
        public string StrOpenFilePath
        {
            get
            {
                return _strOpenFileName;
            }
            set
            {
                _strOpenFileName = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(StrOpenFilePath));
            }
        }
        /// <summary>
        /// ファイルを開くボタンクリック時に発生させるコマンド
        /// </summary>
        private ICommand _fileOpen;
        
        public ICommand FileOpen
        {
            get
            {
                return _fileOpen ?? (_fileOpen = new RelayCommand(FileOpenExecute));
            }
        }

        private async void FileOpenExecute()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".docx";
            dlg.Filter = "Word文書ファイル|*.docx";
            StreamReader file;
            if (dlg.ShowDialog() == false) { return; }

            var window = Application.Current.MainWindow as MetroWindow;
            try
            {
                file = new StreamReader(dlg.FileName);
            }
            catch (IOException)
            {
                await window.ShowMessageAsync("エラー", "ファイルを開けません");
            }
        }
    }
}