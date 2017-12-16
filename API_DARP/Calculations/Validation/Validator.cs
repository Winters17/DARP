using API.DARP.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DARP.Calculations.Validation
{
    internal class Validator
    {

        private static double _error = 0.01;

        internal static void Positions(ref Solution solution, Problem problem)
        {
            foreach (var vehicle in solution.Vehicles)
            {
                for (int i = 1; i < vehicle.Requests.Count - 2; i++)
                {
                    if (vehicle.Requests[i].Origin)
                    {
                        if (vehicle.Requests.Skip(i + 1).ToList().Find(t => t.PairID == vehicle.Requests[i].PairID && t.Origin == false) == null)
                            Console.WriteLine();
                       
                     }
                }
            }
        }

        internal static void ValidateSolution(ref Solution solution, Problem problem)
        {

            //Comprobar sobre cada petición posibles errores.
            foreach(var vehicle in solution.Vehicles)
            {
                for(int i=1; i< vehicle.Requests.Count-2; i++)
                {
                    if (vehicle.Requests[vehicle.Requests.Count - 1].PairID != 0)
                        Console.WriteLine();// throw new Exception("Mal asunto.");
                    //Time window constraints
                    if (vehicle.Requests[i].IsCritical && vehicle.Requests[i].StartService > vehicle.Requests[i].TimeWindow.UpperBound)
                    {
                        //Existe violación de time window. Comprobar si está computada y si no lanzar una excepción.
                        if (Math.Abs(Math.Round(vehicle.Requests[i].TimeWindowViolation,2) - (Math.Round(vehicle.Requests[i].StartService - vehicle.Requests[i].TimeWindow.UpperBound,2))) > _error)
                        {
                            //No coincide time window violation
                            throw new Exception("Error checking time window violation");
                        }
                    }
                    //Ride time constraints
                    var origin = vehicle.Requests[i].ID_Unique > problem.TotalCustomers ? vehicle.Requests.Find(t => t.ID_Unique == vehicle.Requests[i].ID_Unique - problem.TotalCustomers) : vehicle.Requests[i];
                    var destination = vehicle.Requests[i].ID_Unique > problem.TotalCustomers ? vehicle.Requests[i] : vehicle.Requests.Find(t => t.ID_Unique == vehicle.Requests[i].ID_Unique + problem.TotalCustomers);

                    var rideTime = Math.Round(destination.StartService - origin.DepartureTime, 2);
                    if (Math.Abs(Math.Round(rideTime, 2) - Math.Round(origin.RideTime,2)) > _error)
                    {
                        //NO coincide el ride time. 
                        // throw new Exception("Error checking ride time");
                        Console.WriteLine();
                    }
                    rideTime = Math.Round(rideTime - problem.MaxRideTime,2) > 0 ? Math.Round(rideTime - problem.MaxRideTime, 2) : 0;
                    if (rideTime > 0 && Math.Abs(rideTime - Math.Round(origin.RideTimeViolation,2)) > _error)
                    {
                        {
                            //NO coincide el ride time. 
                            throw new Exception("Error checking ride time violation");
                        }
                    }
                }

            }
        }

    }
}
