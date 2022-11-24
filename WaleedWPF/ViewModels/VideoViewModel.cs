using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;
using Caliburn.Micro;

namespace WaleedWPF.ViewModels
{
    internal sealed class VideoViewModel : Screen
    {
        private string _selectedFile;
        public ObservableCollection<string> Files { get; set; } = new ObservableCollection<string>();

        public string SelectedFile
        {
            get => _selectedFile;
            set => Set(ref _selectedFile, value);
        }

        public VideoViewModel()
        {
            PropertyChanged += VideoViewModel_PropertyChanged;
        }


        private void VideoViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (nameof(SelectedFile) == e.PropertyName)
            {

            }
        }

        protected override Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            var files = Directory.EnumerateFiles("./FilesVideo");
            files.Apply(Files.Add);
            return base.OnInitializeAsync(cancellationToken);
        }

        public void Open()
        {
            Process.Start(SelectedFile);
        }
    }
}
