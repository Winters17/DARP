using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DARP.Data.Model.Charts
{
    public class ChartSerie<T>
    {
        public T X { get; set; }

        public double Y { get; set; }
    }
}
