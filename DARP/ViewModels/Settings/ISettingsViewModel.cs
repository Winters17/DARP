using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARP.ViewModels.Settings
{
    public interface ISettingsViewModel
    {
        void ExecuteCloseWindow();
        void ExecuteRestoreDefaultValues();
        void ExecuteSave();
    }
}
