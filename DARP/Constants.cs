using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARP
{
    public static partial class Constants
    {

        private static string LAST_EXECUTION_CONFIGURATIONS_SETTINGS_FILE_NAME = @"lastExecution.savesettings";

        public enum StatusType
        {
            None,
            Ok,
            Warning,
            Error
        }


        public static string ConfigurationFolder
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DARP");
            }
        }

        public static string SpecificLastExecutionPath
        {
            get
            {
                return Path.Combine(ConfigurationFolder, Constants.LAST_EXECUTION_CONFIGURATIONS_SETTINGS_FILE_NAME);
            }
        }


        public static List<string> ColumnTableRoutesNames()
        {
            return new List<string>
            {
                "Vehicle",
                "Route",
                "Route duration",
            };
        }

        public static List<string> Details()
        {
            return new List<string>
            {
                "Summary",
                "ILS Evolution",
                "VNS Operators",
                "Charts"

            };
        }

        public static List<string> ColumnsILSEvolution()
        {                
            return new List<string>
            {
                "Iteration",
                "Init Cost",
                "Init Fitness",
                "VNS Cost",
                "VNS Fitness",
                "Feasible",
                "Perturbation",
                "Perturbation Cost",
                "Perturbation Fitness",
            };
         }

        public static List<string> ColumnsILSSummary()
        {
            return new List<string>
            {
                "Name",
                "Value",
            };
        }

        public static List<string> ColumnsVNSOperators()
        {
            return new List<string>
            {
                "Iteration",
                "Shift_Inter",
                "Swap_Inter",
                "TwoOpt_Intra",
                "Shift_Intra",
                "Swap_Intra",
                "Total Changes",
            };
        }

        public const string EXCEPTION_TITLE = "Exception";
        public const string ERROR_TITLE = "Error";
    }
}
