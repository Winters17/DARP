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
    public class VNSOperatorsViewModel : INotifyPropertyChanged
    {

        #region Bindings

     
        #region VNSOperatorsData 

        private DataView vnsOperatorsData;

        public DataView VNSOperatorsData
        {
            get
            {
                return vnsOperatorsData;
            }

            set
            {
                if (value != vnsOperatorsData)
                {
                    vnsOperatorsData = value;
                    // OnVNSOperatorsDataChanged();
                    NotifyPropertyChanged(nameof(VNSOperatorsData));
                }
            }
        }

        #endregion

        #endregion


        public VNSOperatorsViewModel(Solution solution)
        {
            var SummaryDetails = solution.SummaryDetails;
            //Tabla de evolución d elos operadores VNS.
            DataTable table = new DataTable();
            var columns = Constants.ColumnsVNSOperators();
            foreach (var entry in columns)
            {
                table.Columns.Add(entry);
            }


            foreach (var iteration in SummaryDetails.VNSOperators)
            {
                int index = 0;
                DataRow row = table.NewRow();
                row[index++] = iteration.Iteration;
                row[index++] = iteration.Shift1_0_Inter;
                row[index++] = iteration.Swap1_1_Inter;
                row[index++] = iteration.IntraRouteInsertion;
                row[index++] = iteration.WRI_Intra;
                row[index++] = iteration.Swap1_1_Intra;
                row[index++] = iteration.TotalChanges;
                table.Rows.Add(row);
            }

            VNSOperatorsData = table.DefaultView;
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
