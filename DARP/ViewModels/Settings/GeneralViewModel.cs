using API.DARP.Data.Model.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace DARP.ViewModels.Settings
{
    public class GeneralViewModel : ISettingsViewModel, INotifyPropertyChanged
    {

        #region Fields
        CommandList commands = new CommandList();
        #endregion


        #region Bindings

        public List<string> ListHeuristics { get; set; } = new List<string> { "AAA", "BBB", "CCC" };


        #region GeneralSettings 

        private GeneralSettings generalSettings;

        public GeneralSettings GeneralSettings
        {
            get
            {
                return generalSettings;
            }

            set
            {
                if (value != generalSettings)
                {
                    generalSettings = value;
                    // OnGeneralSettingsChanged();
                    NotifyPropertyChanged(nameof(GeneralSettings));
                }
            }
        }
        #endregion



        #endregion



        #region Commands

        public ICommand BrowserFolders { get; }

        public ICommand BrowserFoldersSolutions { get; set; }



        #endregion

        public GeneralViewModel()
        {
            GeneralSettings = new GeneralSettings(Context.Instance.Settings.GeneralSettings ?? GeneralSettings.CreateDefaults());
            BrowserFolders = commands.AddCommand(ExecuteOpenFolderDialog, CanExecuteOpenFolderDialog);
            BrowserFoldersSolutions = commands.AddCommand(ExecuteOpenFolderDialogSolutions, CanExecuteOpenFolderDialog);
        }


        #region CommandImplementations

        private void ExecuteOpenFolderDialog()
        {

            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.RootFolder = Environment.SpecialFolder.MyComputer;

            DialogResult result = fbd.ShowDialog();
            if (!string.IsNullOrEmpty(fbd.SelectedPath))
            {
                GeneralSettings.RootPath = fbd.SelectedPath;
                NotifyPropertyChanged(nameof(GeneralSettings));
            }
        }

        private void ExecuteOpenFolderDialogSolutions()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.RootFolder = Environment.SpecialFolder.MyComputer;

            DialogResult result = fbd.ShowDialog();
            if (!string.IsNullOrEmpty(fbd.SelectedPath))
            {
                GeneralSettings.RootPathSolutions = fbd.SelectedPath;
                NotifyPropertyChanged(nameof(GeneralSettings));
            }
        }

        private bool CanExecuteOpenFolderDialog()
        {
            return true;
        }


        #endregion


        public void ExecuteCloseWindow()
        {
            throw new NotImplementedException();
        }

        public void ExecuteRestoreDefaultValues()
        {
            GeneralSettings = GeneralSettings.CreateDefaults();
            NotifyPropertyChanged(nameof(GeneralSettings));
        }

        public void ExecuteSave()
        {
            Context.Instance.Settings.GeneralSettings = GeneralSettings;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
