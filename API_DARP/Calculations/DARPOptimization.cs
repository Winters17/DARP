using API.DARP.Calculations.Algorithms;
using API.DARP.Calculations.Metaheuristics;
using API.DARP.Data;
using API.DARP.Data.Model;
using API.DARP.PSAPI.Processes;
using API.DARP.Splash;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DARP.Calculations
{
    public class DARPOptimization
    {

        private static Random _random;

        public static List<Solution> Optimization(RunMetaheuristicInput input)
        {
            var problems = input.Problems;
            //Set randomized value.
            _random = input.Random;
            List<Solution> Solutions = new List<Solution>();

            var default_Metaheuristic = Constants.TypeMetaheuristics.ILS;

            IMetaheuristic metaheuristic = MetaheuristicFactory.CreateMetaheuristic(default_Metaheuristic, input.HeuristicSettings, input.Problems.First(), input.Random);
            


            //Ejecutar cada problema.            
            foreach (var problem in problems)
            {
                //Primer paso. Preprocesar los problemas (ajustar ventanas de tiempo cuando sea posible).
                SplashGlobalData.SetSplashData(Constants.SPLASH_NAME_PROBLEM, problem.ID_Problem);
                SplashGlobalData.SetSplashData(Constants.SPLASH_PROGRESS, 0.0);
                //Create Solution                
                for (int number_Repetition = 0; number_Repetition < input.HeuristicSettings.MaxRepetitions; number_Repetition++)
                {
                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    Solution solution = new Solution(problem.ID_Problem);
                    DARPAlgorithms.BuildInitialSolution(ref solution, problem, ref _random);
                    solution.InitialSolution = GenericCopier<Solution>.DeepCopy(solution);
                    //Update Splash.
                    SplashGlobalData.SetSplashData(Constants.SPLASH_NUMBER_REPETITION, number_Repetition+1);
                    SplashGlobalData.SetSplashData(Constants.SPLASH_PROGRESS, Convert.ToDouble(SplashGlobalData.GetSplashData<double>(Constants.SPLASH_PROGRESS)) + 1);

                    //Execute Heuristic
                    metaheuristic.ExecuteMetaheuristic(ref solution);
                    watch.Stop();
                    solution.ExecutionTime = watch.ElapsedMilliseconds;
                    Solutions.Add(solution);
                    //Validation.Validator.ValidateSolution(ref solution, problem);
                }
            }

            return Solutions;
        }

    }
}
