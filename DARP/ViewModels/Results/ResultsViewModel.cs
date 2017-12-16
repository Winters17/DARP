using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.DARP.Data.Model;
using System.Data;
using API.DARP.Data.Model.Summary;

namespace DARP.ViewModels.Results
{
    public class ResultsViewModel : INotifyPropertyChanged
    {
        public Solution Solution { get; set; }

        #region Bindings
        #region TableRoutesInput 

        private DataView tableRoutesInput;

        public DataView TableRoutesInput
        {
            get
            {
                return tableRoutesInput;
            }

            set
            {
                if (value != tableRoutesInput)
                {
                    tableRoutesInput = value;
                    // OnTableRoutesChanged();
                    NotifyPropertyChanged(nameof(TableRoutesInput));
                }
            }
        }
        #endregion

        #region TableRoutesModel 

        private DataView tableRoutesModel;

        public DataView TableRoutesModel
        {
            get
            {
                return tableRoutesModel;
            }

            set
            {
                if (value != tableRoutesModel)
                {
                    tableRoutesModel = value;
                    // OnTableRoutesModelChanged();
                    NotifyPropertyChanged(nameof(TableRoutesModel));
                }
            }
        }

        #endregion

        #region SolutionCost 

        private double solutionCost;

        public double SolutionCost
        {
            get
            {
                return solutionCost;
            }

            set
            {
                if (value != solutionCost)
                {
                    solutionCost = value;
                    // OnSolutionCostChanged();
                    NotifyPropertyChanged(nameof(SolutionCost));
                }
            }
        }

        #endregion

        #region TotalDuration 

        private double totalDuration;

        public double TotalDuration
        {
            get
            {
                return totalDuration;
            }

            set
            {
                if (value != totalDuration)
                {
                    totalDuration = value;
                    // OnTotalDurationChanged();
                    NotifyPropertyChanged(nameof(TotalDuration));
                }
            }
        }

        #endregion

        #region TotalWaitingTime 

        private double totalWaitingTime;

        public double TotalWaitingTime
        {
            get
            {
                return totalWaitingTime;
            }

            set
            {
                if (value != totalWaitingTime)
                {
                    totalWaitingTime = value;
                    // OnTotalWaitingTimeChanged();
                    NotifyPropertyChanged(nameof(TotalWaitingTime));
                }
            }
        }

        #endregion

        #region TTravelTime 

        private double tTravelTime;

        public double TTravelTime
        {
            get
            {
                return tTravelTime;
            }

            set
            {
                if (value != tTravelTime)
                {
                    tTravelTime = value;
                    // OnTTravelTimeChanged();
                    NotifyPropertyChanged(nameof(TTravelTime));
                }
            }
        }

        #endregion

        #region VDuration 

        private double vDuration;

        public double VDuration
        {
            get
            {
                return vDuration;
            }

            set
            {
                if (value != vDuration)
                {
                    vDuration = value;
                    // OnVDurationChanged();
                    NotifyPropertyChanged(nameof(VDuration));
                }
            }
        }

        #endregion

        #region VTimeWindow 

        private double vTimeWindow;

        public double VTimeWindow
        {
            get
            {
                return vTimeWindow;
            }

            set
            {
                if (value != vTimeWindow)
                {
                    vTimeWindow = value;
                    // OnVTimeWindowChanged();
                    NotifyPropertyChanged(nameof(VTimeWindow));
                }
            }
        }

        #endregion

        #region VRideTime 

        private double vRideTime;

        public double VRideTime
        {
            get
            {
                return vRideTime;
            }

            set
            {
                if (value != vRideTime)
                {
                    vRideTime = value;
                    // OnVRideTimeChanged();
                    NotifyPropertyChanged(nameof(VRideTime));
                }
            }
        }

        #endregion

        #region VLoad 
        private int vLoad;

        public int VLoad
        {
            get
            {
                return vLoad;
            }

            set
            {
                if (value != vLoad)
                {
                    vLoad = value;
                    // OnVLoadChanged();
                    NotifyPropertyChanged(nameof(VLoad));
                }
            }
        }

        #endregion


        #region ModelSolutionCost 

        private double modelSolutionCost;

        public double ModelSolutionCost
        {
            get
            {
                return modelSolutionCost;
            }

            set
            {
                if (value != modelSolutionCost)
                {
                    modelSolutionCost = value;
                    // OnSolutionCostChanged();
                    NotifyPropertyChanged(nameof(ModelSolutionCost));
                }
            }
        }

        #endregion

        #region ModelTotalDuration 

        private double modelTotalDuration;

        public double ModelTotalDuration
        {
            get
            {
                return modelTotalDuration;
            }

            set
            {
                if (value != modelTotalDuration)
                {
                    modelTotalDuration = value;
                    // OnTotalDurationChanged();
                    NotifyPropertyChanged(nameof(ModelTotalDuration));
                }
            }
        }

        #endregion

        #region ModelTotalWaitingTime 

        private double modelTotalWaitingTime;

        public double ModelTotalWaitingTime
        {
            get
            {
                return modelTotalWaitingTime;
            }

            set
            {
                if (value != modelTotalWaitingTime)
                {
                    modelTotalWaitingTime = value;
                    // OnTotalWaitingTimeChanged();
                    NotifyPropertyChanged(nameof(ModelTotalWaitingTime));
                }
            }
        }

        #endregion

        #region ModelTTravelTime 

        private double modelTTravelTime;

