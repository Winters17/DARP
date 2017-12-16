using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_DARP.Data.Model.Results
{
    [Serializable]
    public class VNSSummary
    {
        public int ImpInterShift { get; set; }

        public int ImpInterSwap { get; set; }

        public int ImpIntraShift { get; set; }

        public int ImpIntraSwap { get; set; }

        public int ImpTwoOPT { get; set; }

        public int TotalImprovements { get; set; }

        //public VNSSummary(int interShift, int interSwap, int intraShift, int intraSwap, int twoopt)
        //{
        //    this.ImpInterShift = interShift;
        //    this.ImpInterSwap = interSwap;
        //    this.ImpIntraShift = intraShift;
        //    this.ImpIntraSwap = intraSwap;
        //    this.ImpTwoOPT = twoopt;
        //}
    }
}
