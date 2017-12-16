using API_DARP.Data.Model.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DARP.Data.Model.Summary
{
    [Serializable]
    public class SummaryDetails
    {
        public List<ILSEvolution> ILSEvolution { get; set; } = new List<Summary.ILSEvolution>();

        public List<VNSOperators> VNSOperators { get; set; } = new List<VNSOperators>();

        public ILSSummary ILSSummary { get; set; } = new ILSSummary();

    }
}
