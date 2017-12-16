using API.DARP.Data.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARP.ViewModels.Results
{
    public class SummaryResultsViewModel : INotifyPropertyChanged
    {


        #region Bindings

        #region ILSSummary 

        private DataView ilsSummary;

        public DataView ILSSummary
        {
            get
            {
                return ilsSummary;
            }

            set
            {
                if (value != ilsSummary)
                {
                    ilsSummary = value;
                    // OnILSSummaryChanged();
                    NotifyPropertyChanged(nameof(ILSSummary));
                }
            }
        }

        #endregion

        #region VNSSummary 

        private DataView vnsSummary;

        public DataView VNSSummary
        {
            get
            {
                return vnsSummary;
            }

            set
            {
                if (value != vnsSummary)
                {
                    vnsSummary = value;
                    // OnVNSSummaryChanged();
                    NotifyPropertyChanged(nameof(VNSSummary));
                }
            }
        }

        #endregion




        #endregion


        public SummaryResultsViewModel(Solution solution)
        {
            var SummaryDetails = solution.SummaryDetails;
            VNSSummary = new DataView();
            ILSSummary = new DataView();


            //Tabla de evolución de ILS.
            DataTable table = new DataTable();
            var columns = Constants.ColumnsILSSummary();
            foreach (var entry in columns)
            {
                table.Columns.Add(entry);
            }



            DataRow row = table.NewRow();
            row[0] = "Init Cost";
            row[1] = SummaryDetails.ILSSummary.InitCost;
            table.Rows.Add(row);
            row = table.NewRow();
            row[0] = "Final Cost";
            row[1] = SummaryDetails.ILSSummary.FinalCost;
            table.Rows.Add(row);
            row = table.NewRow();
            row[0] = "First Feasible";
            row[1] = SummaryDetails.ILSSummary.FirstItFeasible;
            table.Rows.Add(row);
            row = table.NewRow();
            row[0] = "Best Iteration";
            row[1] = SummaryDetails.ILSSummary.BestIteration;
            table.Rows.Add(row);
            row = table.NewRow();
            row[0] = "Total Imp. Best";
            row[1] = SummaryDetails.ILSSummary.TotalImpBest + " (" + Math.Round((double)SummaryDetails.ILSSummary.TotalImpBest / (double)SummaryDetails.ILSSummary.TotalIterations, 2) * 100 + "%)";
            table.Rows.Add(row);
            row = table.NewRow();
            row[0] = "Total Imp. Previous";
            row[1] = SummaryDetails.ILSSummary.TotalImpPrevious +" ("+Math.Round((double)SummaryDetails.ILSSummary.TotalImpPrevious/ (double)SummaryDetails.ILSSummary.TotalIterations,2) * 100 + "%)";
            table.Rows.Add(row);
            row = table.NewRow();
            row[0] = "Total Iterations";
            row[1] = SummaryDetails.ILSSummary.TotalIterations;
            table.Rows.Add(row);

            ILSSummary = table.DefaultView;


            //Tabla de evolución de VNS..
            table = new DataTable();
            columns = Constants.ColumnsILSSummary();
            foreach (var entry in columns)
            {
                table.Columns.Add(entry);
            }


            var total = SummaryDetails.VNSOperators.Sum(t => t.Swap1_1_Intra + t.Shift1_0_Inter + t.WRI_Intra + t.IntraRouteInsertion + t.Swap1_1_Inter);
            row = table.NewRow();
            row[0] = "Inter Shift";
            row[1] = SummaryDetails.VNSOperators.Sum(t=>t.Shift1_0_Inter) + " (" + (Math.Round((double)(SummaryDetails.VNSOperators.Sum(t => t.Shift1_0_Inter) / (double)total), 2)) * 100 + "%)";
            table.Rows.Add(row);
            row = table.NewRow();
            row[0] = "Inter Swap";
            row[1] = SummaryDetails.VNSOperators.Sum(t => t.Swap1_1_Inter) + " (" + (Math.Round((double)(SummaryDetails.VNSOperators.Sum(t => t.Swap1_1_Inter) / (double)total), 2)) * 100 + "%)";
            table.Rows.Add(row);
            row = table.NewRow();
            row[0] = "Intra Route Insertion";
            row[1] = SummaryDetails.VNSOperators.Sum(t => t.IntraRouteInsertion) + " (" + (Math.Round((double)(SummaryDetails.VNSOperators.Sum(t => t.IntraRouteInsertion) / (double)total), 2))*100 + "%)";
            table.Rows.Add(row);
            row = table.NewRow();
            row[0] = "WRI Intra";
            row[1] = SummaryDetails.VNSOperators.Sum(t => t.WRI_Intra) + " (" +(Math.Round((double)(SummaryDetails.VNSOperators.Sum(t => t.WRI_Intra) / (double)total),2)) * 100 + "%)";
            table.Rows.Add(row);
            row = table.NewRow();
            row[0] = "Intra Swap";
            row[1] = SummaryDetails.VNSOperators.Sum(t => t.Swap1_1_Intra)+ " (" + (Math.Round((double)(SummaryDetails.VNSOperators.Sum(t => t.Swap1_1_Intra) / (double)total), 2)) * 100 + "%)";
            table.Rows.Add(row);
            row = table.NewRow();
            row[0] = "Total";
            row[1] = total;
            table.Rows.Add(row);


            VNSSummary = table.DefaultView;
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
