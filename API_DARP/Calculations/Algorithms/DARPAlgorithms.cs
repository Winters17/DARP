using API.DARP.Calculations.Metaheuristics;
using API.DARP.Calculations.Validation;
using API.DARP.Data;
using API.DARP.Data.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static API.DARP.Data.Model.Request;

namespace API.DARP.Calculations.Algorithms
{
    public class DARPAlgorithms
    {


        /// <summary>
        /// Método principal para la construcción de úna solución DARP inicial.
        /// </summary>
        /// <param name="solution"></param>
        /// <param name="problem"></param>
        internal static void BuildInitialSolution(ref Solution solution, Problem problem, ref Random _random)
        {
            var upperT = 0.0;
            var lowerT = 0.0;
            var diff = 0.0;
            foreach (var request in problem.Requests)
            {
                if (request.ID_Unique == 0) continue;
                if (request.IsCritical)
                {
                    upperT += request.TimeWindow.UpperBound;
                    lowerT += request.TimeWindow.LowerBound;
                    diff += (request.TimeWindow.UpperBound - request.TimeWindow.LowerBound);
                }
            }

            //Ordenar los n clientes (Total REquests / 2) por su media de tiempo de ventana.
            var requestsOrder = problem.Requests.Where(t => !t.RequestType.Equals(Request.TypeOfRequest.DEPOT)).Take(problem.TotalCustomers).OrderBy(t => (t.TimeWindow.LowerBound + t.TimeWindow.UpperBound) / 2).ToList();
            requestsOrder = requestsOrder.GetRange(0, requestsOrder.Count); //Coger los N clientes, omitiendo el depósito.

            //Inicializar matriz de arcos factibles.
            //problem.CalculateAvgTimeWindow(true);
            problem.CalculateFeasibleArcs(0);


            //Crear vehículos.
            List<Vehicle> vehicles = new List<Vehicle>();
            int i;
            for (i = 0; i < problem.NumVehicles; i++)
            {
                vehicles.Add(new Vehicle());
            }

            var index = 0;
            foreach (var request in requestsOrder)
            {
                if (index < problem.NumVehicles)
                {
                    //Añadir los m primeros uno en cada vehículo..
                    request.RequiredLoad = 1;
                    vehicles[index].Requests.Add(request); //Inicio petición.
                    var destination = problem.Requests.Find(t => t.PairID == request.PairID && t.Origin != request.Origin);
                    vehicles[index].Requests.Add(destination); //Se añade también el final de la petición.
                    index++;
                }
                else
                {
                    //Los n - m restantes se irán insertando por cercanía en base a la regla de los 4 criterios.
                    InsertRequestBy4Criterion(ref solution, vehicles, request, problem, ref _random);
                }
            }

            solution.AddVehicles(vehicles, problem);

            foreach (var vehicle in solution.Vehicles)
                EightStepsEvaluationProcedure(ref solution, vehicle, problem);

            //Actualizar estado de la solución.
            solution.TotalDuration = solution.Vehicles.Sum(t => t.VehicleDuration);
            solution.TimeWindowViolation = solution.Vehicles.Sum(t => t.TotalTimeWindowViolation);
            solution.RideTimeViolation = solution.Vehicles.Sum(r => r.TotalRideTimeViolation);
            solution.DurationViolation = solution.Vehicles.Sum(d => d.TotalDurationViolation);
            solution.LoadViolation = solution.Vehicles.Sum(l => l.TotalLoadViolation);
            solution.TotalWaitingTime = solution.Vehicles.Sum(t => t.TotalWaitingTime);
            solution.Fitness = Costs.EvaluationSolution(solution, problem);

            PrintDataCSV(problem);
        }

