using API.DARP.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARP
{
    internal class Context
    {

        #region Singleton

        private static Context instance = new Context();
        public static Context Instance { get { return instance; } }

        #endregion

        #region Constructor
        private Context()
        {
            this.Settings = new Settings();
        }
        #endregion

        #region Properties

        public Settings Settings { get; set; }

        public List<Solution> Solutions { get; set; }

        public List<Problem> Problems { get; set; }

        #endregion

    }
}
