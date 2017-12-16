using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.DARP.Data.Model;
using API.DARP.Data.Model.Settings;
using API.DARP.Data;
using API.DARP.Calculations.Metaheuristics.VNS;
using System.Diagnostics;
using API.DARP.Data.Model.Summary;
using API.DARP.Calculations.Algorithms;
using API.DARP.Calculations.Validation;

namespace API.DARP.Calculations.Metaheuristics.ILS
{
    internal class ILSMetaheuristic : IMetaheuristic
    {
        private const int _DIVERSIFY_MAX = 10;
        private VNSMetaheuristic _vns;
        private Random _random;
        private Problem _problem;
        private ILSConfigurationSettings _ilsSettings;
        private Perturbations _perturbations;
        private int _diversify;
        static double _coef_Updater_Splash;  //Coeficiente de actualizaciónd e Splash

        public ILSMetaheuristic(IHeuristicConfigurationSetting setting, Problem problem, Random random)
        {
            _ilsSettings = ((ILSConfigurationSettings)setting);
            _random = random;
            _problem = problem;
            _diversify = _DIVERSIFY_MAX;
            _vns = new VNSMetaheuristic(this._ilsSettings, _problem, _random);
            _perturbations = new Perturbations(_random, problem);
            _coef_Updater_Splash = 100 / (double)_ilsSettings.MaxILSIterations;
        }

        IHeuristicConfigurationSetting IMetaheuristic.HeuristicSettings
        {
            get; set;
        }