        private static void PrintDataCSV(Problem problem)
        {
            var requests = problem.Requests;
            var column = requests.Select(t => t.ID_Unique).ToList();
            var rows = requests.Select(t => t.ID_Unique).ToList();

            using (StreamWriter sw = new StreamWriter("prueba.csv", false))
            {
                StringBuilder sRow = new StringBuilder();
                var cadena = ",";
                for (int i = 0; i < column.Count(); i++)
                {
                    cadena += column[i].ToString() + ",";
                }
                //sRow.AppendLine(cadena);
                sw.WriteLine(cadena);


                for (int i = 0; i < column.Count(); i++)
                {
                    var cad = rows[i].ToString() + ",";
                    for (int j = 0; j < rows.Count; j++)
                    {
                        cad += problem.Distances[column[i]][rows[j]] + ",";
                    }
                    //sRow.AppendLine(cad);
                    sw.WriteLine(cad);
                }



            }

            using (StreamWriter sw = new StreamWriter("tiemposVentana.csv", false))
            {
                StringBuilder sRow = new StringBuilder();
                var cadena = ",";
                for (int i = 0; i < column.Count(); i++)
                {
                    cadena = column[i].ToString() + " , " + problem.Requests[column[i]].TimeWindow.LowerBound +" , "+ problem.Requests[column[i]].TimeWindow.UpperBound;
                    sw.WriteLine(cadena);
                }
            }

            ////Distancias para el problema 7.
            //Debug.WriteLine(problem.Distances[0][4] +","+ problem.Distances[4][5] + "," + problem.Distances[5][41] + "," + problem.Distances[41][12] + "," + problem.Distances[12][40] + "," + problem.Distances[40][48] + "," +
            //    problem.Distances[48][19] +","+ problem.Distances[19][36] +","+ problem.Distances[36][2] + "," + problem.Distances[2][38] + "," + problem.Distances[38][18] + "," +
            //    problem.Distances[18][20] +","+ problem.Distances[20][55] + "," + problem.Distances[55][72] + "," + problem.Distances[72][3] + "," + problem.Distances[3][56] + "," + problem.Distances[56][54] + "," +
            //    problem.Distances[54][39] + "," + problem.Distances[39][35] + "," + problem.Distances[35][71] + "," + problem.Distances[71][31] + "," + problem.Distances[31][67] + "," + problem.Distances[67][0]);


        }



        /// <summary>
        /// Inserta una petición completa a través de uno de los 4 criterios de inserción, elegidos aleatoriamente.
        /// </summary>
        /// <param name="vehicles">Listado de vehículos actuales</param>
        /// <param name="request">Petición i</param>
        /// <param name="problem">Problema actual</param>
        private static void InsertRequestBy4Criterion(ref Solution solution, List<Vehicle> vehicles, Request request_Source, Problem problem, ref Random _random)
        {
            var criterio = _random.Next(0, 4);
            double minimun = double.MaxValue;
            int best_Vehicle = 0;
            int c_Vehicle = 0;

            //Obtener la localidad destino de la petición.
            Request request_Target = problem.Requests.Find(t => t.PairID.Equals(request_Source.PairID) && t.Origin != request_Source.Origin);
            switch (criterio)
            {
                case 0: //Criterio A. Distancia del origen de la última petición insertada al origen de la petición a insertar.
                        //Cálculos
                    foreach (var vehicle in vehicles)
                    {
                        //Obtener el origen de la última petición insertada.
                        var client = vehicle.Requests[vehicle.Requests.Count - 2];
                        if (problem.Distances[request_Source.ID_Unique][client.ID_Unique] < minimun)
                        {
                            minimun = problem.Distances[request_Source.ID_Unique][client.ID_Unique];
                            best_Vehicle = c_Vehicle;
                        }
                        c_Vehicle++;
                    }
                    break;
                case 1: //Criterio B. Distancia del origen de la última petición insertada al destino de la petición a insertar.
                    foreach (var vehicle in vehicles)
                    {
                        //Obtener el origen de la última petición insertada.
                        var client = vehicle.Requests[vehicle.Requests.Count - 2];
                        if (problem.Distances[request_Target.ID_Unique][client.ID_Unique] < minimun)
                        {
                            minimun = problem.Distances[request_Target.ID_Unique][client.ID_Unique];
                            best_Vehicle = c_Vehicle;
                        }
                        c_Vehicle++;
                    }
                    break;
                case 2: //Criterio C. Distancia del destino de la última petición insertada al origen de la petición a insertar.
                    foreach (var vehicle in vehicles)
                    {
                        //Obtener el origen de la última petición insertada.
                        var client = vehicle.Requests[vehicle.Requests.Count - 1];
                        if (problem.Distances[request_Source.ID_Unique][client.ID_Unique] < minimun)
                        {
                            minimun = problem.Distances[request_Source.ID_Unique][client.ID_Unique];
                            best_Vehicle = c_Vehicle;
                        }
                        c_Vehicle++;
                    }
                    break;
                case 3: //Criterio D. Distancia del destino de la última petición insertada al destino de la petición a insertar.
                    foreach (var vehicle in vehicles)
                    {
                        //Obtener el origen de la última petición insertada.
                        var client = vehicle.Requests[vehicle.Requests.Count - 1];
                        if (problem.Distances[request_Target.ID_Unique][client.ID_Unique] < minimun)
                        {
                            minimun = problem.Distances[request_Target.ID_Unique][client.ID_Unique];
                            best_Vehicle = c_Vehicle;
                        }
                        c_Vehicle++;
                    }
                    break;
                default: break;
            }
            request_Source.RequiredLoad = 1;
            vehicles[best_Vehicle].Requests.Add(request_Source); //Nodo Origen de la nueva solicitud.
            var destination = problem.Requests.Find(t => t.PairID.Equals(request_Source.PairID) && t.Origin != request_Source.Origin);
            vehicles[best_Vehicle].Requests.Add(destination);  //Destino de la nueva solicitud.
        }


