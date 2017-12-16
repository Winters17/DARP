using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DARP.Data
{
    public static partial class Constants
    {

        public const string TABLE_INPUT = "Problems";

        public static List<string> ColumnsInputProblems()
        {
            return new List<string>
                    {
                         "Name",
                         "Best Known",
                         "N# Vehicles",
                         "N# Requests",
                         "Max Vehicle Distance",
                         "Max Load",
                         "Max Ride Time",
                         "Service Time"
                    };
        }

        public static Dictionary<string, double> BestKnownProblem()
        {
            return new Dictionary<string, double>
            {
                {"pr01", 190.02},
                {"pr02", 302.08},
                {"pr03", 532.08},
                {"pr04", 572.78},
                {"pr05", 636.97},
                {"pr06", 801.40},
                {"pr07", 291.71},
                {"pr08", 494.89},
                {"pr09", 672.44},
                {"pr10", 878.76},
                {"pr11", 164.46},
                {"pr12", 296.06},
                {"pr13", 493.30},
                {"pr14", 535.90},
                {"pr15", 589.74},
                {"pr16", 743.60},
                {"pr17", 248.21},
                {"pr18", 462.69},
                {"pr19", 601.96},
                {"pr20", 798.63},
            };
        }


        public static string DEFAULT_HEURISTIC { get; } = "ILS";

        public static List<string> ListHeuristics { get;} = new List<string>() { "ILS" };

    }
}
