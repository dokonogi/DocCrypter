using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using DocCrypter.Model;
using System.Windows.Input;
using Microsoft.Win32;
using System.Windows;
using System.IO;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;

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
        /// 画像ファイルの中身
        /// </summary>
        private byte[] imageFile;
        /// <summary>
        /// 暗号化するファイルの中身
        /// </summary>
        private byte[] originFile;
        /// <summary>
        /// 復元したファイルの中身
        /// </summary>
        private byte[] restoreFile;

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
                });
        }

        private string _originFilePath;

        /// <summary>
        /// オリジナルのファイルのパスを表示するtextboxに表示する文字列
        /// </summary>
        public string originFilePath
        {
            get
            {
                return _originFilePath;
            }
            set
            {
                _originFilePath = value;
                RaisePropertyChanged("originFilePath");
            }
        }

        private string _imageFilePath;

        /// <summary>
        /// 画像ファイルのパスを表示するtextboxに表示する文字列
        /// </summary>
        public string imageFilePath
        {
            get
            {
                return _imageFilePath;
            }
            set
            {
                _imageFilePath = value;
                RaisePropertyChanged("imageFilePath");
            }
        }

        /// <summary>
        /// ファイルを開くボタンクリック時に発生させるコマンド
        /// </summary>
        private ICommand _ReadOriginFile;
        
        public ICommand ReadOriginFile
        {
            get
            {
                return _ReadOriginFile ?? (_ReadOriginFile = new RelayCommand(ReadOriginFileExecute));
            }
        }

        private async void ReadOriginFileExecute()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".docx";
            dlg.Filter = "Word文書ファイル|*.docx|すべてのファイル|*.*";
            FileStream file;
            BinaryReader reader;
            if (dlg.ShowDialog() == false) { return; }

            var window = Application.Current.MainWindow as MetroWindow;
            try
            {
                file = new FileStream(dlg.FileName, FileMode.Open);
                reader = new BinaryReader(file);
                originFile = reader.ReadBytes((int)file.Length);
            }
            catch (IOException)
            {
                await window.ShowMessageAsync("エラー", "ファイルを開けません");
            }
            originFilePath = dlg.FileName;
        }

        /// <summary>
        /// ファイルを開くボタンクリック時に発生させるコマンド
        /// </summary>
        private ICommand _ReadImageFile;

        public ICommand ReadImageFile
        {
            get
            {
                return _ReadImageFile ?? (_ReadImageFile = new RelayCommand(ReadImageFileExecute));
            }
        }

        private async void ReadImageFileExecute()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".jpg";
            dlg.Filter = "JPEGファイル|*.jpg";
            FileStream file;
            BinaryReader reader;
            if (dlg.ShowDialog() == false) { return; }

            var window = Application.Current.MainWindow as MetroWindow;
            try
            {
                file = new FileStream(dlg.FileName, FileMode.Open);
                reader = new BinaryReader(file);
                imageFile = reader.ReadBytes((int)file.Length);
            }
            catch (IOException)
            {
                await window.ShowMessageAsync("エラー", "ファイルを開けません");
            }
            imageFilePath = dlg.FileName;
        }

        /// <summary>
        /// 暗号化したファイルを生成するコマンド
        /// </summary>
        private ICommand _Generate;

        public ICommand Generate
        {
            get
            {
                return _Generate ?? (_Generate = new RelayCommand(GenerateExecute));
            }
        }

        private void GenerateExecute()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.DefaultExt = ".jpg";
            dlg.Filter = "JPEGファイル|*.jpg";
            if (dlg.ShowDialog() == true)
            {
                Stream stream;
                stream = dlg.OpenFile();
                if (stream != null)
                {
                    BinaryWriter br = new BinaryWriter(stream);
                    br.Write(imageFile);
                    br.Write(originFile);
                    br.Close();
                    stream.Close();
                }
            }
        }

        private ICommand _Restore;

        public ICommand Restore
        {
            get
            {
                return _Restore ?? (_Restore = new RelayCommand(RestoreExecute));
            }
        }

        private void RestoreExecute()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.DefaultExt = ".docx";
            dlg.Filter = "Word文書ファイル|*.docx|すべてのファイル|*.*";
            if(dlg.ShowDialog() == true)
            {
                Stream stream;
                stream = dlg.OpenFile();
                if(stream != null)
                {
                    var tmp = new byte[5]{ 0xFF, 0xD9, 0x50, 0x4B, 0x03 };
                    var startIndex = UsefulMethod.byteSearchFromEnd(imageFile, tmp) + 2;
                    Console.WriteLine(startIndex);
                    restoreFile = new byte[imageFile.Length - startIndex];
                    Array.Copy(imageFile, startIndex, restoreFile, 0, imageFile.Length - startIndex);
                    BinaryWriter br = new BinaryWriter(stream);
                    br.Write(restoreFile);
                    br.Close();
                    stream.Close();
                }
            }
        }

    }
}