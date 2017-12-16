using API.DARP.Data.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DARP.Data.Model
{
    public class InputDataLoaded
    {
        public DataTable TableProblems { get; set; }

        public List<Problem> Problems { get; set; }

        public InputDataLoaded()
        {
            TableProblems = new DataTable();
            TableProblems.TableName = Constants.TABLE_INPUT;
            var columns = Constants.ColumnsInputProblems();
            foreach(var c in columns)
            {
                TableProblems.Columns.Add(c);
            }

            Problems = new List<Problem>();
        }
    }
}
