using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DARP.Calculations
{
    internal static class Constants
    {
        //Neighborhood Names
        internal const string SWAP1_1_INTER = "SWAP1_1";
        internal const string SHIFT1_0_INTER = "SHIFT1_0";
        internal const string TW_MV_INTRA = "TW_MV_INTRA";
        internal const string WRI_INTRA = "WRI_Intra";
        internal const string SWAP_INTRA = "SWAP_INTRA";
        internal const string SHIFT1_0_INTRA_OPT = "SHIFT1_0_INTRA_OPT";
    

        internal enum TypeMetaheuristics { ILS, NONE };


        public static string SPLASH_NAME_PROBLEM = "SPLASH_NAME_PROBLEM";
        public static string SPLASH_NUMBER_REPETITION = "SPLASH_NUMBER_REPETITION";
        public static string SPLASH_PROGRESS = "SPLASH_PROGRESS";
        public static string SPLASH_ILS_IMPROVEMENTS = "SPLASH_ILS_IMPROVEMENTS";
        public static string SPLASH_ILS_TOTAL_ITERATIONS = "SPLASH_TOTAL_ITERATIONS";
        public static string SPLASH_BEST_SOLUTION = "SPLASH_BEST_SOLUTION";
        public static string EXCEPTION_TITLE = "Exception";
        public static string ERROR_TITLE = "Error";
    }
}
