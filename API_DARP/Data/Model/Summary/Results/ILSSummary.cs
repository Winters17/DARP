using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_DARP.Data.Model.Results
{
    [Serializable]
    public class ILSSummary
    {

        public double InitCost { get; set; }

        public double FinalCost { get; set; }

        public int? FirstItFeasible { get; set; } = null;


        public int BestIteration { get; set; }

        public int TotalImpBest { get; set; }

        public int TotalIterations { get; set; }

        public int TotalImpPrevious { get; set; }


        //public ILSSummary(double ic, double fc, int fif, int bi, int tib, int ti, int tip)
        //{
        //    this.InitCost = ic;
        //    this.FinalCost = fc;
        //    this.FirstItFeasible = fif;
        //    this.BestIteration = bi;
        //    this.TotalImpBest = tib;
        //    this.TotalIterations = ti;
        //    this.TotalImpPrevious = tip;
        //}
    }
}