        /// <summary>
        /// Proceso de evaluación de 8 pasos propuesto por Cordeau & Laporte.
        /// </summary>
        internal static void EightStepsEvaluationProcedure(ref Solution solution, Vehicle vehicle, Problem problem)
        {
            //Paso 1 y 2. D0 = e0 y computar Ai, Wi, Bi y Di para cada vértice en la ruta.
            bool skip = false;
            var requests = vehicle.Requests;
            //Procesar primera petición de la ruta (después del depósito).
            requests[0].DepartureTime = problem.Requests.Find(t => t.RequestType.Equals(Request.TypeOfRequest.DEPOT)).TimeWindow.LowerBound;


            for (int i = 1; i < requests.Count - 1; i++)
            {
                requests[i].Arrival = requests[i - 1].DepartureTime + problem.Distances[requests[i - 1].ID_Unique][requests[i].ID_Unique]; //Distancia de depósito al primer cliente..
                requests[i].StartService = Math.Max(requests[i].TimeWindow.LowerBound, requests[i].Arrival);
                requests[i].WaitingTime = requests[i].StartService - requests[i].Arrival;
                requests[i].DepartureTime = requests[i].StartService + requests[i].ServiceTime;
                //Actualización de la carga.
                if (requests[i].Origin)
                    requests[i].RequiredLoad = requests[i - 1].RequiredLoad + 1;
                else
                    requests[i].RequiredLoad = requests[i - 1].RequiredLoad - 1;

                //Comprobar si es necesario continuar con el proceso.
                if (requests[i].RequiredLoad > problem.MaxLoad || requests[i].StartService > requests[i].TimeWindow.UpperBound)
                    skip = true;
            }
            //Llegada de vuelta al depósito.                                             
            requests[requests.Count - 1].Arrival = requests[requests.Count - 2].DepartureTime + problem.Distances[requests[requests.Count - 2].ID_Unique][0]; //Distancia de la última petición al depósito.

            if (skip) //No continuar puesto que la ruta no es factible en relación al TW o a la carga (el algoritmo no es reparativo).
            {
                foreach (var request in vehicle.Requests)
                {
                    if (request.ID_Unique != 0 && request.Origin)
                    {
                        request.RideTime = vehicle.Requests.Find(t => t.PairID == request.PairID && t.Origin == false).StartService - request.DepartureTime;
                    }
                }
                goto Step8;
            }

            //Paso 3 y paso 4. Computar F0 y establecer D0 (mismo paso).
            // ComputeDepotSlackTime(ref solution, vehicle, problem);
            ComputeIndividualSlackTime(requests, 0, problem, solution);

            //Paso 5. Actualizar valores Ai, Wi, Bi y Di.
            for (int i = 1; i < requests.Count - 1; i++)
            {
                requests[i].Arrival = requests[i - 1].DepartureTime + problem.Distances[requests[i - 1].ID_Unique][requests[i].ID_Unique]; //Distancia de depósito al primer cliente..
                requests[i].StartService = Math.Max(requests[i].TimeWindow.LowerBound, requests[i].Arrival);
                requests[i].WaitingTime = requests[i].StartService - requests[i].Arrival;
                requests[i].DepartureTime = requests[i].StartService + requests[i].ServiceTime;
            }
            //Llegada de vuelta al depósito.                                             
            requests[requests.Count - 1].Arrival = requests[requests.Count - 2].DepartureTime + problem.Distances[requests[requests.Count - 2].ID_Unique][0]; //Distancia de la última petición al depósito.

            skip = true; //Marcar como verdadero.
            //Paso 6. Establecer valor Ride Time para cada petición de cada ruta.
            foreach (var request in vehicle.Requests)
            {
                if (request.ID_Unique != 0 && request.Origin)
                {
                    request.RideTime = vehicle.Requests.Find(t => t.PairID == request.PairID && t.Origin == false).StartService - request.DepartureTime;
                    if (request.RideTime > problem.MaxRideTime)
                        skip = false;
                }
            }

            if (skip)
                goto Step8; //Saltar puesto que todas las peticiones cumplen el ride time.


            ////Paso 7.Para cada vértice v que es un origen:
            for (int i = 1; i < requests.Count - 1; i++)
            {
                if (requests[i].Origin)
                {
                    //Computar F(v) Paso 7a.
                    ComputeIndividualSlackTime(requests, i, problem, solution);

                    //Actualizar A,W, B y D para cada vértice que viene después de i en la ruta.
                    for (int j = i + 1; j < requests.Count - 1; j++)
                    {
                        requests[j].Arrival = requests[j - 1].DepartureTime + problem.Distances[requests[j - 1].ID_Unique][requests[j].ID_Unique]; //Distancia de depósito al primer cliente..
                        requests[j].StartService = Math.Max(requests[j].TimeWindow.LowerBound, requests[j].Arrival);
                        requests[j].WaitingTime = requests[j].StartService - requests[j].Arrival;
                        requests[j].DepartureTime = requests[j].StartService + requests[j].ServiceTime;
                    }
                    //Llegada de vuelta al depósito.                                             
                    requests[requests.Count - 1].Arrival = requests[requests.Count - 2].DepartureTime + problem.Distances[requests[requests.Count - 2].ID_Unique][0]; //Distancia de la última petición al depósito.

                    skip = true;
                    //Actualizar Ride Time de cada vértice k cuyo destino está después de i. Paso 7d.
                    for (int k = i + 1; k < requests.Count - 1; k++)
                    {
                        //Si es un destino.
                        if (!requests[k].Origin)
                        {
                            //Al ser un destino y venir después de i, hay que actualizar el Ride Time de su origen
                            var origin = requests.Find(t => t.ID_Unique.Equals(requests[k].ID_Unique - problem.TotalCustomers));
                            origin.RideTime = requests[k].StartService - origin.DepartureTime;
                            if (origin.RideTime > problem.MaxRideTime)
                                skip = false;
                        }
                    }
                    if (skip)
                        goto Step8;
                }
            }


            Step8:
            //Paso 8. Computar violaciones de capacidad vehiculo, duración de cada vehículo, tiempos de ventana y tiempos de viaje por usuario.
            //Resetear valores de restricción.
            vehicle.TotalLoadViolation = 0;
            vehicle.TotalTimeWindowViolation = 0;
            vehicle.TotalDurationViolation = 0;
            vehicle.TotalRideTimeViolation = 0;
            vehicle.TotalWaitingTime = 0;

            //Duración vehiculo = Llegada al depósito - Comienzo del servicio por primera vez (en el depósito).
            vehicle.VehicleDuration = vehicle.Requests[vehicle.Requests.Count - 1].Arrival - vehicle.Requests[0].DepartureTime;
            //Procesar las violaciones sobre los tiempos de ventana y los tiempos máximos de viaje por cliente
            var index = 0;
            foreach (var request in vehicle.Requests)
            {
                if (request.ID_Unique != 0)
                {
                    vehicle.TotalWaitingTime += vehicle.Requests[index].WaitingTime;
                    if (request.Origin)
                    {
                        //Localización petición origen
                        request.RideTimeViolation = (request.RideTime - problem.MaxRideTime < 0) ? 0 : request.RideTime - problem.MaxRideTime;
                        vehicle.TotalRideTimeViolation += request.RideTimeViolation;
                    }

                    if (request.IsCritical) //Si es critico computa TimeWindowViolation
                    {
                        request.TimeWindowViolation = (request.StartService - request.TimeWindow.UpperBound < 0) ? 0 : request.StartService - request.TimeWindow.UpperBound;
                        vehicle.TotalTimeWindowViolation += (request.StartService - request.TimeWindow.UpperBound < 0) ? 0 : request.StartService - request.TimeWindow.UpperBound;
                    }
                }
                if (request.RequiredLoad > problem.MaxLoad)
                {
                    vehicle.TotalLoadViolation += request.RequiredLoad - problem.MaxLoad;
                }
                index++;
            }
            vehicle.TotalDurationViolation = (vehicle.VehicleDuration - problem.MaxVehDistance > 0) ? vehicle.VehicleDuration - problem.MaxVehDistance : 0;
        }




