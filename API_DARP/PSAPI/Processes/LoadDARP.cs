using API.DARP.API;
using API.DARP.Data.Model;
using API.DARP.Data.Readers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DARP.PSAPI.Processes
{
    public class LoadDARPInput
    {
        public string[] Files { get; set; }
    }

    public class LoadDARPOutput
    {
        public InputDataLoaded DataLoaded { get; set; }
    }


    public interface ILoadDARP: IProcess<LoadDARPInput, LoadDARPOutput>
    {

    }

    public class LoadDARP : ILoadDARP
    {        
        
        DARPReader reader = Controllers.Instance.Specific.Data.Readers.DARPReader; 

        public LoadDARPOutput Execute(LoadDARPInput parameters)
        {
            LoadDARPOutput output = new LoadDARPOutput();
            output.DataLoaded = reader.ReadInputProblems(parameters.Files);
            return output;

        }
    }
}
