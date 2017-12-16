using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DARP.Data.Model
{
    [Serializable]
    public class RequestTimeWindow
    {
        public double LowerBound { get; set; }

        public double UpperBound { get; set; }


        public RequestTimeWindow(int lb, int ub)
        {
            this.LowerBound = lb;
            this.UpperBound = ub;
        }
    }
}
