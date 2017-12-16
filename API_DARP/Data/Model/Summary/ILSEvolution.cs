using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DARP.Data.Model.Summary
{
    [Serializable]
    public class ILSEvolution
    {
        public ILSEvolution(int it, double ic, double itf, double vnsc, double vnsf, int perturbation, double pf, double pc, bool feasible)
        {
            this.Iteration = it;
            this.InitCost = ic;
            this.InitFitness = itf;
            this.VNSCost = vnsc;
            this.VNSFitness = vnsf;
            switch(perturbation)
            {
                case 0:
                    PerturbationType = "Shift";
                    break;
                case 1:
                    PerturbationType = "Chain";
                    break;
                case 2:
                    PerturbationType = "Opt";
                    break;
                case 3:
                    PerturbationType = "Inverse";
                    break;
            }
            this.PertFitness = pf;
            this.PertCost = pc;
            this.Feasible = feasible.ToString();
        }

        public int Iteration { get; set; }

        public double InitCost { get; set; }

        public double InitFitness { get; set; }

        public double VNSCost { get; set; }

        public double VNSFitness { get; set; }

        public string PerturbationType { get; set; }

        public double PertFitness { get; set; }

        public double PertCost { get; set; }

        public string Feasible { get; set; }

    }
}
