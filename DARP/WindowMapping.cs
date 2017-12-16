using DARP.Controllers;
using DARP.ViewModels;
using DARP.ViewModels.Main;
using DARP.ViewModels.Settings;
using DARP.Views;
using DARP.Views.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARP
{
    public static class WindowMapping
    {
        public static IEnumerable<WindowMap> Mappings { get; } = new WindowMap[]
        {
                new WindowMap<MainWindow, MainWindowViewModel>(),
                new WindowMap<SettingsWindow, SettingsViewModel>(),
        };
    }

}
