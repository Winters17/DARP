using DARP.Controllers;
using DARP.Splash;
using DARP.ViewModels;
using DARP.ViewModels.Main;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DARP
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var controllers = ViewModelControllers.Instance;
            controllers.SplashController = new SplashController();
            controllers.DialogController = new DialogController();
            controllers.WindowController = new WindowController();
            WindowController.Instance = new WindowController(WindowMapping.Mappings);
            WindowController.Instance.ShowWindow(new MainWindowViewModel());
        }
    }
}
