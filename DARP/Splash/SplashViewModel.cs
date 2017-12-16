using DARP.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DARP.Splash
{
    class SplashViewModel : INotifyPropertyChanged
    {

        private SplashInfo splashInfo;
        public SplashInfo SplashInfo => splashInfo;


        public SplashViewModel(SplashInfo splashInfo)
        {
            this.splashInfo = splashInfo;

        }



        public void UpdateInfo()
        {
            //Actualizar los datos genéricos del splash
            this.splashInfo.UpdateInfo();
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
