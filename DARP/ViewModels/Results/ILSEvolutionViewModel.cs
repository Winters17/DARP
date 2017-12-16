using API.DARP.Data.Model;
using API.DARP.Data.Model.Summary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARP.ViewModels.Results
{
    public class ILSEvolutionViewModel : INotifyPropertyChanged
    {

        #region Binding
        #region SummaryDetails 

        private SummaryDetails summaryDetails;

        public SummaryDetails SummaryDetails
        {
            get
            {
                return summaryDetails;
            }

            set
            {
                if (value != summaryDetails)
                {
                    summaryDetails = value;
                    // OnSummaryDetailsChanged();
                    NotifyPropertyChanged(nameof(SummaryDetails));
                }
            }
        }

        #endregion

        #region TableSelected 

        private DataView tableSelected;

        public DataView TableSelected
        {
            get
            {
                return tableSelected;
            }

            set
            {
                if (value != tableSelected)
                {
                    tableSelected = value;
                    // OnTableSelectedChanged();
                    NotifyPropertyChanged(nameof(TableSelected));
                }
            }
        }

        #endregion


        #region ILSEvolution 

        private DataView resumeTable;

        public DataView ILSEvolutionData
        {
            get
            {
                return resumeTable;
            }

            set
            {
                if (value != resumeTable)
                {
                    resumeTable = value;
                    // OnResumeTableChanged();
                    NotifyPropertyChanged(nameof(ILSEvolutionData));
                }
            }
        }

        #endregion

        #region VNSOperatorsTable 

        private DataView vnsOperatorsTable;

        public DataView VNSOperatorsTable
        {
            get
            {
                return vnsOperatorsTable;
            }

            set
            {
                if (value != vnsOperatorsTable)
                {
                    vnsOperatorsTable = value;
                    // OnVNSOperatorsTableChanged();
                    NotifyPropertyChanged(nameof(VNSOperatorsTable));
                }
            }
        }

        #endregion



        #endregion


        public ILSEvolutionViewModel(Solution solution)
        {
            SummaryDetails = solution.SummaryDetails;
            ILSEvolutionData = new DataView();
            VNSOperatorsTable = new DataView();


            //Tabla de evolución de ILS.
            DataTable table = new DataTable();
            var columns = Constants.ColumnsILSEvolution();
            foreach (var entry in columns)
            {
                table.Columns.Add(entry);
            }


            foreach (var iteration in SummaryDetails.ILSEvolution)
            {
                int index = 0;
                DataRow row = table.NewRow();
                row[index++] = iteration.Iteration;
                row[index++] = iteration.InitCost;
                row[index++] = iteration.InitFitness;
                row[index++] = iteration.VNSCost;
                row[index++] = iteration.VNSFitness;
                row[index++] = iteration.Feasible;
                row[index++] = iteration.PerturbationType;
                row[index++] = iteration.PertCost;
                row[index++] = iteration.PertFitness;
                table.Rows.Add(row);
            }
            ILSEvolutionData = table.DefaultView;
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