        internal static void UpdateConstraintsSolution(ref Solution solution, Problem problem)
        {
            solution.TotalDuration = solution.Vehicles.Sum(t => t.VehicleDuration);
            solution.TimeWindowViolation = solution.Vehicles.Sum(t => t.TotalTimeWindowViolation);
            solution.RideTimeViolation = solution.Vehicles.Sum(r => r.TotalRideTimeViolation);
            solution.DurationViolation = solution.Vehicles.Sum(d => d.TotalDurationViolation);
            solution.LoadViolation = solution.Vehicles.Sum(l => l.TotalLoadViolation);
            solution.TotalWaitingTime = solution.Vehicles.Sum(t => t.TotalWaitingTime);
        }




        /// <summary>
        /// Computación del Slack Time F de todos los depósitos.
        /// </summary>'
        private static void ComputeDepotSlackTime(ref Solution solution, Vehicle vehicle, Problem problem)
        {
            var requests = vehicle.Requests;
            var depot = requests[0];
            double minimun = Double.MaxValue;
            double value = 0.0;
            for (int j = 0; j < requests.Count - 1; j++)
            {
                var request_Source = requests[j];
                var request_Target = requests.Find(t => t.PairID.Equals(request_Source.PairID) && t.Origin != request_Source.Origin);
                if (request_Target == null)
                    request_Target = requests[requests.Count - 1];
                double ride_Time_User = 0.0;
                if (request_Source.ID_Unique != 0 && !request_Source.Origin)
                {
                    ride_Time_User = request_Source.StartService - request_Target.DepartureTime;
                }
                //if (request_Source.ID_Unique != 0 && request_Source.Origin)
                //    ride_Time_User = request_Target.StartService - request_Source.DepartureTime;
                else
                    ride_Time_User = 0;
                var ec = Math.Max(0, Math.Min(request_Source.TimeWindow.UpperBound - request_Source.StartService, problem.MaxRideTime - ride_Time_User));
                var nodes = requests.Take(j + 1).ToList();

                var t_WTime = 0.0;
                foreach (var node in nodes)
                {
                    t_WTime += node.WaitingTime;
                }
                value = t_WTime + ec;
                if (value < minimun)
                    minimun = value;
            }
            requests[0].SlackTime = minimun; //Slack time depot.

            var t_WT = 0.0;
            foreach (var node in vehicle.Requests)
            {
                t_WT += node.WaitingTime;
            }

            vehicle.Requests[0].DepartureTime = vehicle.Requests[0].TimeWindow.LowerBound + Math.Min(vehicle.Requests[0].SlackTime, t_WT);


        }



