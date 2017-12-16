using API.DARP.Data.Error;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARP.Controllers
{
    public interface IDialogController
    {
        void ShowError(ErrorInfo error, Exception exception);
    }
}
