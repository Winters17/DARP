using API.DARP.Data.Model;
using API.DARP.Data.Model.Charts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARP.ViewModels.Results
{
    public class ChartsViewModel : INotifyPropertyChanged
    {


        #region Binding

        #region VNSOperators 
        private ObservableCollection<ChartSerie<int>> vnsOperators;

        public ObservableCollection<ChartSerie<int>> VNSOperators
        {
            get
            {
                return vnsOperators;
            }

            set
            {
                if (value != vnsOperators)
                {
                    vnsOperators = value;
                    // OnVNSOperatorsChanged();
                    NotifyPropertyChanged(nameof(VNSOperators));
                }
            }
        }

        #endregion

        #region PerturbationImpact 

        private ObservableCollection<ChartSerie<int>> perturbationImpact;

        public ObservableCollection<ChartSerie<int>> PerturbationImpact
        {
            get
            {
                return perturbationImpact;
            }

            set
            {
                if (value != perturbationImpact)
                {
                    perturbationImpact = value;
                    // OnPerturbationImpactChanged();
                    NotifyPropertyChanged(nameof(PerturbationImpact));
                }
            }
        }

        #endregion

        #region CostEvolution 

        private ObservableCollection<ChartSerie<int>> costEvolution;

        public ObservableCollection<ChartSerie<int>> CostEvolution
        {
            get
            {
                return costEvolution;
            }

            set
            {
                if (value != costEvolution)
                {
                    costEvolution = value;
                    // OnCostEvolutionChanged();
                    NotifyPropertyChanged(nameof(CostEvolution));
                }
            }
        }

        #endregion

        #region FitnessEvolution 

        private ObservableCollection<ChartSerie<int>> fitnessEvolution;


        public ObservableCollection<ChartSerie<int>> FitnessEvolution
        {
            get
            {
                return fitnessEvolution;
            }

            set
            {
                if (value != fitnessEvolution)
                {
                    fitnessEvolution = value;
                    // OnFitnessEvolutionChanged();
                    NotifyPropertyChanged(nameof(FitnessEvolution));
                }
            }
        }

        #endregion





        #region MaxValueYChart1

        private double maxValueYChart1;

        public double MaxValueYChart1
        {
            get
            {
                return maxValueYChart1;
            }

            set
            {
                if (value != maxValueYChart1)
                {
                    maxValueYChart1 = value;
                    // OnMaxValueYChartVNSChanged();
                    NotifyPropertyChanged(nameof(MaxValueYChart1));
                }
            }
        }

        #endregion

        #region MinValueYChart1 

        private double minValueYChart1;

        public double MinValueYChart1
        {
            get
            {
                return minValueYChart1;
            }

            set
            {
                if (value != minValueYChart1)
                {
                    minValueYChart1 = value;
                    // OnMinValueYChart1Changed();
                    NotifyPropertyChanged(nameof(MinValueYChart1));
                }
            }
        }

        #endregion

        #region MaxValueYChart2 

        private double maxValueYChart2;

        public double MaxValueYChart2
        {
            get
            {
                return maxValueYChart2;
            }

            set
            {
                if (value != maxValueYChart2)
                {
                    maxValueYChart2 = value;
                    // OnMaxValueYChart2Changed();
                    NotifyPropertyChanged(nameof(MaxValueYChart2));
                }
            }
        }

        #endregion




        #endregion



        public ChartsViewModel(Solution solution)
        {            
            var summary = solution.SummaryDetails;

            //Chart1. Ev. impacto de la perturbación y de los operadores VNS.

            PerturbationImpact = new ObservableCollection<ChartSerie<int>>();
            for(int i=0; i< summary.ILSEvolution.Count; i++)
            {
                if (i==0) //Primera iteración.
                    PerturbationImpact.Add(new ChartSerie<int> { X = i + 1, Y = 0 });
                else
                    PerturbationImpact.Add(new ChartSerie<int> { X = i + 1, Y = ((summary.ILSEvolution[i-1].PertFitness - summary.ILSEvolution[i].VNSFitness) / summary.ILSEvolution[i-1].PertFitness) * 100 });
            }


            VNSOperators = new ObservableCollection<ChartSerie<int>>();
            for(int i=0; i< summary.VNSOperators.Count; i++)
            {
                VNSOperators.Add(new ChartSerie<int> { X = i + 1, Y = (summary.VNSOperators[i].WRI_Intra + summary.VNSOperators[i].IntraRouteInsertion+ summary.VNSOperators[i].Swap1_1_Intra+ summary.VNSOperators[i].Shift1_0_Inter+ summary.VNSOperators[i].Swap1_1_Inter) });
            }


            //Valores máximos y mínimos.
            MaxValueYChart1 = Math.Max(PerturbationImpact.Max(t => t.Y), VNSOperators.Max(t => t.Y)) + 20;
            MinValueYChart1 = (Math.Min(PerturbationImpact.Min(t => t.Y), VNSOperators.Min(t => t.Y)) - 20 < 0) ? Math.Min(PerturbationImpact.Min(t => t.Y), VNSOperators.Min(t => t.Y)) - 20 : 0;


            //Chart2. Evolución coste y fitness
            CostEvolution = new ObservableCollection<ChartSerie<int>>();
            FitnessEvolution = new ObservableCollection<ChartSerie<int>>();
            int index = 1;
            foreach(var entry in summary.ILSEvolution)
            {
                CostEvolution.Add(new ChartSerie<int> { X = index++, Y = entry.VNSCost });
                FitnessEvolution.Add(new ChartSerie<int> { X = index++, Y = entry.VNSFitness });
            }
            //Valores máximos y mínimos.
            MaxValueYChart2 = Math.Max(CostEvolution.Max(t => t.Y), FitnessEvolution.Max(r => r.Y)) + 50;


        }


        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
