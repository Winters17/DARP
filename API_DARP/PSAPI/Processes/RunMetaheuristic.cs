using API.DARP.API;
using API.DARP.Calculations;
using API.DARP.Data.Model;
using API.DARP.Data.Model.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DARP.PSAPI.Processes
{
    public class RunMetaheuristicInput
    {
        public List<Problem> Problems { get; set; }

        public IHeuristicConfigurationSetting HeuristicSettings { get; set; }

        public Random Random { get; set; }

    }

    public class RunMetaheuristicOutput
    {
        public List<Solution> Solutions { get; set; }
    }


    public interface IRunMetehauristic : IProcess<RunMetaheuristicInput,RunMetaheuristicOutput>
    {

    }


    public class RunMetaheuristic : IRunMetehauristic
    {
        public RunMetaheuristicOutput Execute(RunMetaheuristicInput parameters)
        {
            RunMetaheuristicOutput output = new RunMetaheuristicOutput();
            output.Solutions = DARPOptimization.Optimization(parameters);

            return output;
        }
    }
}
