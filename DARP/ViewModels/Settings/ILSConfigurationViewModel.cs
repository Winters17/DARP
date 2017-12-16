using API.DARP.Data.Model.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARP.ViewModels.Settings
{
    public class ILSConfigurationViewModel : ISettingsViewModel, INotifyPropertyChanged
    {

        public ILSConfigurationSettings ILSConfiguration { get; set; }

        public ILSConfigurationViewModel()
        {
            ILSConfiguration = new ILSConfigurationSettings(Context.Instance.Settings.ILSConfigurationSettings ?? ILSConfigurationSettings.CreateDefaults());
        }


        public void ExecuteCloseWindow()
        {
            throw new NotImplementedException();
        }

        public void ExecuteRestoreDefaultValues()
        {
            ILSConfiguration = ILSConfigurationSettings.CreateDefaults();
            NotifyPropertyChanged(nameof(ILSConfiguration));
        }

        public void ExecuteSave()
        {
            Context.Instance.Settings.ILSConfigurationSettings = ILSConfiguration;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;


        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
