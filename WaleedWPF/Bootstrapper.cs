using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using WaleedWPF.ViewModels;
using WaleedWPF.Views;

namespace WaleedWPF
{
    public class Bootstrapper : BootstrapperBase
    {
        private SimpleContainer _container;

        public Bootstrapper()
        {
            Initialize();
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override void Configure()
        {
            base.Configure();
            _container = new SimpleContainer();
            _container.Singleton<IEventAggregator, EventAggregator>();
            _container.PerRequest<IWindowManager, WindowManager>();
            _container.PerRequest<ShellViewModel>();
            _container.PerRequest<PdfViewModel>();
            _container.PerRequest<VideoViewModel>();
            _container.PerRequest<BatViewModel>();
        }

        protected override async void OnStartup(object sender, StartupEventArgs e)
        {
            await DisplayRootViewForAsync<ShellViewModel>();
        }
    }
}
