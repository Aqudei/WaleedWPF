using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace WaleedWPF.ViewModels
{
    internal class ShellViewModel : Conductor<object>
    {
        public ShellViewModel()
        {

        }

        public async void ShowPdfPage()
        {
            await ActivateItemAsync(IoC.Get<PdfViewModel>());
        }

        public async void ShowVideoPage()
        {
            await ActivateItemAsync(IoC.Get<VideoViewModel>());
        }

        public async void ShowBatPage()
        {
            await ActivateItemAsync(IoC.Get<BatViewModel>());
        }
    }
}