        public double ModelTTravelTime
        {
            get
            {
                return modelTTravelTime;
            }

            set
            {
                if (value != modelTTravelTime)
                {
                    modelTTravelTime = value;
                    // OnTTravelTimeChanged();
                    NotifyPropertyChanged(nameof(ModelTTravelTime));
                }
            }
        }

        #endregion

        #region ModelVDuration 

        private double modelVDuration;

        public double ModelVDuration
        {
            get
            {
                return modelVDuration;
            }

            set
            {
                if (value != modelVDuration)
                {
                    modelVDuration = value;
                    // OnVDurationChanged();
                    NotifyPropertyChanged(nameof(ModelVDuration));
                }
            }
        }

        #endregion

        #region ModelVTimeWindow 

        private double modelVTimeWindow;

        public double ModelVTimeWindow
        {
            get
            {
                return modelVTimeWindow;
            }

            set
            {
                if (value != modelVTimeWindow)
                {
                    modelVTimeWindow = value;
                    // OnVTimeWindowChanged();
                    NotifyPropertyChanged(nameof(ModelVTimeWindow));
                }
            }
        }

        #endregion

        #region ModelVRideTime 

        private double modelvRideTime;

        public double ModelVRideTime
        {
            get
            {
                return modelvRideTime;
            }

            set
            {
                if (value != modelvRideTime)
                {
                    modelvRideTime = value;
                    // OnVRideTimeChanged();
                    NotifyPropertyChanged(nameof(ModelVRideTime));
                }
            }
        }

        #endregion

        #region ModelVLoad 
        private int modelVLoad;

        public int ModelVLoad
        {
            get
            {
                return modelVLoad;
            }

            set
            {
                if (value != modelVLoad)
                {
                    modelVLoad = value;
                    // OnVLoadChanged();
                    NotifyPropertyChanged(nameof(ModelVLoad));
                }
            }
        }

        #endregion

        #region ExecutionTime 

        private string executionTime;

        public string ExecutionTime
        {
            get
            {
                return executionTime;
            }

            set
            {
                if (value != executionTime)
                {
                    executionTime = value;
                    // OnExecutionTimeChanged();
                    NotifyPropertyChanged(nameof(ExecutionTime));
                }
            }
        }

        #endregion







        #endregion



        public ResultsViewModel(Solution solution)
        {
            this.Solution = solution;
            InicializateTableRoutes();
            //Initial Solution
            SolutionCost = Math.Round(Solution.InitialSolution.Fitness,2);
            TotalDuration = Math.Round(Solution.InitialSolution.TotalDuration, 2);  
            TotalWaitingTime = Math.Round(Solution.InitialSolution.TotalWaitingTime, 2);
            TTravelTime = Math.Round(Solution.InitialSolution.TotalTransitTime, 2);
            VDuration = Math.Round(Solution.InitialSolution.DurationViolation, 2);
            VTimeWindow = Math.Round(Solution.InitialSolution.TimeWindowViolation, 2);
            VRideTime = Math.Round(Solution.InitialSolution.RideTimeViolation, 2);
            VLoad = Solution.InitialSolution.LoadViolation;


            //Model Solution
            ModelSolutionCost = Math.Round(Solution.Fitness, 2);
            ModelTotalDuration = Math.Round(Solution.TotalDuration, 2);
            ModelTotalWaitingTime = Math.Round(Solution.TotalWaitingTime, 2);
            ModelTTravelTime = Math.Round(Solution.TotalTransitTime, 2);
            ModelVDuration = Math.Round(Solution.DurationViolation, 2);
            ModelVTimeWindow = Math.Round(Solution.TimeWindowViolation, 2);
            ModelVRideTime = Math.Round(Solution.RideTimeViolation, 2);
            ModelVLoad = Solution.LoadViolation;
            TimeSpan ts = TimeSpan.FromMilliseconds(solution.ExecutionTime);
            ExecutionTime = ts.ToString(@"hh\:mm\:ss");
        }

        #region Methods
        private void InicializateTableRoutes()
        {
            TableRoutesModel = new DataView();
            TableRoutesInput = new DataView();

            DataTable table = new DataTable();
            var columns = Constants.ColumnTableRoutesNames();
            foreach (var entry in columns)
            {
                table.Columns.Add(entry);                
            }

            int veh_index = 1;
            foreach(var vehicle in Solution.Vehicles)
            {
                int index = 0;
                DataRow row = table.NewRow();
                row[index++] = veh_index++;                
                var route = String.Empty;
                for(int i=0; i< vehicle.Requests.Count-1; i++)
                {                    
                    route += vehicle.Requests[i].PairID + "-";
                }
                route += vehicle.Requests[vehicle.Requests.Count - 1].PairID;
                row[index++] = route;
                row[index++] = vehicle.VehicleDuration;
                table.Rows.Add(row);
            }

            TableRoutesModel = table.DefaultView;

            //Input Table
            table = new DataTable();
            columns = Constants.ColumnTableRoutesNames();
            foreach (var entry in columns)
            {
                table.Columns.Add(entry);
            }

            veh_index = 1;
            foreach (var vehicle in Solution.InitialSolution.Vehicles)
            {
                int index = 0;
                DataRow row = table.NewRow();
                row[index++] = veh_index++;
                var route = String.Empty;
                for (int i = 0; i < vehicle.Requests.Count - 1; i++)
                {
                    route += vehicle.Requests[i].PairID + "-";
                }
                route += vehicle.Requests[vehicle.Requests.Count - 1].PairID;
                row[index++] = route;
                row[index++] = vehicle.VehicleDuration;
                table.Rows.Add(row);
            }

            TableRoutesInput = table.DefaultView;
        }
        #endregion



        #region INotifyPropertyChanged implementation

        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