        /// <summary>
        /// Computación del slack time de un vértice.
        /// </summary>
        private static void ComputeIndividualSlackTime(List<Request> requests, int index, Problem problem, Solution solution)
        {
            double minimun = Double.MaxValue;
            double value = 0.0;
            Request customer = null;

            //ver fórmula aplicada.
            for (int j = index; j < requests.Count; j++)
            {
                customer = requests[j];
                double ride_Time_User = 0.0;
                if (customer.ID_Unique != 0 && !customer.Origin)
                {
                    ride_Time_User = customer.StartService - requests.Find(t => t.PairID == customer.PairID && t.Origin).DepartureTime;
                }
                else
                    ride_Time_User = Double.MinValue;

                var customers_WT = requests.GetRange(index + 1, j - index);
                if (customers_WT.Count > 0)
                {
                    double waiting_time = 0.0;
                    foreach (var node in customers_WT)
                    {
                        waiting_time += node.WaitingTime;
                    }
                    value = waiting_time + Math.Max(0, Math.Min(customer.TimeWindow.UpperBound - customer.StartService, problem.MaxRideTime - ride_Time_User));
                }
                else
                    value = Math.Max(0, Math.Min(customer.TimeWindow.UpperBound - customer.StartService, problem.MaxRideTime - ride_Time_User));
                if (value < minimun)
                    minimun = value;
            }
            if (index != 0)
            {
                customer = requests[index];
                customer.SlackTime = minimun;
                var remainingNodes = requests.Skip(index + 1);
                var t_WTime = 0.0;
                foreach (var node in remainingNodes)
                {
                    t_WTime += node.WaitingTime;
                }
                customer.WaitingTime = customer.WaitingTime + Math.Min(customer.SlackTime, t_WTime);
                customer.StartService = customer.Arrival + customer.WaitingTime;
                customer.DepartureTime = customer.StartService + customer.ServiceTime;
            }
            else
            {
                requests[0].SlackTime = minimun; //Slack time depot.

                var t_WT = 0.0;
                foreach (var node in requests)
                {
                    t_WT += node.WaitingTime;
                }

                requests[0].DepartureTime = requests[0].TimeWindow.LowerBound + Math.Min(requests[0].SlackTime, t_WT);
            }
        }
    }
}
