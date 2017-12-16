
using API.DARP.Calculations.Algorithms;
using API.DARP.Data.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static API.DARP.Data.Model.Request;

namespace API.DARP.Data.Readers
{
    public class DARPReader
    {        


        /// <summary>
        /// Proceso lector de problemas en formato DARP
        /// </summary>
        public InputDataLoaded ReadInputProblems(string [] files)
        {
            int interval = 0;
            InputDataLoaded input = new InputDataLoaded();
            
            for (int i = 0; i < files.Count(); i++)
            {
                int index = 0;
                DataRow row = input.TableProblems.NewRow();
                Problem data = new Problem();
                //Name file.
                var name = Path.GetFileName(files[i]); 
                string problemInfo = File.ReadLines(files[i]).First();
                var headers = problemInfo.Split(' ');
                data.ID_Problem = name;
                var i_rem = name.IndexOf('.');
                data.BestKnownSolution = Constants.BestKnownProblem()[i_rem > -1 ? name.Remove(name.IndexOf('.')) : name];
                data.NumVehicles = Convert.ToInt16(headers[0]);
                data.NumRequests = Convert.ToUInt16(headers[1]);
                data.MaxVehDistance = Convert.ToDouble(headers[2]);
                data.MaxLoad = Convert.ToInt16(headers[3]);
                data.MaxRideTime = Convert.ToDouble(headers[4]);

                //Fill rows.
                row[index++] = data.ID_Problem;
                row[index++] = data.BestKnownSolution;
                row[index++] = data.NumVehicles;
                row[index++] = data.NumRequests;
                row[index++] = data.MaxVehDistance;
                row[index++] = data.MaxLoad;
                row[index++] = data.MaxRideTime;

                //Set interval.
                interval = data.NumRequests/4;

                //Set  the limit interval. 
                //Skip header, it has already been read.
                List<string> requestsLines = File.ReadLines(files[i]).Skip(1).ToList();
                int num_Request = 0;
                foreach(var line in requestsLines)
                {
                    bool critical = false;
                    var requestLine = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries); //Split by spaces.
                    TypeOfRequest type = TypeOfRequest.DEPOT;
                    var x = Convert.ToDouble(requestLine[1]); //Pos X.
                    var y = Convert.ToDouble(requestLine[2]); //Pos Y.
                    var st = Convert.ToInt16(requestLine[3]); //Service time.
                    var rl = Convert.ToInt16(requestLine[4]); //Required load.
                    var lb = Convert.ToInt16(requestLine[5]); //Upper bound.
                    var ub = Convert.ToInt16(requestLine[6]); //Lower bound.    
                                  
                    if (Convert.ToInt16(requestLine[0]) != 0 && (Convert.ToInt16(requestLine[0]) <= (interval * 1)))
                    {
                        type = TypeOfRequest.OUTCOME;
                    }
                    else if (Convert.ToInt16(requestLine[0]) != 0 && (Convert.ToInt16(requestLine[0]) <= (interval * 2)))
                    {
                        type = TypeOfRequest.INCOME;
                        critical = true;
                    }
                    else if (Convert.ToInt16(requestLine[0]) != 0 && (Convert.ToInt16(requestLine[0]) <= (interval * 3)))
                    {
                        type = TypeOfRequest.OUTCOME;
                        critical = true;
                    }
                    else if (Convert.ToInt16(requestLine[0]) != 0)
                    {
                        type = TypeOfRequest.INCOME;
                    }
                    Request request = new Request(num_Request, type, x, y, lb, ub, st, rl, critical, num_Request <= data.TotalCustomers, num_Request <= data.TotalCustomers ? num_Request : num_Request - data.TotalCustomers);
                    num_Request++;
                    data.Requests.Add(request);
                    //if (num_Request > data.TotalCustomers)
                    //    num_Request = 1;
                }



                //Service time.
                row[index++] = data.Requests.Last().ServiceTime;
                data.ServiceTime = data.Requests.Last().ServiceTime;
                data.CalculateDistanceMatrix();

                data.CalculateAvgTimeWindow();

                input.Problems.Add(data);
                input.TableProblems.Rows.Add(row);
            }

            return input;
        }

    }
}
