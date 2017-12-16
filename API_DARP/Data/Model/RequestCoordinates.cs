using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DARP.Data.Model
{
    [Serializable]
    public class RequestCoordinates
    {
        public double AxisX { get; set; }

        public double AxisY { get; set; }

        public RequestCoordinates(double ax, double ay)
        {
            this.AxisX = ax;
            this.AxisY = ay;
        }
    }
}
