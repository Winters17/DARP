

using API.DARP.PSAPI.Processes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DARP.PSAPI
{
    public class ProcessController
    {
        public class SpecificController
        {
            public ILoadDARP LoadDARP { get; } = new LoadDARP();

            public IRunMetehauristic RunMetaheuristic { get; } = new RunMetaheuristic();

            public IExportCSV ExportCSV { get; set; } = new ExportCSV();
        }

        public static ProcessController Instance => instance ?? (instance = new ProcessController());
        private static ProcessController instance;

        public SpecificController Specific { get; }
        public ProcessController()
        {
            Specific = new SpecificController();
        }
    }
}
