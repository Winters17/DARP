using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DARP.Data.Model.Settings
{
    public interface IHeuristicConfigurationSetting 
    {
        int MaxRepetitions { get; set; }
    }
}
