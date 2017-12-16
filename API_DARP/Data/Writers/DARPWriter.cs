using API.DARP.Data.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DARP.Data.Writers
{
    public class DARPWriter
    {

        public void WriteCSV(Solution solution, string path)
        {            
            using (StreamWriter sw = new StreamWriter(path,false))
            {
                sw.WriteLine("sep=,");
                int num_V = 1;
                foreach(var vehicle in solution.Vehicles)
                {
                    sw.Write("Vehicle N#: " + num_V);
                    sw.Write(sw.NewLine);
                    sw.Write("ID,Arrival,Start Service, Departure, Slack Time, Waiting Time, Time Window Violation, Ride Time, Load");
                    sw.Write(sw.NewLine);
                    sw.Write(vehicle.Requests[0].ID_Unique + "," + Math.Round(vehicle.Requests[0].Arrival, 2) + "," + Math.Round(vehicle.Requests[0].StartService, 2) + "," + Math.Round(vehicle.Requests[0].DepartureTime, 2) + "," + Math.Round(vehicle.Requests[0].SlackTime, 2) + "," + Math.Round(vehicle.Requests[0].WaitingTime, 2) + "," + Math.Round(0.0, 2) + "," + Math.Round(0.0, 2) + "," + 0);
                    sw.Write(sw.NewLine);
                    for (int i=1; i< vehicle.Requests.Count-1; i++)
                    {
                        sw.Write(vehicle.Requests[i].ID_Unique+ ","+ Math.Round(vehicle.Requests[i].Arrival,2) + "," + Math.Round(vehicle.Requests[i].StartService,2) + "," + Math.Round(vehicle.Requests[i].DepartureTime,2) + "," + Math.Round(vehicle.Requests[i].SlackTime,2) + "," + Math.Round(vehicle.Requests[i].WaitingTime,2) + "," + Math.Round(vehicle.Requests[i].TimeWindowViolation,2) + "," + Math.Round(vehicle.Requests[i].RideTime,2) + "," + vehicle.Requests[i].RequiredLoad);
                        sw.Write(sw.NewLine);
                    }
                    sw.Write(vehicle.Requests[vehicle.Requests.Count-1].ID_Unique + "," + Math.Round(vehicle.Requests[vehicle.Requests.Count - 1].Arrival, 2) + "," + Math.Round(vehicle.Requests[vehicle.Requests.Count - 1].StartService, 2) + "," + Math.Round(vehicle.Requests[vehicle.Requests.Count - 1].DepartureTime, 2) + "," + Math.Round(vehicle.Requests[vehicle.Requests.Count - 1].SlackTime, 2) + "," + Math.Round(vehicle.Requests[0].WaitingTime, 2) + "," + Math.Round(0.0, 2) + "," + Math.Round(0.0, 2) + "," + 0);
                    sw.Write(sw.NewLine);
                    num_V++;
                    Console.Write("\n");
                }
                sw.Write(sw.NewLine);
                sw.Write("--------- SOLUTION DETAILS ---------");
                sw.Write(sw.NewLine);
                sw.Write("Problem,Date,Cost, Feasible, Total Duration, Total Waiting Time , Total Transit Time, Duration Violation, Time Window Violation, Ride Time Violation, Load Violation, Execution Time");
                sw.Write(sw.NewLine);
                sw.Write(solution.ID_DARP + "," + DateTime.Now.ToShortDateString() + "," + solution.Fitness + "," + solution.Feasible + "," + solution.TotalDuration + "," + solution.TotalWaitingTime + "," + solution.TotalTransitTime + "," + solution.DurationViolation + "," + solution.TimeWindowViolation + "," + solution.RideTimeViolation + "," + solution.LoadViolation + "," + solution.ExecutionTime);

            }
        }
    }
}
