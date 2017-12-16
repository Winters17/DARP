using API.DARP.Splash;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARP.Splash
{
    public class SplashInfo : INotifyPropertyChanged
    {

        #region Bindings

        #region ProgressValue 

        private int progressValue;

        public int ProgressValue
        {
            get
            {
                return progressValue;
            }

            set
            {
                if (value != progressValue)
                {
                    progressValue = value;
                    // OnProgressValueChanged();
                    NotifyPropertyChanged(nameof(ProgressValue));
                }
            }
        }

        #endregion

        #region ILSImprovements
        private string ilsimprovements;

        public string ILSImprovements
        {
            get
            {
                return ilsimprovements;
            }

            set
            {
                if (value != ilsimprovements)
                {
                    ilsimprovements = value;
                    // OnILSRepetitionChanged();
                    NotifyPropertyChanged(nameof(ILSImprovements));
                }
            }
        }

        #endregion

        #region TotalIterations 

        private string totalIterations;

        public string TotalIterations
        {
            get
            {
                return totalIterations;
            }

            set
            {
                if (value != totalIterations)
                {
                    totalIterations = value;
                    // OnTotalIterationsChanged();
                    NotifyPropertyChanged(nameof(TotalIterations));
                }
            }
        }

        #endregion


        #region NumberProblem 

        private string numberProblem;

        public string NumberProblem
        {
            get
            {
                return numberProblem;
            }

            set
            {
                if (value != numberProblem)
                {
                    numberProblem = value;
                    // OnInfoChanged();
                    NotifyPropertyChanged(nameof(NumberProblem));
                }
            }
        }

        #endregion

        #region NumberRepetition 

        private string numberRepetition;

        public string NumberRepetition
        {
            get
            {
                return numberRepetition;
            }

            set
            {
                if (value != numberRepetition)
                {
                    numberRepetition = value;
                    // OnTitleChanged();
                    NotifyPropertyChanged(nameof(NumberRepetition));
                }
            }
        }

        #endregion

        #region BestSolution 

        private string bestSolution;

        public string BestSolution
        {
            get
            {
                return bestSolution;
            }

            set
            {
                if (value != bestSolution)
                {
                    bestSolution = value;
                    // OnBestSolutionChanged();
                    NotifyPropertyChanged(nameof(BestSolution));
                }
            }
        }

        #endregion



        public SplashInfo(int numberProblem = 0, int numberRepetition = 0,int improvements = 0, int totalIterations = 0, double bestSolution = 0,double percentage = 0.0)
        {
            SplashGlobalData.SetSplashData(API.DARP.Constants.SPLASH_NAME_PROBLEM, numberProblem);
            SplashGlobalData.SetSplashData(API.DARP.Constants.SPLASH_NUMBER_REPETITION, numberRepetition);
            SplashGlobalData.SetSplashData(API.DARP.Constants.SPLASH_PROGRESS, percentage);
            SplashGlobalData.SetSplashData(API.DARP.Constants.SPLASH_ILS_IMPROVEMENTS, improvements);
            SplashGlobalData.SetSplashData(API.DARP.Constants.SPLASH_TOTAL_ITERATIONS, totalIterations);
            SplashGlobalData.SetSplashData(API.DARP.Constants.SPLASH_BEST_SOLUTION, bestSolution);
        }


        #endregion


        #region Methods

        internal void UpdateInfo()
        {            
            // Porcentaje de avance
            int dataProgress = Convert.ToInt32(SplashGlobalData.GetSplashData<double>(API.DARP.Constants.SPLASH_PROGRESS));
            if (dataProgress > 100)
            {
                SplashGlobalData.SetSplashData(API.DARP.Constants.SPLASH_PROGRESS, 0);
                dataProgress = 0;
            }
            else if (dataProgress < 0)
                dataProgress = 0;

            //Numero de problema
            string problem = SplashGlobalData.GetSplashData<string>(API.DARP.Constants.SPLASH_NAME_PROBLEM).ToString();
            string repetition = SplashGlobalData.GetSplashData<string>(API.DARP.Constants.SPLASH_NUMBER_REPETITION).ToString();
            string improvements = SplashGlobalData.GetSplashData<string>(API.DARP.Constants.SPLASH_ILS_IMPROVEMENTS).ToString();
            string totalIterations = SplashGlobalData.GetSplashData<string>(API.DARP.Constants.SPLASH_TOTAL_ITERATIONS).ToString();
            string bestSolution = SplashGlobalData.GetSplashData<string>(API.DARP.Constants.SPLASH_BEST_SOLUTION).ToString();

            // Actualizar propiedades
            ProgressValue = dataProgress;
            NumberProblem = problem;
            NumberRepetition = repetition;
            ILSImprovements = improvements;
            TotalIterations = totalIterations;
            BestSolution = bestSolution;
        }

        public void UpdateProgress(int progress)
        {
            SplashGlobalData.SetSplashData(API.DARP.Constants.SPLASH_PROGRESS, progress);
        }
        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged
    }
}
