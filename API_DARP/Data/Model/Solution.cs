using API.DARP.Calculations.Algorithms;
using API.DARP.Calculations.Metaheuristics;
using API.DARP.Data;
using API.DARP.Data.Model.Summary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DARP.Data.Model
{

    [Serializable]
    public class Solution
    {
        public Solution(string id)
        {
            this.ID_DARP = id;
            SummaryDetails = new SummaryDetails();
        }

        public double Parc_Cost { get; set; }

        public Solution InitialSolution { get; set; }

        public String ID_DARP { get; private set; }

        public double Fitness { get; set; }

        public bool Feasible { get; set; } = false;


        //Sum durations.
        public double TotalDuration { get; set; }

        public double TotalWaitingTime { get; set; }

        public double TotalTransitTime { get; set; }



        //Constraints
        /// <summary>
        /// Establece, para soluciones no factibles, la duración máxima excedida.
        /// </summary>
        public double DurationViolation { get; set; }

        public double TimeWindowViolation { get; set; }

        public double RideTimeViolation { get; set; }

        public int LoadViolation { get; set; }

        public double ExecutionTime { get; set; }

        public SummaryDetails SummaryDetails { get; set; }


        public List<Vehicle> Vehicles { get; set; } = new List<Vehicle>();

        internal void AddVehicles(List<Vehicle> vehicles, Problem problem)
        {
            //Copiar los vehículos y añadir depósitos al inicio y al final
            foreach(var vehicle in vehicles)
            {
                var depot = problem.Requests.Find(t=>t.ID_Unique==0);
                vehicle.Requests.Insert(0, GenericCopier<Request>.DeepCopy(depot));
                depot.Origin = false;
                vehicle.Requests.Add(GenericCopier<Request>.DeepCopy(depot));
            }

            Vehicles = vehicles.ToList();
        }

        public void MarkAllNotPerturbed()
        {
            this.Vehicles = this.Vehicles.Select(x => { x.Perturbed = true; return x; }).ToList();
        }

    }
}
