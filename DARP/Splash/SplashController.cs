using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace DARP.Splash
{

    public interface IProgressWindow
    {
        void ShowProgress();
        void HideProgress();
    }
    public class SplashController : IProgressWindow
    {


        private DispatcherTimer timer;
        private SplashView splash;
        private IEnumerable<Window> AllWindows => Application.Current.Windows.Cast<Window>();
        private bool restoreActiveWindow;
        private Window MainWindow => Application.Current.MainWindow;

        public SplashController()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += TimerTick;
        }

        private void TimerTick(object sender, EventArgs e)
        {
            if (splash != null)
            {
                SplashViewModel info = splash.DataContext as SplashViewModel;
                if (info != null)
                    info.UpdateInfo();
            }
        }

        public void HideProgress()
        {
            throw new NotImplementedException();
        }

        public void ShowProgress()
        {
            ShowSplash(new SplashInfo());
        }

        public void ShowSplash(SplashInfo splashInfo)
        {
            if (splash != null)
            {
                splash.DataContext = new SplashViewModel(splashInfo);
            }
            else
            {
                Application.Current.Activated += AppActivated;
                Application.Current.Deactivated += AppDeactivated;

                Window active = AllWindows.FirstOrDefault(w => w.IsActive);
                restoreActiveWindow = active != null;


                foreach (Window window in AllWindows)
                    window.IsEnabled = false;

                splash = new SplashView();
                splash.DataContext = new SplashViewModel(splashInfo);
                splash.Owner = active ?? this.MainWindow;

                splash.Show();
                timer.Start();
            }
        }

        private void AppActivated(object sender, EventArgs e)
        {
            restoreActiveWindow = true;
        }

        private void AppDeactivated(object sender, EventArgs e)
        {
            restoreActiveWindow = false;
        }

        public void HideSplash()
        {
            if (splash != null)
            {
                Application.Current.Activated -= AppActivated;
                Application.Current.Deactivated -= AppDeactivated;

                timer.Stop();
                splash.Close();

                if (restoreActiveWindow)
                    splash.Owner?.Activate();

                splash.DataContext = null;
                splash = null;

                foreach (Window window in AllWindows)
                    window.IsEnabled = true;
            }
        }

        public static SplashController Instance
        {
            get
            {
                return instance;
            }
            set
            {
                if (instance != null)
                    throw new InvalidOperationException("Ya hay una instancia de la clase Singleton");

                instance = value;
            }
        }

        private static SplashController instance;
    }
}
