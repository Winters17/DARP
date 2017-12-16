using API.DARP.Data.Model.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace DARP.ViewModels.Settings
{
    public class SettingsViewModel : INotifyPropertyChanged
    {

        #region Fields
        CommandList commands = new CommandList();
        #endregion

        public List<ISettingsViewModel> ChildViewModels { get; set; }

        #region Bindings


        #region SelectedTab 

        private int selectedTab;

        public int SelectedTab
        {
            get
            {
                return selectedTab;
            }

            set
            {
                if (value != selectedTab)
                {
                    selectedTab = value;
                    // OnSelectedTabChanged();
                    NotifyPropertyChanged(nameof(SelectedTab));
                }
            }
        }

        #endregion


        #endregion

        #region Commands

        public ICommand BrowserFolders { get; }

        public ICommand RestoreDefaultValuesCommand { get; set; }
        public ICommand SaveCommand { get; }

        #endregion


        public SettingsViewModel()
        {
            ChildViewModels = new List<ISettingsViewModel>();

            GeneralViewModel generalViewModel = new GeneralViewModel();
            ChildViewModels.Add(generalViewModel);

            ILSConfigurationViewModel ilsConfViewModel = new ILSConfigurationViewModel();
            ChildViewModels.Add(ilsConfViewModel);

            RestoreDefaultValuesCommand = commands.AddCommand(ExecuteRestoreDefaultValues, CanExecuteRestoreDefaultValues);
            SaveCommand = commands.AddCommand(ExecuteSaveCommand, CanExecuteSaveCommand);
        }



        #region CommandImplementations

        private void ExecuteSaveCommand()
        {
            foreach (ISettingsViewModel settingsViewModel in ChildViewModels)
            {
                settingsViewModel.ExecuteSave();
            }
        }

        private bool CanExecuteSaveCommand()
        {
            return true;
        }


        private void ExecuteRestoreDefaultValues()
        {
            if (SelectedTab >= 0 && SelectedTab < ChildViewModels.Count)
                ChildViewModels[SelectedTab].ExecuteRestoreDefaultValues();
        }


        private bool CanExecuteRestoreDefaultValues()
        {
            return true;
        }


        #endregion


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
