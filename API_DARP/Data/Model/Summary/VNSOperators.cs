using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DARP.Data.Model.Summary
{
    [Serializable]
    public class VNSOperators
    {
        public VNSOperators(int it, int shiftInter, int swapInter, int wRI_Intra, int intraRouteInsertion, int swapIntra)
        {
            this.Iteration = it;
            this.Shift1_0_Inter = shiftInter;
            this.Swap1_1_Inter = swapInter;
            this.WRI_Intra = wRI_Intra;
            this.IntraRouteInsertion = intraRouteInsertion;
            this.Swap1_1_Intra = swapIntra;
            this.TotalChanges = Shift1_0_Inter + Swap1_1_Inter + WRI_Intra + IntraRouteInsertion + Swap1_1_Intra;
        }

        public int Iteration { get; set; }


        public int Shift1_0_Inter { get; set; }

        public int Swap1_1_Inter { get; set; }

        public int WRI_Intra { get; set; }

        public int IntraRouteInsertion { get; set; }

        public int Swap1_1_Intra { get; set; }

        public int TotalChanges { get; set; }
    }
}
