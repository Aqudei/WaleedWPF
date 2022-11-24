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
    internal sealed class BatViewModel : Screen
    {
        private string _selectedFile;
        private string _batContent;
        public ObservableCollection<string> Files { get; set; } = new ObservableCollection<string>();

        public string BatContent
        {
            get => _batContent;
            set => Set(ref _batContent, value);
        }

        public string SelectedFile
        {
            get => _selectedFile;
            set => Set(ref _selectedFile, value);
        }

        public BatViewModel()
        {
            PropertyChanged += VideoViewModel_PropertyChanged;
        }


        private void VideoViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (nameof(SelectedFile) == e.PropertyName)
            {
                BatContent = File.ReadAllText(SelectedFile);
            }
        }

        protected override Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            var files = Directory.EnumerateFiles("./FilesBat");
            files.Apply(Files.Add);
            return base.OnInitializeAsync(cancellationToken);
        }

        public IEnumerable<IResult> Open()
        {
            yield return Task.Run(() =>
            {
                var procInfo = new ProcessStartInfo
                {
                    FileName = SelectedFile,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(procInfo))
                {
                    if (process != null)
                    {
                        // var output = await process.StandardOutput.ReadToEndAsync();
                        process.OutputDataReceived += Process_OutputDataReceived;
                        process.BeginOutputReadLine();
                        process.WaitForExit();
                        process.OutputDataReceived -= Process_OutputDataReceived;

                    }
                }
            }).AsResult();
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Debug.WriteLine(e.Data);
        }
    }
}