        public void ExecuteMetaheuristic(ref Solution bestSolution)
        {
            int bestIteration = 0;
            Solution partialSolution = GenericCopier<Solution>.DeepCopy(bestSolution);
            int control_Diversify = 0;
            SummaryDetails summary = new SummaryDetails();
            Splash.SplashGlobalData.SetSplashData(Constants.SPLASH_BEST_SOLUTION, bestSolution.Fitness);
            double pertCost = 0, vnsCost = 0;

            int iterations = 0;
            //Guardar coste inicial.
            summary.ILSSummary.InitCost = bestSolution.Parc_Cost;
            for (int i = 0; iterations < _ilsSettings.MaxILSIterations && i < _ilsSettings.MaxILSNoImprovement; i++)
            {
                //Control de intensificación-diversificación.
                if (i % _DIVERSIFY_MAX == 0) _diversify--;
                _diversify = _diversify < 3 ? 3 : _diversify;
                if (i % 100 == 0) _diversify = _DIVERSIFY_MAX;
                //Guardar valores estadísticos.
                var initCost = partialSolution.Parc_Cost;
                var initFitness = partialSolution.Fitness;

                Splash.SplashGlobalData.SetSplashData(Constants.SPLASH_ILS_TOTAL_ITERATIONS, iterations + 1);
                //Apply VNS local search.        
                _vns.ExecuteMetaheuristic(ref partialSolution);


                //Validator.ValidateSolution(ref partialSolution, _problem);
                bool feasible = partialSolution.Feasible;
                //Guardar valores estadísticos.
                vnsCost = Costs.ParcialCostSolution(partialSolution, _problem); // partialSolution.Parc_Cost;
                var vnsFitness = partialSolution.Fitness;

                if (vnsFitness < initFitness)
                    summary.ILSSummary.TotalImpPrevious++; //Incrementar el número de veces que se mejora la solución previa (perturbada).


                if (bestSolution.Feasible)
                {
                    if (partialSolution.Feasible && (Math.Round(partialSolution.Fitness, 2) < Math.Round(bestSolution.Fitness, 2)))
                    {
                        bestSolution = GenericCopier<Solution>.DeepCopy(partialSolution);
                        //Actualizar la mejor solución con los nuevos parámetros.
                        control_Diversify = 0;
                        bestIteration = i;
                        i = 0;
                        _diversify = _DIVERSIFY_MAX;
                        summary.ILSSummary.BestIteration = iterations + 1; //Guardar mejor iteración encontrada.
                        summary.ILSSummary.TotalImpBest++; //Incrementar el número de veces que se mejora la mejor solución.
                        Singleton.Instance.UpdatePenaltyTerms(partialSolution);
                        Splash.SplashGlobalData.SetSplashData(Constants.SPLASH_ILS_IMPROVEMENTS, summary.ILSSummary.TotalImpBest);
                        Splash.SplashGlobalData.SetSplashData(Constants.SPLASH_BEST_SOLUTION, Math.Round(bestSolution.Fitness, 2));
                    }
                    else
                    {
                        control_Diversify++;
                    }
                }
                else
                {
                    if (partialSolution.Feasible || Math.Round(partialSolution.Fitness, 2) < Math.Round(bestSolution.Fitness, 2))
                    {
                        if (partialSolution.Feasible && summary.ILSSummary.FirstItFeasible == null)
                            summary.ILSSummary.FirstItFeasible = iterations + 1; //Guardar primera iteración factible.
                        bestSolution = GenericCopier<Solution>.DeepCopy(partialSolution);
                        control_Diversify = 0;
                        i = 0;
                        _diversify = _DIVERSIFY_MAX;
                        summary.ILSSummary.BestIteration = iterations + 1; //Guardar mejor iteración encontrada.
                        summary.ILSSummary.TotalImpBest++; //Incrementar el número de veces que se mejora la mejor solución.
                        Singleton.Instance.UpdatePenaltyTerms(partialSolution);
                        //bestSolution.Fitness = Costs.EvaluationSolution(bestSolution, _problem);
                        Splash.SplashGlobalData.SetSplashData(Constants.SPLASH_ILS_IMPROVEMENTS, summary.ILSSummary.TotalImpBest);
                        Splash.SplashGlobalData.SetSplashData(Constants.SPLASH_BEST_SOLUTION, bestSolution.Fitness);
                    }
                    else
                    {
                        control_Diversify++;
                    }
                }

                if (control_Diversify > _diversify)
                {
                    partialSolution = GenericCopier<Solution>.DeepCopy(bestSolution);
                    control_Diversify = 0;
                }

                var perturbation = _perturbations.ExecutePerturbation(ref partialSolution);
                //Guardar valores estadísticos.
                pertCost = partialSolution.Parc_Cost;
                var pertFitness = partialSolution.Fitness;

                //Añadir resumen de la iteración.
                summary.VNSOperators.Add(new VNSOperators(iterations, _vns.ShiftInter, 0, _vns.WRI_IntraOP, _vns.IntraRouteInsertionOP, _vns.IntraSwap));
                summary.ILSEvolution.Add(new ILSEvolution(iterations, Math.Round(initCost, 2), Math.Round(initFitness, 2), Math.Round(vnsCost, 2), Math.Round(vnsFitness, 2), perturbation, Math.Round(pertFitness, 2), Math.Round(pertCost, 2), feasible));
                Splash.SplashGlobalData.SetSplashData(Constants.SPLASH_PROGRESS, Convert.ToDouble(Splash.SplashGlobalData.GetSplashData<double>(Constants.SPLASH_PROGRESS)) + _coef_Updater_Splash);
                //Guardar cambios de cada operador.



                iterations++;
            }

            //Recalcular con el proceso de 8 pasos y actualizar el coste.
            foreach (var vehicle in bestSolution.Vehicles)
            {
                DARPAlgorithms.EightStepsEvaluationProcedure(ref bestSolution, vehicle, _problem);
            }
            bestSolution.TotalDuration = bestSolution.Vehicles.Sum(t => t.VehicleDuration);
            bestSolution.TimeWindowViolation = bestSolution.Vehicles.Sum(t => t.TotalTimeWindowViolation);
            bestSolution.RideTimeViolation = bestSolution.Vehicles.Sum(r => r.TotalRideTimeViolation);
            bestSolution.DurationViolation = bestSolution.Vehicles.Sum(d => d.TotalDurationViolation);
            bestSolution.LoadViolation = bestSolution.Vehicles.Sum(l => l.TotalLoadViolation);
            bestSolution.TotalWaitingTime = bestSolution.Vehicles.Sum(t => t.TotalWaitingTime);
            DARPAlgorithms.UpdateConstraintsSolution(ref bestSolution, _problem);
            bestSolution.Fitness = Costs.EvaluationSolution(bestSolution, _problem);
            bestSolution.Parc_Cost = Costs.ParcialCostSolution(bestSolution, _problem);

            summary.ILSSummary.TotalIterations = iterations + 1;
            summary.ILSSummary.FinalCost = bestSolution.Parc_Cost;


            bestSolution.SummaryDetails = summary;
        }
    }
}
