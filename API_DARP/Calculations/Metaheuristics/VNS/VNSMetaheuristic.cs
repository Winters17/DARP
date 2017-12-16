

using API.DARP.Calculations.Algorithms;
using API.DARP.Calculations.Validation;
using API.DARP.Data;
using API.DARP.Data.Model;
using API.DARP.Data.Model.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace API.DARP.Calculations.Metaheuristics.VNS
{


    internal class VNSMetaheuristic : IMetaheuristic
    {

        private Random _random;
        private Problem _problem;
        private Dictionary<int, string> _interSearchNeighborhood;
        private Dictionary<int, string> _intraSearchNeighborhood;

        internal int ShiftInter;
        internal int WRI_IntraOP;
        internal int IntraRouteInsertionOP;
        internal int IntraSwap;

        public VNSMetaheuristic(IHeuristicConfigurationSetting setting, Problem problem, Random random)
        {
            HeuristicSettings = setting as ILSConfigurationSettings;
            _random = random;
            _problem = problem;
        }

        public IHeuristicConfigurationSetting HeuristicSettings
        {
            get; set;
        }

        public void ExecuteMetaheuristic(ref Solution bestSolution)
        {
            ShiftInter = 0;
            IntraRouteInsertionOP = 0;
            WRI_IntraOP = 0;
            IntraSwap = 0;

            Debug.WriteLine("\n\nNueva búsuqeda: ");
            bool ls1 = true;
            bool ls2 = true;
            bool ls3 = true;
            bool ls4 = true;
            while (ls1 || ls2 || ls3 || ls4)
            {
                List<int> localInternal = Enumerable.Range(0,3).ToList();
                do
                {
                    var variable = localInternal[_random.Next(0, localInternal.Count)];
                    switch(variable)
                    {
                        case 0:
                            ls1 = WRI_Intra(ref bestSolution);
                            localInternal.Remove(0);
                            break;
                        case 1:
                            ls3 = IntraRouteInsertion(ref bestSolution);
                            localInternal.Remove(1);
                            break;
                        case 2:
                            ls4 = Swap_Intra(ref bestSolution);
                            localInternal.Remove(2);
                            break;
                    }
                }
                while (localInternal.Count > 0);
                //ls4 = Swap_Intra(ref bestSolution);
                //ls1 = WRI_Intra(ref bestSolution);
                ls2 = Shift1_0Inter(ref bestSolution);
                //ls3 = IntraRouteInsertion(ref bestSolution);
                //ls1 = false;
                //ls2 = false;
                //ls3 = false;
                //ls4 = false;
            }



            DARPAlgorithms.UpdateConstraintsSolution(ref bestSolution, _problem);
            CheckFesiability(ref bestSolution);
        }

        private void CreateIntraSearchNeighborhood()
        {
            _intraSearchNeighborhood = new Dictionary<int, string>();
            _intraSearchNeighborhood.Add(3, Constants.SHIFT1_0_INTRA_OPT);

        }

        private void CheckFesiability(ref Solution bestSolution)
        {
            if (Math.Round(bestSolution.DurationViolation, 2) <= 0.01 && bestSolution.LoadViolation == 0 && Math.Round(bestSolution.RideTimeViolation, 2) <= 0.01 && Math.Round(bestSolution.TimeWindowViolation, 2) <= 0.01)
                bestSolution.Feasible = true;
            else
                bestSolution.Feasible = false;
        }

        private void CreateInterSearchNeighborhood()
        {
            _interSearchNeighborhood = new Dictionary<int, string>();
            _interSearchNeighborhood.Add(0, Constants.SWAP1_1_INTER);
        }


        #region IntraLocalSearch

        private void IntraLocalSearch(ref Solution solution, string neighborhood)
        {
            switch (neighborhood)
            {
                case Constants.WRI_INTRA:
                    WRI_Intra(ref solution);
                    break;
                case Constants.SWAP_INTRA:
                    Swap_Intra(ref solution);
                    break;
                case Constants.SHIFT1_0_INTRA_OPT:
                    IntraRouteInsertion(ref solution);
                    break;
                default:
                    throw new Exception("Intra Local Search does not founded");
            }
        }

        #region IntraRouteInsertion



        private bool IntraRouteInsertion(ref Solution solution)
        {
            bool improvement = false;
            Solution optSolution = solution;
            double bestCost = optSolution.Fitness;
            Init:
            var vehicle = optSolution.Vehicles;
            for (int v = 0; v < vehicle.Count; v++)
            {
                if (!vehicle[v].Perturbed)
                    continue;
                //Vehicle selection.
                for (int i = 1; i < vehicle[v].Requests.Count - 1; i++)
                {
                    var critical = vehicle[v].Requests[i];
                    var criticalPos = i;
                    if (!critical.IsCritical) continue;
                    var notCritical = vehicle[v].Requests.Find(t => t.PairID == critical.PairID && t.Origin != critical.Origin);
                    var notCriticalPos = vehicle[v].Requests.IndexOf(notCritical);
                    vehicle[v].Requests.RemoveAll(t => t.PairID == critical.PairID);
                    for (int j = 1; j < vehicle[v].Requests.Count - 1; j++)
                    {
                        if (CheckTimeWindowShift1_0(ref optSolution, vehicle[v].Requests, critical, j))
                        {
                            vehicle[v].Requests.Insert(j, critical);
                            if (critical.Origin)
                            {
                                int index_Target = j + 1;
                                while (index_Target < vehicle[v].Requests.Count) //Desde la posición actual a la primera posición.
                                {
                                    if (CheckTimeWindowShift1_0(ref optSolution, vehicle[v].Requests, notCritical, index_Target))
                                    {
                                        vehicle[v].Requests.Insert(index_Target, notCritical);
                                        //Salvar estados vehículo actuales.
                                        var v1DurV = vehicle[v].TotalDurationViolation;
                                        var v1TWV = vehicle[v].TotalTimeWindowViolation;
                                        var v1RTV = vehicle[v].TotalRideTimeViolation;
                                        var v1LV = vehicle[v].TotalLoadViolation;
                                        var v1TWT = vehicle[v].TotalWaitingTime;
                                        var v1Dur = vehicle[v].VehicleDuration;

                                        DARPAlgorithms.EightStepsEvaluationProcedure(ref optSolution, optSolution.Vehicles[v], _problem);

                                        //Actualizar estado de la solución.
                                        var vd = optSolution.Vehicles.Sum(t => t.VehicleDuration);
                                        var twv = optSolution.Vehicles.Sum(t => t.TotalTimeWindowViolation);
                                        var rtv = optSolution.Vehicles.Sum(r => r.TotalRideTimeViolation);
                                        var tdv = optSolution.Vehicles.Sum(d => d.TotalDurationViolation);
                                        var lv = optSolution.Vehicles.Sum(l => l.TotalLoadViolation);
                                        var wt = optSolution.Vehicles.Sum(t => t.TotalWaitingTime);

                                        var cost = Costs.EvaluationSolution(ref optSolution, twv, rtv, tdv, lv, _problem);
                                        if (Math.Round(cost, 2) < Math.Round(bestCost, 2))
                                        {
                                            bestCost = cost;
                                            IntraRouteInsertionOP++;
                                            optSolution.TotalDuration = vd;
                                            optSolution.TimeWindowViolation = twv;
                                            optSolution.RideTimeViolation = rtv;
                                            optSolution.DurationViolation = tdv;
                                            optSolution.LoadViolation = lv;
                                            optSolution.TotalWaitingTime = wt;
                                            improvement = true;
                                            goto Init;
                                        }
                                        else
                                        {
                                            //Deshacer el movimiento de inserción.
                                            vehicle[v].Requests.Remove(notCritical);
                                            //Copiar estados anteriores a las modificaciones.
                                            optSolution.Vehicles[v].TotalDurationViolation = v1DurV;
                                            optSolution.Vehicles[v].TotalTimeWindowViolation = v1TWV;
                                            optSolution.Vehicles[v].TotalRideTimeViolation = v1RTV;
                                            optSolution.Vehicles[v].TotalLoadViolation = v1LV;
                                            optSolution.Vehicles[v].TotalWaitingTime = v1TWT;
                                            optSolution.Vehicles[v].VehicleDuration = v1Dur;
                                            //if (vehicle[v].Requests[index_Target - 1].DepartureTime - vehicle[v].Requests[index_Target - 1].SlackTime + _problem.Distances[vehicle[v].Requests[index_Target - 1].ID_Unique][notCritical.ID_Unique] > notCritical.TimeWindow.UpperBound)
                                            //    break;
                                        }
                                    }
                                    index_Target++;
                                }
                            }
                            else
                            {
                                int index_Target = j;
                                while (index_Target > 0) //mientras no se llegue a la petición destino.
                                {
                                    if (CheckTimeWindowShift1_0(ref optSolution, vehicle[v].Requests, notCritical, index_Target))
                                    {
                                        vehicle[v].Requests.Insert(index_Target, notCritical);
                                        //Marcar rutas que han sido modificadas.
                                        var v1DurV = optSolution.Vehicles[v].TotalDurationViolation;
                                        var v1TWV = optSolution.Vehicles[v].TotalTimeWindowViolation;
                                        var v1RTV = optSolution.Vehicles[v].TotalRideTimeViolation;
                                        var v1LV = optSolution.Vehicles[v].TotalLoadViolation;
                                        var v1TWT = optSolution.Vehicles[v].TotalWaitingTime;
                                        var v1Dur = optSolution.Vehicles[v].VehicleDuration;
                                        DARPAlgorithms.EightStepsEvaluationProcedure(ref optSolution, optSolution.Vehicles[v], _problem);

                                        //Actualizar estado de la solución.
                                        var vd = optSolution.Vehicles.Sum(t => t.VehicleDuration);
                                        var twv = optSolution.Vehicles.Sum(t => t.TotalTimeWindowViolation);
                                        var rtv = optSolution.Vehicles.Sum(r => r.TotalRideTimeViolation);
                                        var tdv = optSolution.Vehicles.Sum(d => d.TotalDurationViolation);
                                        var lv = optSolution.Vehicles.Sum(l => l.TotalLoadViolation);
                                        var wt = optSolution.Vehicles.Sum(t => t.TotalWaitingTime);

                                        var cost = Costs.EvaluationSolution(ref optSolution, twv, rtv, tdv, lv, _problem);
                                        if (Math.Round(cost, 2) < Math.Round(bestCost, 2))
                                        {
                                            bestCost = cost;
                                            IntraRouteInsertionOP++;
                                            optSolution.TotalDuration = vd;
                                            optSolution.TimeWindowViolation = twv;
                                            optSolution.RideTimeViolation = rtv;
                                            optSolution.DurationViolation = tdv;
                                            optSolution.LoadViolation = lv;
                                            optSolution.TotalWaitingTime = wt;
                                            improvement = true;
                                            goto Init;
                                        }
                                        else
                                        {
                                            //Deshacer el movimiento Swap1_1
                                            vehicle[v].Requests.Remove(notCritical);
                                            //Copiar estados anteriores a las modificaciones.
                                            optSolution.Vehicles[v].TotalDurationViolation = v1DurV;
                                            optSolution.Vehicles[v].TotalTimeWindowViolation = v1TWV;
                                            optSolution.Vehicles[v].TotalRideTimeViolation = v1RTV;
                                            optSolution.Vehicles[v].TotalLoadViolation = v1LV;
                                            optSolution.Vehicles[v].TotalWaitingTime = v1TWT;
                                            optSolution.Vehicles[v].VehicleDuration = v1Dur;
                                             //if (vehicle[v].Requests[index_Target - 1].DepartureTime - vehicle[v].Requests[index_Target - 1].SlackTime + _problem.Distances[vehicle[v].Requests[index_Target - 1].ID_Unique][notCritical.ID_Unique] > notCritical.TimeWindow.UpperBound)
                                            //     break;
                                        }
                                    }
                                    index_Target--;
                                }
                            }
                            vehicle[v].Requests.Remove(critical);
                        }
                    }
                    if (criticalPos < notCriticalPos)
                    {
                        vehicle[v].Requests.Insert(criticalPos, critical);
                        vehicle[v].Requests.Insert(notCriticalPos, notCritical);
                    }
                    else
                    {
                        vehicle[v].Requests.Insert(notCriticalPos, notCritical);
                        vehicle[v].Requests.Insert(criticalPos, critical);
                    }
                }
            }
            optSolution.Fitness = bestCost;
            return improvement;
        }



        #endregion

        #region WRI_Intra

        private bool WRI_Intra(ref Solution solution)
        {
            bool improvement = false;
            Solution optSolution = solution;
            double bestCost = optSolution.Fitness;
            double parcCost = optSolution.Parc_Cost;
            Init:
            var vehicle = optSolution.Vehicles;
            for (int v = 0; v < vehicle.Count; v++)
            {
                if (!vehicle[v].Perturbed)
                    continue; //NO hay sido modificado.
                //Vehicle selection.
                var requests = vehicle[v].Requests;
                for (int i = 1; i < requests.Count - 1; i++)
                {
                    var node = requests[i];
                    requests.RemoveAt(i); //Eliminar de su posición de origen.
                    if (node.Origin)
                    {
                        var index_Destination = requests.IndexOf(requests.Find(t => t.PairID == node.PairID && t.Origin != node.Origin));
                        for (int j = 1; j <= index_Destination; j++)
                        {
                            if (!CheckTimeWindowShift1_0(ref optSolution, requests, node, j))
                                continue;
                            requests.Insert(j, node); //Insertar el nodo en su nueva posición.

                            var v1DurV = optSolution.Vehicles[v].TotalDurationViolation;
                            var v1TWV = optSolution.Vehicles[v].TotalTimeWindowViolation;
                            var v1RTV = optSolution.Vehicles[v].TotalRideTimeViolation;
                            var v1LV = optSolution.Vehicles[v].TotalLoadViolation;
                            var v1TWT = optSolution.Vehicles[v].TotalWaitingTime;
                            var v1Dur = optSolution.Vehicles[v].VehicleDuration;
                            DARPAlgorithms.EightStepsEvaluationProcedure(ref optSolution, optSolution.Vehicles[v], _problem);


                            //Actualizar estado de la solución.
                            var vd = optSolution.Vehicles.Sum(t => t.VehicleDuration);
                            var twv = optSolution.Vehicles.Sum(t => t.TotalTimeWindowViolation);
                            var rtv = optSolution.Vehicles.Sum(r => r.TotalRideTimeViolation);
                            var tdv = optSolution.Vehicles.Sum(d => d.TotalDurationViolation);
                            var lv = optSolution.Vehicles.Sum(l => l.TotalLoadViolation);
                            var wt = optSolution.Vehicles.Sum(t => t.TotalWaitingTime);

                            var cost = Costs.EvaluationSolution(ref optSolution, twv, rtv, tdv, lv, _problem);
                            if (Math.Round(cost, 2) < Math.Round(bestCost, 2))
                            {
                                bestCost = cost;
                                optSolution.TotalDuration = vd;
                                optSolution.TimeWindowViolation = twv;
                                optSolution.RideTimeViolation = rtv;
                                optSolution.DurationViolation = tdv;
                                optSolution.LoadViolation = lv;
                                optSolution.TotalWaitingTime = wt;
                                WRI_IntraOP++;
                                improvement = true;
                                goto Init;
                            }
                            else
                            {
                                //Deshacer el movimiento SHIFT
                                requests.RemoveAt(j); //Eliminar de su posición de origen.
                                optSolution.Vehicles[v].TotalDurationViolation = v1DurV;
                                optSolution.Vehicles[v].TotalTimeWindowViolation = v1TWV;
                                optSolution.Vehicles[v].TotalRideTimeViolation = v1RTV;
                                optSolution.Vehicles[v].TotalLoadViolation = v1LV;
                                optSolution.Vehicles[v].TotalWaitingTime = v1TWT;
                                optSolution.Vehicles[v].VehicleDuration = v1Dur;
                            }
                        }
                        requests.Insert(i, node); //No ha habido cambios e insertarlo en su posición original
                    }
                    else
                    {
                        var index_Destination = requests.IndexOf(requests.Find(t => t.PairID == node.PairID && t.Origin != node.Origin));
                        for (int j = requests.Count - 1; j > index_Destination; j--)
                        {
                            if (!CheckTimeWindowShift1_0(ref optSolution, requests, node, j)) continue;
                            requests.Insert(j, node); //Insertar el nodo en su nueva posición.

                            var v1DurV = optSolution.Vehicles[v].TotalDurationViolation;
                            var v1TWV = optSolution.Vehicles[v].TotalTimeWindowViolation;
                            var v1RTV = optSolution.Vehicles[v].TotalRideTimeViolation;
                            var v1LV = optSolution.Vehicles[v].TotalLoadViolation;
                            var v1TWT = optSolution.Vehicles[v].TotalWaitingTime;
                            var v1Dur = optSolution.Vehicles[v].VehicleDuration;
                            DARPAlgorithms.EightStepsEvaluationProcedure(ref optSolution, optSolution.Vehicles[v], _problem);


                            //Actualizar estado de la solución.
                            var vd = optSolution.Vehicles.Sum(t => t.VehicleDuration);
                            var twv = optSolution.Vehicles.Sum(t => t.TotalTimeWindowViolation);
                            var rtv = optSolution.Vehicles.Sum(r => r.TotalRideTimeViolation);
                            var tdv = optSolution.Vehicles.Sum(d => d.TotalDurationViolation);
                            var lv = optSolution.Vehicles.Sum(l => l.TotalLoadViolation);
                            var wt = optSolution.Vehicles.Sum(t => t.TotalWaitingTime);

                            var cost = Costs.EvaluationSolution(ref optSolution, twv, rtv, tdv, lv, _problem);
                            if (Math.Round(cost, 2) < Math.Round(bestCost, 2))
                            {
                                bestCost = cost;
                                optSolution.TotalDuration = vd;
                                optSolution.TimeWindowViolation = twv;
                                optSolution.RideTimeViolation = rtv;
                                optSolution.DurationViolation = tdv;
                                optSolution.LoadViolation = lv;
                                optSolution.TotalWaitingTime = wt;
                                WRI_IntraOP++;
                                improvement = true;
                                goto Init;
                            }
                            else
                            {
                                //Deshacer el movimiento SHIFT
                                requests.RemoveAt(j); //Eliminar de su posición de origen.
                                optSolution.Vehicles[v].TotalDurationViolation = v1DurV;
                                optSolution.Vehicles[v].TotalTimeWindowViolation = v1TWV;
                                optSolution.Vehicles[v].TotalRideTimeViolation = v1RTV;
                                optSolution.Vehicles[v].TotalLoadViolation = v1LV;
                                optSolution.Vehicles[v].TotalWaitingTime = v1TWT;
                                optSolution.Vehicles[v].VehicleDuration = v1Dur;
                            }
                        }
                        requests.Insert(i, node); //No ha habido cambios e insertarlo en su posición original              
                    }

                }
            }
            optSolution.Fitness = bestCost;
            return improvement;
        }

        private bool CheckTimeWindowShift1_0(ref Solution optSolution, List<Request> requests, Request node, int j)
        {

            bool feasibleSource;
            bool feasibleTarget;
            feasibleSource = _problem.FeasibleTravel[requests[j - 1].ID_Unique][node.ID_Unique];
            feasibleTarget = _problem.FeasibleTravel[node.ID_Unique][requests[j].ID_Unique];

            return feasibleSource && feasibleTarget;


        }

        #endregion

        #region Swap_Intra

        private bool Swap_Intra(ref Solution solution)
        {
            Solution optSolution = solution;
            bool improvement = false;
            double bestCost = optSolution.Fitness;
            double parcCost = optSolution.Parc_Cost;
            Init:
            var vehicle = optSolution.Vehicles;
            for (int v = 0; v < vehicle.Count; v++)
            {
                //Vehicle selection.
                var requests = vehicle[v].Requests;
                if (!vehicle[v].Perturbed)
                    continue; //NO hay sido modificado.
                for (int i = 1; i < requests.Count - 1; i++)
                {
                    var node1V1 = requests[i];
                    if (!node1V1.Origin) continue;
                    var node2V1 = requests.Find(t => t.PairID == node1V1.PairID && t.Origin != node1V1.Origin);
                    var indexNode2V1 = requests.IndexOf(node2V1);
                    for (int j = 1; j < requests.Count - 1; j++)
                    {
                        var node1V2 = requests[j];
                        if (!node1V2.Origin) continue;
                        if (i == j || node1V1.PairID == node1V2.PairID) continue; //omitir mismas posiciones
                        var node2V2 = requests.Find(t => t.PairID == node1V2.PairID && t.Origin != node1V2.Origin);
                        var indexNode2V2 = requests.IndexOf(node2V2);

                        if (!CheckTimeWindowSwap1_1_Intra(ref optSolution, vehicle[v].Requests, i, j) && !_problem.NeighborhoodTW[node1V1.PairID][node1V2.PairID] && !_problem.NeighborhoodTW[node1V2.PairID][node1V1.PairID])
                        {
                            continue;
                        }

                        requests[i] = node1V2;
                        requests[j] = node1V1;
                        requests[indexNode2V1] = node2V2;
                        requests[indexNode2V2] = node2V1;



                        var v1DurV = optSolution.Vehicles[v].TotalDurationViolation;
                        var v1TWV = optSolution.Vehicles[v].TotalTimeWindowViolation;
                        var v1RTV = optSolution.Vehicles[v].TotalRideTimeViolation;
                        var v1LV = optSolution.Vehicles[v].TotalLoadViolation;
                        var v1TWT = optSolution.Vehicles[v].TotalWaitingTime;
                        var v1Dur = optSolution.Vehicles[v].VehicleDuration;
                        DARPAlgorithms.EightStepsEvaluationProcedure(ref optSolution, optSolution.Vehicles[v], _problem);


                        //Actualizar estado de la solución.
                        var vd = optSolution.Vehicles.Sum(t => t.VehicleDuration);
                        var twv = optSolution.Vehicles.Sum(t => t.TotalTimeWindowViolation);
                        var rtv = optSolution.Vehicles.Sum(r => r.TotalRideTimeViolation);
                        var tdv = optSolution.Vehicles.Sum(d => d.TotalDurationViolation);
                        var lv = optSolution.Vehicles.Sum(l => l.TotalLoadViolation);
                        var wt = optSolution.Vehicles.Sum(t => t.TotalWaitingTime);

                        var cost = Costs.EvaluationSolution(ref optSolution, twv, rtv, tdv, lv, _problem);
                        if (Math.Round(cost, 2) < Math.Round(bestCost, 2))
                        {
                            bestCost = cost;
                            optSolution.TotalDuration = vd;
                            optSolution.TimeWindowViolation = twv;
                            optSolution.RideTimeViolation = rtv;
                            optSolution.DurationViolation = tdv;
                            optSolution.LoadViolation = lv;
                            optSolution.TotalWaitingTime = wt;
                            IntraSwap++;
                            improvement = true;
                            goto Init;
                        }
                        else
                        {
                            //Deshacer el movimiento de intercambio
                            requests[i] = node1V1;
                            requests[j] = node1V2;
                            requests[indexNode2V1] = node2V1;
                            requests[indexNode2V2] = node2V2;
                            //Copiar estados anteriores a las modificaciones.
                            optSolution.Vehicles[v].TotalDurationViolation = v1DurV;
                            optSolution.Vehicles[v].TotalTimeWindowViolation = v1TWV;
                            optSolution.Vehicles[v].TotalRideTimeViolation = v1RTV;
                            optSolution.Vehicles[v].TotalLoadViolation = v1LV;
                            optSolution.Vehicles[v].TotalWaitingTime = v1TWT;
                            optSolution.Vehicles[v].VehicleDuration = v1Dur;
                        }
                    }
                }
            }
            optSolution.Fitness = bestCost;
            optSolution.Parc_Cost = parcCost;
            return improvement;
        }

        private bool CheckTimeWindowSwap1_1_Intra(ref Solution optSolution, List<Request> requests, int i, int j)
        {

            //Caso normal.
            bool feasibleTarget = _problem.FeasibleTravel[requests[i - 1].ID_Unique][requests[i].ID_Unique] &&
                  _problem.FeasibleTravel[requests[i].ID_Unique][requests[i + 1].ID_Unique];

            bool feasibleSource = _problem.FeasibleTravel[requests[j - 1].ID_Unique][requests[j].ID_Unique] &&
                 _problem.FeasibleTravel[requests[j].ID_Unique][requests[j + 1].ID_Unique];



            return feasibleSource && feasibleTarget;
        }

        #endregion



        #endregion

        #region InterLocalSearch    


        /// <summary>
        /// Inter Local Search Selector 
        /// </summary>
        private void InterLocalSearch(ref Solution solution, string neighborhood)
        {
            switch (neighborhood)
            {
                case Constants.SHIFT1_0_INTER:
                    Shift1_0Inter(ref solution);
                    break;
                default:
                    throw new Exception("Inter Local Search not founded");
            }
        }




        #region InterSHIFT


        private bool Shift1_0Inter(ref Solution solution)
        {
            Solution optSolution = solution;
            bool improvement = false;
            double bestCost = optSolution.Fitness;
            bool exit = false;
            int v1_aux;
            Init:
            var vehicle = optSolution.Vehicles;
            for (int v1 = 0; v1 < optSolution.Vehicles.Count; v1++)
            {
                if (optSolution.Vehicles[v1].Requests.Count == 4) continue;
                for (int v2 = 0; v2 < optSolution.Vehicles.Count; v2++)
                {
                    if (v1 == v2 || (!optSolution.Vehicles[v1].Perturbed || !optSolution.Vehicles[v2].Perturbed))
                        continue;
                    for (int i = 1; i < optSolution.Vehicles[v1].Requests.Count - 1; i++)
                    {
                        var critical = vehicle[v1].Requests[i];
                        var criticalPos = i;
                        if (!critical.IsCritical) continue;
                        var notCritical = vehicle[v1].Requests.Find(t => t.PairID == critical.PairID && t.Origin != critical.Origin);
                        var notCriticalPos = vehicle[v1].Requests.IndexOf(notCritical);
                        var v1DurV = vehicle[v1].TotalDurationViolation;
                        var v1TWV = vehicle[v1].TotalTimeWindowViolation;
                        var v1RTV = vehicle[v1].TotalRideTimeViolation;
                        var v1LV = vehicle[v1].TotalLoadViolation;
                        var v1TWT = vehicle[v1].TotalWaitingTime;
                        var v1Dur = vehicle[v1].VehicleDuration;

                        vehicle[v1].Requests.RemoveAll(t => t.PairID == critical.PairID);
                        DARPAlgorithms.EightStepsEvaluationProcedure(ref optSolution, optSolution.Vehicles[v1], _problem);

                        for (int j = 1; j < vehicle[v2].Requests.Count - 1; j++)
                        {
                            if (_problem.FeasibleTravel[vehicle[v2].Requests[j - 1].ID_Unique][critical.ID_Unique] && _problem.FeasibleTravel[critical.ID_Unique][vehicle[v2].Requests[j].ID_Unique])
                            {
                                vehicle[v2].Requests.Insert(j, critical);
                                if (critical.Origin)
                                {
                                    int index_Target = j + 1;
                                    while (index_Target < vehicle[v2].Requests.Count) //Desde la posición actual a la primera posición.
                                    {
                                        if (_problem.FeasibleTravel[vehicle[v2].Requests[index_Target - 1].ID_Unique][notCritical.ID_Unique] && _problem.FeasibleTravel[notCritical.ID_Unique][vehicle[v2].Requests[index_Target].ID_Unique])
                                        {
                                            vehicle[v2].Requests.Insert(index_Target, notCritical);
                                            //Salvar estados vehículo actuales.
                                            var v2DurV = vehicle[v2].TotalDurationViolation;
                                            var v2TWV = vehicle[v2].TotalTimeWindowViolation;
                                            var v2RTV = vehicle[v2].TotalRideTimeViolation;
                                            var v2LV = vehicle[v2].TotalLoadViolation;
                                            var v2TWT = vehicle[v2].TotalWaitingTime;
                                            var v2Dur = vehicle[v2].VehicleDuration;


                                            DARPAlgorithms.EightStepsEvaluationProcedure(ref optSolution, optSolution.Vehicles[v2], _problem);

                                            //Actualizar estado de la solución.
                                            var vd = optSolution.Vehicles.Sum(t => t.VehicleDuration);
                                            var twv = optSolution.Vehicles.Sum(t => t.TotalTimeWindowViolation);
                                            var rtv = optSolution.Vehicles.Sum(r => r.TotalRideTimeViolation);
                                            var tdv = optSolution.Vehicles.Sum(d => d.TotalDurationViolation);
                                            var lv = optSolution.Vehicles.Sum(l => l.TotalLoadViolation);
                                            var wt = optSolution.Vehicles.Sum(t => t.TotalWaitingTime);

                                            var cost = Costs.EvaluationSolution(ref optSolution, twv, rtv, tdv, lv, _problem);
                                            if (Math.Round(cost, 2) < Math.Round(bestCost, 2))
                                            {
                                                bestCost = cost;
                                                ShiftInter++;
                                                optSolution.TotalDuration = vd;
                                                optSolution.TimeWindowViolation = twv;
                                                optSolution.RideTimeViolation = rtv;
                                                optSolution.DurationViolation = tdv;
                                                optSolution.LoadViolation = lv;
                                                optSolution.TotalWaitingTime = wt;
                                                improvement = true;
                                                goto Init;
                                            }
                                            else
                                            {
                                                //Deshacer el movimiento de inserción.
                                                vehicle[v2].Requests.Remove(notCritical);
                                                //Copiar estados anteriores a las modificaciones.
                                                optSolution.Vehicles[v2].TotalDurationViolation = v2DurV;
                                                optSolution.Vehicles[v2].TotalTimeWindowViolation = v2TWV;
                                                optSolution.Vehicles[v2].TotalRideTimeViolation = v2RTV;
                                                optSolution.Vehicles[v2].TotalLoadViolation = v2LV;
                                                optSolution.Vehicles[v2].TotalWaitingTime = v2TWT;
                                                optSolution.Vehicles[v2].VehicleDuration = v2Dur;
                                            }
                                        }
                                        index_Target++;
                                    }
                                }
                                else
                                {
                                    int index_Target = j;
                                    while (index_Target > 0) //mientras no se llegue a la petición destino.
                                    {
                                        if (_problem.FeasibleTravel[vehicle[v2].Requests[index_Target - 1].ID_Unique][notCritical.ID_Unique] && _problem.FeasibleTravel[notCritical.ID_Unique][vehicle[v2].Requests[index_Target].ID_Unique])
                                        {
                                            vehicle[v2].Requests.Insert(index_Target, notCritical);
                                            //Marcar rutas que han sido modificadas.
                                            var v2DurV = optSolution.Vehicles[v2].TotalDurationViolation;
                                            var v2TWV = optSolution.Vehicles[v2].TotalTimeWindowViolation;
                                            var v2RTV = optSolution.Vehicles[v2].TotalRideTimeViolation;
                                            var v2LV = optSolution.Vehicles[v2].TotalLoadViolation;
                                            var v2TWT = optSolution.Vehicles[v2].TotalWaitingTime;
                                            var v2Dur = optSolution.Vehicles[v2].VehicleDuration;
                                            DARPAlgorithms.EightStepsEvaluationProcedure(ref optSolution, optSolution.Vehicles[v2], _problem);

                                            //Actualizar estado de la solución.
                                            var vd = optSolution.Vehicles.Sum(t => t.VehicleDuration);
                                            var twv = optSolution.Vehicles.Sum(t => t.TotalTimeWindowViolation);
                                            var rtv = optSolution.Vehicles.Sum(r => r.TotalRideTimeViolation);
                                            var tdv = optSolution.Vehicles.Sum(d => d.TotalDurationViolation);
                                            var lv = optSolution.Vehicles.Sum(l => l.TotalLoadViolation);
                                            var wt = optSolution.Vehicles.Sum(t => t.TotalWaitingTime);

                                            var cost = Costs.EvaluationSolution(ref optSolution, twv, rtv, tdv, lv, _problem);
                                            if (Math.Round(cost, 2) < Math.Round(bestCost, 2))
                                            {
                                                bestCost = cost;
                                                ShiftInter++;
                                                optSolution.TotalDuration = vd;
                                                optSolution.TimeWindowViolation = twv;
                                                optSolution.RideTimeViolation = rtv;
                                                optSolution.DurationViolation = tdv;
                                                optSolution.LoadViolation = lv;
                                                optSolution.TotalWaitingTime = wt;
                                                improvement = true;
                                                goto Init;
                                            }
                                            else
                                            {
                                                //Deshacer el movimiento Swap1_1
                                                vehicle[v2].Requests.Remove(notCritical);
                                                //Copiar estados anteriores a las modificaciones.
                                                optSolution.Vehicles[v2].TotalDurationViolation = v2DurV;
                                                optSolution.Vehicles[v2].TotalTimeWindowViolation = v2TWV;
                                                optSolution.Vehicles[v2].TotalRideTimeViolation = v2RTV;
                                                optSolution.Vehicles[v2].TotalLoadViolation = v2LV;
                                                optSolution.Vehicles[v2].TotalWaitingTime = v2TWT;
                                                optSolution.Vehicles[v2].VehicleDuration = v2Dur;
                                            }
                                        }
                                        index_Target--;
                                    }
                                }
                                vehicle[v2].Requests.Remove(critical);
                            }
                        }
                        if (criticalPos < notCriticalPos)
                        {
                            vehicle[v1].Requests.Insert(criticalPos, critical);
                            vehicle[v1].Requests.Insert(notCriticalPos, notCritical);
                            exit = true;
                        }
                        else
                        {
                            vehicle[v1].Requests.Insert(notCriticalPos, notCritical);
                            vehicle[v1].Requests.Insert(criticalPos, critical);
                            exit = true;
                            v1_aux = v1;
                        }
                        vehicle[v1].TotalDurationViolation = v1DurV;
                        vehicle[v1].TotalTimeWindowViolation = v1TWV;
                        vehicle[v1].TotalRideTimeViolation = v1RTV;
                        vehicle[v1].TotalLoadViolation = v1LV;
                        vehicle[v1].TotalWaitingTime = v1TWT;
                        vehicle[v1].VehicleDuration = v1Dur;
                    }
                }
            }

            optSolution.Fitness = bestCost;
            return improvement;
        }


        #endregion

        #endregion


    }
}
