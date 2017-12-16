using API.DARP.Calculations.Metaheuristics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DARP.Data.Model
{

    [Serializable]
    public class Vehicle
    {
        public List<Request> Requests { get; set; } = new List<Request>();

        public bool Perturbed { get; set; } = true;

        public int Capacity { get; set; }
        

        public double VehicleDuration { get; set; }

        public double TotalDurationViolation { get; set; }

        public double TotalTimeWindowViolation { get; set; }

        public double TotalRideTimeViolation { get; set; }

        public int TotalLoadViolation { get; set; }

        public double TotalWaitingTime { get; set; }


        internal string GetString()
        {
            return String.Join(" - ", Requests.Select(t => t.PairID));
        }

    }
}
