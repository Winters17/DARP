using API.DARP.Data.Model;
using API.DARP.Data.Model.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DARP.Calculations.Metaheuristics
{
    internal interface IMetaheuristic
    {

        IHeuristicConfigurationSetting HeuristicSettings { get; set; }

        void ExecuteMetaheuristic(ref Solution solution);
        

        
    }
}
