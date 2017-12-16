using DARP.Splash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARP.Controllers
{
    public class ViewModelControllers
    {
        #region Singleton

        private static ViewModelControllers instance = new ViewModelControllers();

        public static ViewModelControllers Instance { get { return instance; } }

        #endregion

        //public IWindowController WindowController { get; set; }

        public IDialogController DialogController { get; set; }

        public SplashController SplashController { get; set; }

        public IWindowController WindowController { get; set; }


        //public IProcessController ProcessController { get; set; }

        public void Shutdown()
        {
            System.Windows.Application.Current.Shutdown();
        }

    }
}
