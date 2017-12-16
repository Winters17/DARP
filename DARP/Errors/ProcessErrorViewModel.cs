using API.DARP;
using API.DARP.Data.Error;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARP.Errors
{
    public class ProcessErrorViewModel : INotifyPropertyChanged
    {

        #region Bindings
        #region ErrorTitle 

        private string errorTitle;

        public string ErrorTitle
        {
            get
            {
                return errorTitle;
            }

            set
            {
                if (value != errorTitle)
                {
                    errorTitle = value;
                    // OnErrorTitleChanged();
                    NotifyPropertyChanged(nameof(ErrorTitle));
                }
            }
        }

        #endregion

        #region ErrorMessage 

        private string errorMessage;


        public string ErrorMessage
        {
            get
            {
                return errorMessage;
            }

            set
            {
                if (value != errorMessage)
                {
                    errorMessage = value;
                    // OnErrorMessageChanged();
                    NotifyPropertyChanged(nameof(ErrorMessage));
                }
            }
        }

        #endregion

        #region Warning 

        private string warning;

        public string Warning
        {
            get
            {
                return warning;
            }

            set
            {
                if (value != warning)
                {
                    warning = value;
                    // OnWarningChanged();
                    NotifyPropertyChanged(nameof(Warning));
                }
            }
        }

        #endregion


        #endregion


        public ProcessErrorViewModel()
        {
            ErrorTitle = "Process error";
            ErrorMessage = String.Empty;
            Warning = String.Empty;
        }



        #region PublicMethods

        internal void SetError(ErrorInfo error, Exception exception = null)
        {
            if (exception != null)
            {
                ErrorTitle = exception.Message;
                ErrorMessage =  exception.StackTrace;
                Warning = Constants.EXCEPTION_TITLE;
            }
            else
            {
                ErrorTitle = error.Title;
                ErrorMessage = error.Message;
                Warning = Constants.ERROR_TITLE;
            }
        }


        #endregion

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
