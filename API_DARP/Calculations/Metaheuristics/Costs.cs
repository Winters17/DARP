using API.DARP.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DARP.Calculations.Metaheuristics
{
    public class Costs
    {

        internal static double SingleEvaluation(Vehicle vehicle, Solution solution, Problem problem)
        {
            var fCost = 0.0;


            for (int i = 1; i < vehicle.Requests.Count; i++)
            {
                fCost += problem.Distances[vehicle.Requests[i - 1].ID_Unique][vehicle.Requests[i].ID_Unique];
            }

            //Evaluación función de coste  + cómputo de penalizaciones.
            var fitness = (fCost + (Singleton.Instance.Alpha * solution.LoadViolation) + (Singleton.Instance.Beta * solution.DurationViolation) + (Singleton.Instance.Gamma * solution.TimeWindowViolation) + (Singleton.Instance.Rho * solution.RideTimeViolation));
            return fitness;
        }

        internal static double ParcialCostSolution(Solution solution, Problem problem)
        {
            var fCost = 0.0;

            foreach (var vehicle in solution.Vehicles)
            {
                for (int i = 1; i < vehicle.Requests.Count; i++)
                {
                    fCost += problem.Distances[vehicle.Requests[i - 1].ID_Unique][vehicle.Requests[i].ID_Unique];
                }
            }

            //Evaluación función de coste  + cómputo de penalizaciones.
            return (fCost);
        }

        internal static double EvaluationSingleRoute(Vehicle vehicle, Solution solution, Problem problem)
        {
            var fCost = 0.0;

            for (int i = 1; i < vehicle.Requests.Count; i++)
            {
                fCost += problem.Distances[vehicle.Requests[i - 1].ID_Unique][vehicle.Requests[i].ID_Unique];
            }

             //Evaluación función de coste  + cómputo de penalizaciones.
            return (fCost + (Singleton.Instance.Alpha * vehicle.TotalLoadViolation) + (Singleton.Instance.Beta * vehicle.TotalDurationViolation) + (Singleton.Instance.Gamma *vehicle.TotalTimeWindowViolation) + (Singleton.Instance.Rho * vehicle.TotalRideTimeViolation));
        }


        internal static double EvaluationSolution(Solution solution, Problem problem)
        {
            var fCost = 0.0;

            foreach (var vehicle in solution.Vehicles)
            {
                for (int i = 1; i < vehicle.Requests.Count; i++)
                {
                    fCost += problem.Distances[vehicle.Requests[i - 1].ID_Unique][vehicle.Requests[i].ID_Unique];
                }
            }

            solution.Parc_Cost = fCost;
            //Evaluación función de coste  + cómputo de penalizaciones.
            var fitness = (fCost + (Singleton.Instance.Alpha * solution.LoadViolation) + (Singleton.Instance.Beta * solution.DurationViolation) + (Singleton.Instance.Gamma * solution.TimeWindowViolation) + (Singleton.Instance.Rho * solution.RideTimeViolation));
            return fitness;
        }


        internal static double EvaluationSolution(ref Solution solution, double timeWVio, double rideTimeViol, double tDurViol, int tLoadViol, Problem problem)
        {
            var fCost = 0.0;

            foreach (var vehicle in solution.Vehicles)
            {
                for (int i = 1; i < vehicle.Requests.Count; i++)
                {
                    fCost += problem.Distances[vehicle.Requests[i - 1].ID_Unique][vehicle.Requests[i].ID_Unique];
                }
            }

            solution.Parc_Cost = fCost;
            //Evaluación función de coste  + cómputo de penalizaciones.
            var fitness = (fCost + (Singleton.Instance.Alpha * tLoadViol) + (Singleton.Instance.Beta * tDurViol) + (Singleton.Instance.Gamma * timeWVio) + (Singleton.Instance.Rho * rideTimeViol));
            return fitness;
        }
    }


    public class Singleton
    {

        private static PenaltyTerms instance;

        public static PenaltyTerms Instance
        {
            get
            {
                if (instance == null)
                    instance = new PenaltyTerms();
                return instance;
            }
        }

    }

    public class PenaltyTerms
    {
        private const double EPS = 1.5;
        public PenaltyTerms()
        {
            Alpha = 1;
            Beta = 1;
            Gamma = 1;
            Rho = 1;
        }

        public double Alpha { get; set; } //Penalizes load violation.

        public double Beta { get; set; } //Penalizes duration violation.

        public double Gamma { get; set; } //Penalizes time window violation.

        public double Rho { get; set; } //Penalizes ride time violation.


        //instance methods
        internal void UpdatePenaltyTerms(Solution solution)
        {
            Random random = new Random();
            if (solution.LoadViolation > 0) //Update Alpha
            {
                Alpha = Alpha * EPS;
            }
            else
            {
                Alpha = Alpha / EPS;
            }
            if (Math.Round(solution.DurationViolation, 2) > 0) //Update Beta
            {
                Beta = Beta * EPS;
            }
            else
            {
                Beta = Beta / EPS;
            }
            if (Math.Round(solution.TimeWindowViolation, 2) > 0) //Update Gamma
            {
                Gamma = Gamma * EPS;
            }
            else
            {
                Gamma = Gamma / EPS;
            }
            if (Math.Round(solution.RideTimeViolation, 2) > 0) //Update Rho
            {
                Rho = Rho * EPS;
            }
            else
            {
                Rho = Rho / EPS;
            }

            if (Alpha > 10) Alpha = 10;
            else if (Alpha < 1) Alpha = 1;
            if (Beta > 10) Beta = 10;
            else if (Beta < 1) Beta = 1;
            if (Gamma > 10) Gamma = 10;
            else if (Gamma < 1) Gamma = 1;
            if (Rho > 10) Rho = 10;
            else if (Rho < 1) Rho = 1;

        }

        internal void UpdatePenaltyTerms2(Solution solution)
        {
            Random random = new Random();
            if (solution.LoadViolation > 0) //Update Alpha
            {
                Alpha = Alpha * (1 + ((double)random.Next(5, 10) / (100)));
            }
            else
            {
                Alpha = Alpha / (1 + ((double)random.Next(5, 10) / (100)));
            }
            if (Math.Round(solution.DurationViolation, 2) > 0) //Update Beta
            {
                Beta = Beta * (1 + ((double)random.Next(5, 10) / (100)));
            }
            else
            {
                Beta = Beta / (1 + ((double)random.Next(5, 10) / (100)));
            }
            if (Math.Round(solution.TimeWindowViolation, 2) > 0) //Update Gamma
            {
                Gamma = Gamma * (1 + ((double)random.Next(5, 10) / (100)));
            }
            else
            {
                Gamma = Gamma / (1 + ((double)random.Next(5, 10) / (100)));
            }
            if (Math.Round(solution.RideTimeViolation, 2) > 0) //Update Rho
            {
                Rho = Rho * (1 + ((double)random.Next(5, 10) / (100)));
            }
            else
            {
                Rho = Rho / (1 + ((double)random.Next(5, 10) / (100)));
            }

        }
    }
}
