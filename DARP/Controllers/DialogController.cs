using API.DARP.Data.Error;
using DARP.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DARP.Controllers
{
    public class DialogController : IDialogController
    {
        private Window MainWindow => Application.Current.Windows.Cast<Window>().FirstOrDefault(w => w.IsActive) ?? Application.Current.MainWindow;

        public void ShowError(ErrorInfo error, Exception exception)
        {
            var window = new ProcessErrorWindow();

            var vm = new ProcessErrorViewModel();

            vm.ErrorTitle = String.IsNullOrEmpty(error.Title) ? "Process Error" : error.Title;

            if (exception == null)
                vm.SetError(error);
            else
                vm.SetError(error, exception);

            window.DataContext = vm;

            if (this.MainWindow.IsVisible)
                window.Owner = this.MainWindow;
            window.ShowDialog();
            window.DataContext = null;
        }


    }
}
