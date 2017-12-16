using API.DARP.Calculations.Metaheuristics.ILS;
using API.DARP.Data.Model;
using API.DARP.Data.Model.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DARP.Calculations.Metaheuristics
{
    internal class MetaheuristicFactory
    {
        public static IMetaheuristic CreateMetaheuristic(Constants.TypeMetaheuristics type, IHeuristicConfigurationSetting settings, Problem problem, Random random)
        {
            switch (type)
            {
                case Constants.TypeMetaheuristics.ILS:
                    return new ILSMetaheuristic(settings, problem, random);
                default:
                    return new ILSMetaheuristic(settings, problem, random);
            }

        }
    }
}
