using API.DARP.Calculations.Algorithms;
using API.DARP.Calculations.Validation;
using API.DARP.Data.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DARP.Calculations.Metaheuristics.ILS
{
    internal class Perturbations
    {
        private Random _random;
        private Problem _problem;
        private const int MAX_PERTURBATIONS = 2;
        List<int> routes = new List<int>();
        Dictionary<int, List<int>> PerturbationMechanism;

        public Perturbations(Random rand, Problem problem)
        {
            this._random = rand;
            this._problem = problem;
            routes = Enumerable.Range(0, problem.NumVehicles).ToList();
            ResetPerturbationMechanism();
        }

        private void ResetPerturbationMechanism(bool all = true, int vehicle = -1)
        {
            if (all)
            {
                PerturbationMechanism = new Dictionary<int, List<int>>();
                for (int i = 0; i < _problem.NumVehicles; i++)
                {
                    PerturbationMechanism[i] = new List<int>();
                    var list = new List<int>();
                    for (int j = 0; j < _problem.NumVehicles; j++)
                    {
                        if (j != i)
                            list.Add(j);
                    }
                    PerturbationMechanism[i] = list;
                }
            }
            else
            {
                var list = new List<int>();
                for (int j = 0; j < _problem.NumVehicles; j++)
                {
                    if (j != vehicle)
                        list.Add(j);
                }
                PerturbationMechanism[vehicle] = list;
            }
        }


        internal int ExecutePerturbation(ref Solution solution)
        {
            var perturbation = _random.Next(0, MAX_PERTURBATIONS);
            //perturbation = 2;
            switch (perturbation)
            {
                case 0:
                    ShiftPerturbation(ref solution);
                    return 0;
                case 3:
                    ChainPerturbation(ref solution);
                    return 2;
                case 1:
                    SwapPerturbation(ref solution);
                    return 1;
                case 2:
                    SpecialPerturbation(ref solution);
                    return 2;
                default:
                    return -1;
            }
        }



        #region Perturbations



        private void ShiftPerturbation(ref Solution solution)
        {
            int v1 = -1, v2 = -1;

            //marcar todos los vehículos como no perturbados.
            solution.Vehicles = solution.Vehicles.Select(t => { t.Perturbed = true; return t; }).ToList();

            //Recorrer todos los vehículos
            //for (int v = 0; v < solution.Vehicles.Count; v++)
            //{
            //Seleccionar el primer vehículo.
            do
            {
                v1 = _random.Next(0, PerturbationMechanism.Keys.Count);
            }
            while (solution.Vehicles[v1].Requests.Count <= 4);
            if (PerturbationMechanism[v1].Count == 0)
                ResetPerturbationMechanism(false, v1);

            do
            {
                v2 = PerturbationMechanism[v1][_random.Next(0, PerturbationMechanism[v1].Count)];
            }
            while (v1 == v2);

            //Seleccionar aleatoriamente un cliente de v1.
            List<Request> nodes = new List<Request>();

            var node = solution.Vehicles[v1].Requests[_random.Next(1, solution.Vehicles[v1].Requests.Count - 1)];
            nodes = solution.Vehicles[v1].Requests.FindAll(t => t.PairID.Equals(node.PairID)); //Extraemos inicio y destino.  

            //Siguiente paso. Insertarlos en su mejor posición posible.
            var origin = nodes.Find(t => t.Origin == true);
            int posOrigin = solution.Vehicles[v1].Requests.IndexOf(origin);
            var destination = nodes.Find(t => t.Origin == false);
            int posDestination = solution.Vehicles[v1].Requests.IndexOf(destination);

            if (solution.Vehicles[v2].Requests.Count == 2)
            {
                solution.Vehicles[v2].Requests.Insert(1, destination);
                solution.Vehicles[v2].Requests.Insert(1, origin);
            }
            else
            {
                solution.Vehicles[v1].Requests.RemoveAll(t => t.PairID.Equals(nodes.First().PairID)); //Los eliminamos de su ruta inicio.

                //Determinamos nodo crítico (será el primero a insertar).
                Request critical;
                Request notCritical;
                if (origin.IsCritical)
                {
                    critical = origin;
                    notCritical = destination;
                }
                else
                {
                    critical = destination;
                    notCritical = origin;
                }

                //Seleccionar posiciones en el vehículo v2

                Dictionary<int, int> bestPair = new Dictionary<int, int>();
                int bestPosSource = -1;
                double min = double.MaxValue;


                for (int pos = 1; pos < solution.Vehicles[v2].Requests.Count; pos++)
                {                   
                    double twDist = 0.0;
                    if (pos == 1)
                    {
                        if ((critical.TimeWindow.LowerBound + _problem.Distances[critical.ID_Unique][solution.Vehicles[v2].Requests[pos].ID_Unique]) > solution.Vehicles[v2].Requests[pos].TimeWindow.UpperBound) continue;
                        twDist = CalculateDistance(critical, v2, pos, solution);
                    }
                    else if (pos == solution.Vehicles[v2].Requests.Count - 1)
                    {
                        if ((solution.Vehicles[v2].Requests[pos].TimeWindow.LowerBound + _problem.Distances[critical.ID_Unique][solution.Vehicles[v2].Requests[pos].ID_Unique]) > critical.TimeWindow.UpperBound) continue;
                        twDist = CalculateDistance(critical, v2, pos-1, solution);
                    }
                    else
                    {
                        if (((solution.Vehicles[v2].Requests[pos-1].TimeWindow.LowerBound + _problem.Distances[critical.ID_Unique][solution.Vehicles[v2].Requests[pos-1].ID_Unique]) > critical.TimeWindow.UpperBound) ||
                            (critical.TimeWindow.LowerBound + _problem.Distances[critical.ID_Unique][solution.Vehicles[v2].Requests[pos].ID_Unique]) > solution.Vehicles[v2].Requests[pos].TimeWindow.UpperBound) continue;
                            twDist = (CalculateDistance(critical, v2, pos, solution) + CalculateDistance(critical, v2, pos - 1, solution)) / 2.0;
                    }
                    if (twDist < min)
                    {
                        bestPosSource = pos;
                        min = twDist;
                    }
                }

                if (bestPosSource == -1) //Insertar en pos = 1;
                {
                    bestPosSource = 1;
                }

                solution.Vehicles[v2].Requests.Insert(bestPosSource, critical);

                int notCriticalPos = -1;

                min = double.MaxValue;
                //Insertar no crítico.
                if (critical.Origin)
                {
                    //De pos en adelante.
                    for (int init = bestPosSource + 1; init < solution.Vehicles[v2].Requests.Count; init++)
                    {
                        if ((solution.Vehicles[v2].Requests[init - 1].TimeWindow.LowerBound + _problem.Distances[solution.Vehicles[v2].Requests[init - 1].ID_Unique][notCritical.ID_Unique]) > notCritical.TimeWindow.UpperBound) continue;
                        double twDist = 0.0;
                        if (init == solution.Vehicles[v2].Requests.Count)
                        {
                            if ((solution.Vehicles[v2].Requests[init-1].TimeWindow.LowerBound + _problem.Distances[critical.ID_Unique][solution.Vehicles[v2].Requests[init-1].ID_Unique]) > critical.TimeWindow.UpperBound) continue;
                            twDist = CalculateDistance(critical, v2, init - 1, solution);
                        }
                        else
                        {
                            if (((solution.Vehicles[v2].Requests[init - 1].TimeWindow.LowerBound + _problem.Distances[critical.ID_Unique][solution.Vehicles[v2].Requests[init-1].ID_Unique]) > critical.TimeWindow.UpperBound) ||
                            (critical.TimeWindow.LowerBound + _problem.Distances[critical.ID_Unique][solution.Vehicles[v2].Requests[init].ID_Unique]) > solution.Vehicles[v2].Requests[init].TimeWindow.UpperBound) continue;
                            twDist = (CalculateDistance(critical, v2, init, solution) + CalculateDistance(critical, v2, init - 1, solution)) / 2.0;
                        }
                        //var diff = Problem.EuclideanDistance(notCritical.TimeWindow.LowerBound, notCritical.TimeWindow.UpperBound, solution.Vehicles[v2].Requests[init - 1].TimeWindow.LowerBound, solution.Vehicles[v2].Requests[init - 1].TimeWindow.UpperBound);
                        if (twDist < min)
                        {
                            notCriticalPos = init;
                            min = twDist;
                        }
                    }
                }
                else
                {
                    //De pos hacia atrás.
                    for (int init = bestPosSource; init > 0; init--)
                    {
                        if ((notCritical.TimeWindow.LowerBound + _problem.Distances[notCritical.ID_Unique][solution.Vehicles[v2].Requests[init].ID_Unique]) > solution.Vehicles[v2].Requests[init].TimeWindow.UpperBound) continue;

                        //var diff = Problem.EuclideanDistance(notCritical.TimeWindow.LowerBound, notCritical.TimeWindow.UpperBound, solution.Vehicles[v2].Requests[init].TimeWindow.LowerBound, solution.Vehicles[v2].Requests[init].TimeWindow.UpperBound);
                        double twDist = 0.0;
                        if (init == 1)
                        {
                            if ((critical.TimeWindow.LowerBound + _problem.Distances[critical.ID_Unique][solution.Vehicles[v2].Requests[init].ID_Unique]) > solution.Vehicles[v2].Requests[init].TimeWindow.UpperBound) continue;
                            twDist = CalculateDistance(critical, v2, init, solution);
                        }
                        else
                        {
                            if (((solution.Vehicles[v2].Requests[init - 1].TimeWindow.LowerBound + _problem.Distances[critical.ID_Unique][solution.Vehicles[v2].Requests[init - 1].ID_Unique]) > critical.TimeWindow.UpperBound) ||
                            (critical.TimeWindow.LowerBound + _problem.Distances[critical.ID_Unique][solution.Vehicles[v2].Requests[init].ID_Unique]) > solution.Vehicles[v2].Requests[init].TimeWindow.UpperBound) continue;
                            twDist = CalculateDistance(critical, v2, init, solution);
                        }
                        if (twDist < min)
                        {
                            notCriticalPos = init;
                            min = twDist;
                        }
                    }
                }

                solution.Vehicles[v2].Requests.Insert(notCriticalPos, notCritical);
            }
          //  }

            //Recalcular con el proceso de 8 pasos y actualizar el coste.
            foreach (var vehicle in solution.Vehicles)
            {
                DARPAlgorithms.EightStepsEvaluationProcedure(ref solution, vehicle, _problem);
            }
            solution.TotalDuration = solution.Vehicles.Sum(t => t.VehicleDuration);
            solution.TimeWindowViolation = solution.Vehicles.Sum(t => t.TotalTimeWindowViolation);
            solution.RideTimeViolation = solution.Vehicles.Sum(r => r.TotalRideTimeViolation);
            solution.DurationViolation = solution.Vehicles.Sum(d => d.TotalDurationViolation);
            solution.LoadViolation = solution.Vehicles.Sum(l => l.TotalLoadViolation);
            solution.TotalWaitingTime = solution.Vehicles.Sum(t => t.TotalWaitingTime);
            DARPAlgorithms.UpdateConstraintsSolution(ref solution, _problem);
            solution.Fitness = Costs.EvaluationSolution(solution, _problem);

            try
            {
                Validator.Positions(ref solution, _problem);
                Validation.Validator.ValidateSolution(ref solution, _problem);
            }
            catch (Exception ex)
            {
                Console.WriteLine();
            }
        }

        private double CalculateDistance(Request critical, int v2, int pos, Solution solution)
        {
            return Problem.EuclideanDistance(critical.TimeWindow.LowerBound, critical.TimeWindow.UpperBound, solution.Vehicles[v2].Requests[pos].TimeWindow.LowerBound, solution.Vehicles[v2].Requests[pos].TimeWindow.UpperBound);
        }

        private void SwapPerturbation(ref Solution solution)
        {

            solution.Vehicles = solution.Vehicles.Select(t => { t.Perturbed = false; return t; }).ToList();
            //Seleccionar el primer vehículo.
            int v1 = _random.Next(0, PerturbationMechanism.Keys.Count);
            if (PerturbationMechanism[v1].Count == 0)
                ResetPerturbationMechanism(false, v1);

            int v2;
            do
            {
                v2 = PerturbationMechanism[v1][_random.Next(0, PerturbationMechanism[v1].Count)];
            }
            while (v1 == v2);

            PerturbationMechanism[v1].Remove(v2);
            PerturbationMechanism[v2].Remove(v1);

            var posA = _random.Next(1, solution.Vehicles[v1].Requests.Count - 1);
            var node1 = solution.Vehicles[v1].Requests[posA];
            var node2 = solution.Vehicles[v1].Requests.Find(t => t.PairID == node1.PairID && t.Origin != node1.Origin);
            var posB = solution.Vehicles[v1].Requests.IndexOf(node2);

            //Determinamos nodo crítico (será el primero a insertar).
            Request originA;
            Request destinationA;

            int posOriginA = -1;
            int posDestinationA = -1;
            if (node1.Origin)
            {
                originA = node1;
                destinationA = node2;
                posOriginA = posA;
                posDestinationA = posB;
            }
            else
            {
                originA = node2;
                destinationA = node1;
                posOriginA = posB;
                posDestinationA = posA;
            }



            int minPos = -1;
            double min = Double.MaxValue;
            for (int i = 1; i < solution.Vehicles[v2].Requests.Count - 1; i++)
            {
                //if (!ValidTimeWindow(solution, node1, _problem, i + 1, v2)) continue;
               // var distance = Problem.EuclideanDistance(node1.TimeWindow.LowerBound, node1.TimeWindow.UpperBound, solution.Vehicles[v2].Requests[i].TimeWindow.LowerBound, solution.Vehicles[v2].Requests[i].TimeWindow.UpperBound);

                double twDist = 0.0;
                if (i == 1)
                {
                    if (!ValidTimeWindow(solution, node1, _problem, i + 1, v2)) continue;
                    twDist = CalculateDistance(node1, v2, i-1, solution);
                }
                else if (i == solution.Vehicles[v2].Requests.Count - 2)
                {
                    if (!ValidTimeWindow(solution, node1, _problem, i - 1, v2)) continue;
                    twDist = CalculateDistance(node1, v2, i - 1, solution);
                }
                else
                {
                    if (!ValidTimeWindow(solution, node1, _problem, i + 1, v2) || !ValidTimeWindow(solution, node1, _problem, i - 1, v2)) continue;
                        twDist = (CalculateDistance(node1, v2, i+1, solution) + CalculateDistance(node1, v2, i - 1, solution)) / 2.0;
                }
                if (twDist < min)
                {
                    minPos = i;
                    min = twDist;
                }
            }

            if (minPos == -1)
                return;

            var nodeB1 = solution.Vehicles[v2].Requests[minPos];
            var nodeB2 = solution.Vehicles[v2].Requests.Find(t => t.PairID == nodeB1.PairID && t.Origin != nodeB1.Origin);
            var posNodeB2 = solution.Vehicles[v2].Requests.IndexOf(nodeB2);

            //Determinamos nodo crítico (será el primero a insertar).
            Request originB;
            Request destinationB;
            if (nodeB1.Origin)
            {
                originB = nodeB1;
                destinationB = nodeB2;
            }
            else
            {
                originB = nodeB2;
                destinationB = nodeB1;
                var aux = minPos;
                minPos = posNodeB2;
                posNodeB2 = aux;
            }

            solution.Vehicles[v2].Requests[minPos] = originA;
            solution.Vehicles[v2].Requests[posNodeB2] = destinationA;
            solution.Vehicles[v1].Requests[posOriginA] = originB;
            solution.Vehicles[v1].Requests[posDestinationA] = destinationB;


            Validator.Positions(ref solution, _problem);

            solution.Vehicles[v2].Perturbed = true;
            solution.Vehicles[v1].Perturbed = true;

            DARPAlgorithms.EightStepsEvaluationProcedure(ref solution, solution.Vehicles[v1], _problem);
            DARPAlgorithms.EightStepsEvaluationProcedure(ref solution, solution.Vehicles[v2], _problem);

            solution.TotalDuration = solution.Vehicles.Sum(t => t.VehicleDuration);
            solution.TimeWindowViolation = solution.Vehicles.Sum(t => t.TotalTimeWindowViolation);
            solution.RideTimeViolation = solution.Vehicles.Sum(r => r.TotalRideTimeViolation);
            solution.DurationViolation = solution.Vehicles.Sum(d => d.TotalDurationViolation);
            solution.LoadViolation = solution.Vehicles.Sum(l => l.TotalLoadViolation);
            solution.TotalWaitingTime = solution.Vehicles.Sum(t => t.TotalWaitingTime);
            solution.Fitness = Costs.EvaluationSolution(solution, _problem);
            Validator.Positions(ref solution, _problem);
        }

        private void SpecialPerturbation(ref Solution solution)
        {
            solution.Vehicles = solution.Vehicles.Select(t => { t.Perturbed = true; return t; }).ToList();
            //1. Sacar un vehículo aleatorio.
            var vehicle = solution.Vehicles[_random.Next(0, solution.Vehicles.Count - 1)];


            List<int> numbers = new List<int>();
            //2. Sacar un cliente crítico.
            var client = vehicle.Requests[_random.Next(1, vehicle.Requests.Count - 1)];

            List<Tuple<Request, Tuple<int, int>>> data = new List<Tuple<Request, Tuple<int, int>>>();

            if (!client.Origin)
                client = vehicle.Requests.Find(t => t.Origin && t.PairID == client.PairID);

            var vehicle_pos = solution.Vehicles.IndexOf(vehicle);
            var client_pos = vehicle.Requests.IndexOf(client);

            numbers.Add(client.PairID);

            data.Add(new Tuple<Request, Tuple<int, int>>(client, new Tuple<int, int>(vehicle_pos, client_pos)));

            //3. Sacar sus N vecinos críticos.
            var neighbours = _problem.CloseTWMatrix[client.ID_Unique];
            List<Request> vecinos = new List<Request>();
            for(int i=0; vecinos.Count < Convert.ToInt32(Math.Sqrt(_problem.NumRequests)) && i< neighbours.Count; i++)
            {
                if (_problem.Requests.Find(t => t.ID_Unique == neighbours[i]).Origin)
                {
                    vecinos.Add(_problem.Requests.Find(t => t.ID_Unique == neighbours[i]));
                    numbers.Add(vecinos[vecinos.Count - 1].PairID);
                }
            }

            //4. Sacar vehículo y posición de cada uno
            foreach (var v in vecinos)
            {
                for (int i=0; i<solution.Vehicles.Count; i++)
                {
                    if (solution.Vehicles[i].Requests.Find(t => t.ID_Unique == v.ID_Unique) != null)
                    {
                        data.Add(new Tuple<Request, Tuple<int, int>>(v, new Tuple<int, int>(i, solution.Vehicles[i].Requests.IndexOf(solution.Vehicles[i].Requests.Find(t => t.ID_Unique == v.ID_Unique)))));
                    }

                }
            }

            List<Tuple<Request, Tuple<int, int>>> movements = new List<Tuple<Request, Tuple<int, int>>>();
            foreach (var element in data)
            {
                var request = element.Item1;
                int selected = -1;
                if (numbers.Count == 1 && numbers.First() == element.Item1.PairID)
                    break;
                //5. Mezclarlos.
                do
                {
                    selected = numbers[_random.Next(0, numbers.Count)];
                }
                while (selected == request.ID_Unique);
                numbers.Remove(selected); //eliminarlo.

                var posInterchange = data.Find(t => t.Item1.ID_Unique == selected);

                //solution.Vehicles[element.Item2.Item1].Requests[element.Item2.Item2] = posInterchange.Item1;
                //solution.Vehicles[posInterchange.Item2.Item1].Requests[posInterchange.Item2.Item2] = element.Item1;
                //los destinos.
                var destinoElement = solution.Vehicles[element.Item2.Item1].Requests.Find(t => t.PairID == element.Item1.PairID && t.Origin == false);
                var destinoInterchange = solution.Vehicles[posInterchange.Item2.Item1].Requests.Find(t => t.PairID == posInterchange.Item1.PairID && t.Origin == false);
                //solution.Vehicles[element.Item2.Item1].Requests[solution.Vehicles[element.Item2.Item1].Requests.IndexOf(destinoElement)] = destinoInterchange;
                //solution.Vehicles[posInterchange.Item2.Item1].Requests[solution.Vehicles[posInterchange.Item2.Item1].Requests.IndexOf(destinoInterchange)] = destinoElement;

                movements.Add(new Tuple<Request, Tuple<int, int>>(element.Item1, new Tuple<int, int>(posInterchange.Item2.Item1, posInterchange.Item2.Item2)));
                movements.Add(new Tuple<Request, Tuple<int, int>>(destinoElement, new Tuple<int, int>(posInterchange.Item2.Item1, solution.Vehicles[posInterchange.Item2.Item1].Requests.IndexOf(destinoInterchange))));
            }

            foreach(var entry in movements)
            {
                solution.Vehicles[entry.Item2.Item1].Requests[entry.Item2.Item2] = entry.Item1;
            }
            //Recalcular con el proceso de 8 pasos y actualizar el coste.
            foreach (var veh in solution.Vehicles)
            {
                DARPAlgorithms.EightStepsEvaluationProcedure(ref solution, veh, _problem);
            }
            solution.TotalDuration = solution.Vehicles.Sum(t => t.VehicleDuration);
            solution.TimeWindowViolation = solution.Vehicles.Sum(t => t.TotalTimeWindowViolation);
            solution.RideTimeViolation = solution.Vehicles.Sum(r => r.TotalRideTimeViolation);
            solution.DurationViolation = solution.Vehicles.Sum(d => d.TotalDurationViolation);
            solution.LoadViolation = solution.Vehicles.Sum(l => l.TotalLoadViolation);
            solution.TotalWaitingTime = solution.Vehicles.Sum(t => t.TotalWaitingTime);
            DARPAlgorithms.UpdateConstraintsSolution(ref solution, _problem);
            solution.Fitness = Costs.EvaluationSolution(solution, _problem);

        }

        private void ChainPerturbation(ref Solution solution)
        {
            try
            {
                //marcar todos los vehículos como no perturbados.
                solution.Vehicles = solution.Vehicles.Select(t => { t.Perturbed = false; return t; }).ToList();

                //Seleccionar el primer vehículo.
                int v1 = _random.Next(0, PerturbationMechanism.Keys.Count);
                if (PerturbationMechanism[v1].Count == 0)
                    ResetPerturbationMechanism(false, v1);

                int v2;
                do
                {
                    v2 = PerturbationMechanism[v1][_random.Next(0, PerturbationMechanism[v1].Count)];
                }
                while (v1 == v2);
                //routes.Remove(v2);

                PerturbationMechanism[v1].Remove(v2);
                PerturbationMechanism[v2].Remove(v1);

                //Seleccionar el tamaño de la cadena
                int maxSizeR1 = Math.Min(2, solution.Vehicles[v1].Requests.Count - 2);
                int maxSizeR2 = Math.Min(2, solution.Vehicles[v2].Requests.Count - 2);

                if (maxSizeR1 == 0)
                    maxSizeR1 = 1;
                if (maxSizeR2 == 0)
                    maxSizeR2 = 1;

                int size_R1 = _random.Next(1, maxSizeR1);
                int size_R2 = _random.Next(1, maxSizeR2);



                //Posición inicial desde la que coger elementos.
                int init_PosR1 = _random.Next(1, solution.Vehicles[v1].Requests.Count - 1);
                int init_PosR2 = _random.Next(1, solution.Vehicles[v2].Requests.Count - 1);

                if (init_PosR1 <= 0)
                    init_PosR1 = 1;
                if (init_PosR2 <= 0)
                    init_PosR2 = 1;




                //Obtener los elementos seleccionados en cada rango.
                var elementsR1 = solution.Vehicles[v1].Requests.GetRange(init_PosR1, size_R1);
                var elementsR2 = solution.Vehicles[v2].Requests.GetRange(init_PosR2, size_R2);

                foreach(var element in elementsR1)
                {
                    solution.Vehicles[v1].Requests.RemoveAll(t => t.PairID == element.ID_Unique);
                }

                foreach (var element in elementsR2)
                {
                    solution.Vehicles[v2].Requests.RemoveAll(t => t.PairID == element.ID_Unique);
                }

                //Marcar ambas rutas como perturbadas.
                solution.Vehicles[v1].Perturbed = true;
                solution.Vehicles[v2].Perturbed = true;

                List<Node> chainR1ToR2 = new List<Node>();
                List<Node> chainR2ToR1 = new List<Node>();


                List<int> tabu = new List<int>();
                foreach (var node in elementsR1)
                {
                    if (tabu.Contains(node.PairID))
                        continue;
                    tabu.Add(node.PairID);
                    var secondNode = _problem.Requests.Find(t => t.PairID == node.PairID && t.Origin != node.Origin);
                    //solution.Vehicles[v1].Requests.RemoveAll(t => t.PairID.Equals(node.PairID)); //Eliminar el destino y el origen


                    Request criticalNode;
                    Request notCriticalNode;
                    if (node.IsCritical)
                    {
                        criticalNode = node;
                        notCriticalNode = secondNode;
                    }
                    else
                    {
                        criticalNode = secondNode;
                        notCriticalNode = node;
                    }


                    double min_Dif = double.MaxValue;
                    int bestCriticalPos = -1;
                    //Si el nodo actual es crítico, insertarlo en su primera posición válida respecto a TW.
                    for (int i = 1; i < solution.Vehicles[v2].Requests.Count; i++)
                    {
                        //if (!_problem.FeasibleTravel[solution.Vehicles[v2].Requests[i].ID_Unique][criticalNode.ID_Unique]) continue;
                        double twDist = 0.0;
                        if (i == 1)
                        {
                            if ((criticalNode.TimeWindow.LowerBound + _problem.Distances[criticalNode.ID_Unique][solution.Vehicles[v2].Requests[i].ID_Unique]) > solution.Vehicles[v2].Requests[i].TimeWindow.UpperBound) continue;
                            twDist = CalculateDistance(criticalNode, v2, i, solution);
                        }
                        else if (i == solution.Vehicles[v2].Requests.Count - 1)
                        {
                            if ((solution.Vehicles[v2].Requests[i-1].TimeWindow.LowerBound + _problem.Distances[criticalNode.ID_Unique][solution.Vehicles[v2].Requests[i-1].ID_Unique]) > criticalNode.TimeWindow.UpperBound) continue;
                            twDist = CalculateDistance(criticalNode, v2, i-1, solution);
                        }
                        else
                        {
                            if (((solution.Vehicles[v2].Requests[i - 1].TimeWindow.LowerBound + _problem.Distances[criticalNode.ID_Unique][solution.Vehicles[v2].Requests[i - 1].ID_Unique]) > criticalNode.TimeWindow.UpperBound) ||
                            (criticalNode.TimeWindow.LowerBound + _problem.Distances[criticalNode.ID_Unique][solution.Vehicles[v2].Requests[i].ID_Unique]) > solution.Vehicles[v2].Requests[i].TimeWindow.UpperBound) continue;
                            twDist = (CalculateDistance(criticalNode, v2, i, solution) + CalculateDistance(criticalNode, v2, i - 1, solution)) / 2.0;
                        }
                        if (twDist < min_Dif)
                        {
                            min_Dif = twDist;
                            bestCriticalPos = i;
                        }

                    }

                    if (bestCriticalPos == -1)
                    {
                        bestCriticalPos = 1;
                        solution.Vehicles[v2].Requests.Insert(1, criticalNode);
                    }
                    else
                        solution.Vehicles[v2].Requests.Insert(bestCriticalPos, criticalNode);

                    //La posición actual es válida para insertar el nodo crítico. Insertar el no crítico en su mejor posición
                    if (criticalNode.Origin)
                    {
                        double bestCost = double.PositiveInfinity;
                        int bestPos = bestCriticalPos + 1;
                        //Insertar el nodo no crítico después.
                        for (int j = bestCriticalPos + 1; j < solution.Vehicles[v2].Requests.Count; j++)
                        {
                            if (!_problem.FeasibleTravel[notCriticalNode.ID_Unique][solution.Vehicles[v2].Requests[j].ID_Unique]) continue;

                            //var diff = Problem.EuclideanDistance(notCriticalNode.TimeWindow.LowerBound, notCriticalNode.TimeWindow.UpperBound, solution.Vehicles[v2].Requests[j + 1].TimeWindow.LowerBound, solution.Vehicles[v2].Requests[j + 1].TimeWindow.UpperBound);
                            double twDist = 0.0;
                            if (j == solution.Vehicles[v2].Requests.Count-1)
                            {
                                if ((solution.Vehicles[v2].Requests[j-1].TimeWindow.LowerBound + _problem.Distances[criticalNode.ID_Unique][solution.Vehicles[v2].Requests[j-1].ID_Unique]) > criticalNode.TimeWindow.UpperBound) continue;
                                twDist = CalculateDistance(notCriticalNode, v2, j-1, solution);
                            }
                            else
                            {
                                if (((solution.Vehicles[v2].Requests[j - 1].TimeWindow.LowerBound + _problem.Distances[criticalNode.ID_Unique][solution.Vehicles[v2].Requests[j - 1].ID_Unique]) > criticalNode.TimeWindow.UpperBound) ||
                                (criticalNode.TimeWindow.LowerBound + _problem.Distances[criticalNode.ID_Unique][solution.Vehicles[v2].Requests[j].ID_Unique]) > solution.Vehicles[v2].Requests[j].TimeWindow.UpperBound) continue;
                                twDist = (CalculateDistance(notCriticalNode, v2, j, solution) + CalculateDistance(notCriticalNode, v2, j - 1, solution)) / 2.0;
                            }
                            if (twDist < bestCost)
                            {
                                bestPos = j;
                                bestCost = twDist;
                            }
                        }
                        //Insertar en la mejor posición declarada.
                        solution.Vehicles[v2].Requests.Insert(bestPos, notCriticalNode);
                    }
                    else //El nodo no crítico es el origen.
                    {
                        double bestCost = double.PositiveInfinity;
                        int bestPos = bestCriticalPos;
                        //Insertar el nodo no crítico después.
                        //Insertar el nodo no crítico antes.
                        for (int j = bestCriticalPos; j > 0; j--)
                        {
                            if (!_problem.FeasibleTravel[solution.Vehicles[v2].Requests[j - 1].ID_Unique][notCriticalNode.ID_Unique]) continue;
                            //var diff = Problem.EuclideanDistance(notCriticalNode.TimeWindow.LowerBound, notCriticalNode.TimeWindow.UpperBound, solution.Vehicles[v2].Requests[j + 1].TimeWindow.LowerBound, solution.Vehicles[v2].Requests[j + 1].TimeWindow.UpperBound);
                            double twDist = 0.0;
                            if (j == 1)
                            {
                                if ((criticalNode.TimeWindow.LowerBound + _problem.Distances[criticalNode.ID_Unique][solution.Vehicles[v2].Requests[j].ID_Unique]) > solution.Vehicles[v2].Requests[j].TimeWindow.UpperBound) continue;
                                twDist = CalculateDistance(notCriticalNode, v2, j, solution);
                            }
                            else
                            {
                                if (((solution.Vehicles[v2].Requests[j - 1].TimeWindow.LowerBound + _problem.Distances[criticalNode.ID_Unique][solution.Vehicles[v2].Requests[j - 1].ID_Unique]) > criticalNode.TimeWindow.UpperBound) ||
                                (criticalNode.TimeWindow.LowerBound + _problem.Distances[criticalNode.ID_Unique][solution.Vehicles[v2].Requests[j].ID_Unique]) > solution.Vehicles[v2].Requests[j].TimeWindow.UpperBound) continue;
                                twDist = (CalculateDistance(notCriticalNode, v2, j, solution) + CalculateDistance(notCriticalNode, v2, j - 1, solution)) / 2.0;
                            }
                            if (twDist < bestCost)
                            {
                                bestPos = j;
                                bestCost = twDist;
                            }
                        }
                        //Insertar en la mejor posición declarada.
                        solution.Vehicles[v2].Requests.Insert(bestPos, notCriticalNode);
                        //break; //terminamos.
                    }
                }

                //Mismo proceso para los elementos de R2.
                tabu.Clear();
                foreach (var node in elementsR2)
                {
                    if (tabu.Contains(node.PairID))
                        continue;
                    tabu.Add(node.PairID);
                    var secondNode = _problem.Requests.Find(t => t.PairID == node.PairID && t.Origin != node.Origin); //solution.Vehicles[v2].Requests.Find(t => t.PairID == node.PairID && t.Origin != node.Origin);
                    //solution.Vehicles[v2].Requests.RemoveAll(t => t.PairID.Equals(node.PairID)); //Eliminar el destino y el origen


                    Request criticalNode;
                    Request notCriticalNode;
                    if (node.IsCritical)
                    {
                        criticalNode = node;
                        notCriticalNode = secondNode;
                    }
                    else
                    {
                        criticalNode = secondNode;
                        notCriticalNode = node;
                    }


                    double min_Dif = double.MaxValue;
                    int bestCriticalPos = -1;
                    //Si el nodo actual es crítico, insertarlo en su primera posición válida respecto a TW.
                    for (int i = 1; i < solution.Vehicles[v1].Requests.Count; i++)
                    {
                        double twDist = 0.0;
                        if (i == 1)
                        {
                            if ((criticalNode.TimeWindow.LowerBound + _problem.Distances[criticalNode.ID_Unique][solution.Vehicles[v1].Requests[i].ID_Unique]) > solution.Vehicles[v1].Requests[i].TimeWindow.UpperBound) continue;
                            twDist = CalculateDistance(criticalNode, v1, i, solution);
                        }
                        else if (i == solution.Vehicles[v1].Requests.Count - 1)
                        {
                            if ((solution.Vehicles[v1].Requests[i - 1].TimeWindow.LowerBound + _problem.Distances[criticalNode.ID_Unique][solution.Vehicles[v1].Requests[i - 1].ID_Unique]) > criticalNode.TimeWindow.UpperBound) continue;
                            twDist = CalculateDistance(criticalNode, v1, i - 1, solution);
                        }
                        else
                        {
                            if (((solution.Vehicles[v1].Requests[i - 1].TimeWindow.LowerBound + _problem.Distances[criticalNode.ID_Unique][solution.Vehicles[v1].Requests[i - 1].ID_Unique]) > criticalNode.TimeWindow.UpperBound) ||
                            (criticalNode.TimeWindow.LowerBound + _problem.Distances[criticalNode.ID_Unique][solution.Vehicles[v1].Requests[i].ID_Unique]) > solution.Vehicles[v1].Requests[i].TimeWindow.UpperBound) continue;
                            twDist = (CalculateDistance(criticalNode, v1, i, solution) + CalculateDistance(criticalNode, v1, i - 1, solution)) / 2.0;
                        }
                        if (twDist < min_Dif)
                        {
                            min_Dif = twDist;
                            bestCriticalPos = i;
                        }

                    }

                    if (bestCriticalPos == -1)
                    {
                        bestCriticalPos = 1;
                        solution.Vehicles[v1].Requests.Insert(1, criticalNode);
                    }
                    else
                        solution.Vehicles[v1].Requests.Insert(bestCriticalPos, criticalNode);

                    //La posición actual es válida para insertar el nodo crítico. Insertar el no crítico en su mejor posición
                    if (criticalNode.Origin)
                    {
                        double bestCost = double.PositiveInfinity;
                        int bestPos = bestCriticalPos + 1;
                        //Insertar el nodo no crítico después.
                        for (int j = bestCriticalPos + 1; j < solution.Vehicles[v1].Requests.Count; j++)
                        {
                            if (!_problem.FeasibleTravel[notCriticalNode.ID_Unique][solution.Vehicles[v1].Requests[j].ID_Unique]) continue;

                            //var diff = Problem.EuclideanDistance(notCriticalNode.TimeWindow.LowerBound, notCriticalNode.TimeWindow.UpperBound, solution.Vehicles[v2].Requests[j + 1].TimeWindow.LowerBound, solution.Vehicles[v2].Requests[j + 1].TimeWindow.UpperBound);
                            double twDist = 0.0;
                            if (j == solution.Vehicles[v1].Requests.Count - 1)
                            {
                                if ((solution.Vehicles[v1].Requests[j - 1].TimeWindow.LowerBound + _problem.Distances[criticalNode.ID_Unique][solution.Vehicles[v1].Requests[j - 1].ID_Unique]) > criticalNode.TimeWindow.UpperBound) continue;
                                twDist = CalculateDistance(notCriticalNode, v1, j - 1, solution);
                            }
                            else
                            {
                                if (((solution.Vehicles[v1].Requests[j - 1].TimeWindow.LowerBound + _problem.Distances[criticalNode.ID_Unique][solution.Vehicles[v1].Requests[j - 1].ID_Unique]) > criticalNode.TimeWindow.UpperBound) ||
                                (criticalNode.TimeWindow.LowerBound + _problem.Distances[criticalNode.ID_Unique][solution.Vehicles[v1].Requests[j].ID_Unique]) > solution.Vehicles[v1].Requests[j].TimeWindow.UpperBound) continue;
                                twDist = (CalculateDistance(notCriticalNode, v1, j, solution) + CalculateDistance(notCriticalNode, v1, j - 1, solution)) / 2.0;
                            }
                            if (twDist < bestCost)
                            {
                                bestPos = j;
                                bestCost = twDist;
                            }
                        }
                        //Insertar en la mejor posición declarada.
                        solution.Vehicles[v1].Requests.Insert(bestPos, notCriticalNode);
                        //break; //terminamos.
                    }
                    else //El nodo no crítico es el origen.
                    {
                        double bestCost = double.PositiveInfinity;
                        int bestPos = bestCriticalPos;
                        //Insertar el nodo no crítico después.
                        //Insertar el nodo no crítico antes.
                        for (int j = bestCriticalPos; j > 0; j--)
                        {
                            if (!_problem.FeasibleTravel[solution.Vehicles[v1].Requests[j - 1].ID_Unique][notCriticalNode.ID_Unique]) continue;
                            //var diff = Problem.EuclideanDistance(notCriticalNode.TimeWindow.LowerBound, notCriticalNode.TimeWindow.UpperBound, solution.Vehicles[v2].Requests[j + 1].TimeWindow.LowerBound, solution.Vehicles[v2].Requests[j + 1].TimeWindow.UpperBound);
                            double twDist = 0.0;
                            if (j == 1)
                            {
                                if ((criticalNode.TimeWindow.LowerBound + _problem.Distances[criticalNode.ID_Unique][solution.Vehicles[v1].Requests[j].ID_Unique]) > solution.Vehicles[v1].Requests[j].TimeWindow.UpperBound) continue;
                                twDist = CalculateDistance(notCriticalNode, v1, j, solution);
                            }
                            else
                            {
                                if (((solution.Vehicles[v1].Requests[j - 1].TimeWindow.LowerBound + _problem.Distances[criticalNode.ID_Unique][solution.Vehicles[v1].Requests[j - 1].ID_Unique]) > criticalNode.TimeWindow.UpperBound) ||
                                (criticalNode.TimeWindow.LowerBound + _problem.Distances[criticalNode.ID_Unique][solution.Vehicles[v1].Requests[j].ID_Unique]) > solution.Vehicles[v1].Requests[j].TimeWindow.UpperBound) continue;
                                twDist = (CalculateDistance(notCriticalNode, v1, j, solution) + CalculateDistance(notCriticalNode, v1, j - 1, solution)) / 2.0;
                            }
                            if (twDist < bestCost)
                            {
                                bestPos = j;
                                bestCost = twDist;
                            }
                        }
                        //Insertar en la mejor posición declarada.
                        solution.Vehicles[v1].Requests.Insert(bestPos, notCriticalNode);
                        //break; //terminamos.
                    }
                }


                //CorrectPositions(solution.Vehicles);
                //Recalcular con el proceso de 8 pasos y actualizar el coste.
                foreach (var vehicle in solution.Vehicles)
                {
                    DARPAlgorithms.EightStepsEvaluationProcedure(ref solution, vehicle, _problem);
                }


                solution.TotalDuration = solution.Vehicles.Sum(t => t.VehicleDuration);
                solution.TimeWindowViolation = solution.Vehicles.Sum(t => t.TotalTimeWindowViolation);
                solution.RideTimeViolation = solution.Vehicles.Sum(r => r.TotalRideTimeViolation);
                solution.DurationViolation = solution.Vehicles.Sum(d => d.TotalDurationViolation);
                solution.LoadViolation = solution.Vehicles.Sum(l => l.TotalLoadViolation);
                solution.TotalWaitingTime = solution.Vehicles.Sum(t => t.TotalWaitingTime);
                solution.Fitness = Costs.EvaluationSolution(solution, _problem);

                Validator.ValidateSolution(ref solution, _problem);



            }
            catch (Exception ex)
            {
                Console.WriteLine();
            }

        }

        private bool ValidTimeWindow2(Solution optSolution, Request source, Problem _problem, int target, int v)
        {
            return (((optSolution.Vehicles[v].Requests[target - 1].DepartureTime - optSolution.Vehicles[v].Requests[target - 1].SlackTime) + _problem.Distances[optSolution.Vehicles[v].Requests[target - 1].ID_Unique][source.ID_Unique]) <= source.TimeWindow.UpperBound);

        }

        private bool ValidTimeWindow(Solution optSolution, Request source, Problem _problem, int target, int v)
        {
            return ((optSolution.Vehicles[v].Requests[target].TimeWindow.LowerBound + _problem.Distances[optSolution.Vehicles[v].Requests[target].ID_Unique][source.ID_Unique]) <= source.TimeWindow.UpperBound);

        }


        private void CorrectPositions(List<Vehicle> vehicles)
        {
            foreach (var veh in vehicles)
            {
                List<int> visited = new List<int>();
                var requests = veh.Requests;
                for (int i = 1; i < requests.Count - 1; i++)
                {
                    if (visited.Contains(requests[i].PairID))
                        requests[i].Origin = false;
                    else
                    {
                        requests[i].Origin = true;
                        visited.Add(requests[i].PairID);
                    }
                }
            }
        }


        private double CalculateDiff(Solution optSolution, Request source, Problem _problem, int target, int v)
        {
            return Math.Abs((source.TimeWindow.LowerBound - ((optSolution.Vehicles[v].Requests[target - 1].DepartureTime - optSolution.Vehicles[v].Requests[target - 1].SlackTime) + _problem.Distances[optSolution.Vehicles[v].Requests[target - 1].ID_Unique][source.ID_Unique])));
        }







        #endregion
    }
}





