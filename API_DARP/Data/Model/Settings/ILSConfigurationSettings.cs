using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DARP.Data.Model.Settings
{
    [Serializable]
    public class ILSConfigurationSettings : IHeuristicConfigurationSetting
    {

        public int MaxRepetitions { get; set; }

        public int MaxILSIterations { get; set; }

        public int MaxILSNoImprovement { get; set; }



        public ILSConfigurationSettings()
        {

        }

        public ILSConfigurationSettings(ILSConfigurationSettings settings)
        {
            this.MaxILSIterations = settings.MaxILSIterations;
            this.MaxRepetitions = settings.MaxRepetitions;
            this.MaxILSNoImprovement = settings.MaxILSNoImprovement;
        }


        public static ILSConfigurationSettings CreateDefaults()
        {
            return new ILSConfigurationSettings { MaxRepetitions = 5, MaxILSIterations = 500, MaxILSNoImprovement = 100 };
        }

    }
}
