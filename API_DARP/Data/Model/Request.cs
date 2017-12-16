using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DARP.Data.Model
{
    [Serializable]
    public class Request
    {
        public enum TypeOfRequest
        {
            DEPOT,
            INCOME,
            OUTCOME
        }

        public int ID_Unique { get; private set; }

        public int PairID { get; private set; }

        public TypeOfRequest RequestType { get; private set; }

        public bool Origin { get; set; }


        public bool IsCritical { get; set; } 

        public RequestCoordinates Coordinates { get; private set; }

        public RequestTimeWindow TimeWindow { get; private set; }

        public int RequiredLoad { get; set; }

        public int ServiceTime { get; private set; }

        public double TimeWindowViolation { get; set; }

        public double RideTimeViolation { get; set; }

        /// <summary>
        /// Arrival - A 
        /// </summary>
        public double Arrival { get; set; }

        /// <summary>
        /// Start Service - B
        /// </summary>
        public double StartService { get; set; }


        /// <summary>
        /// Waiting Time - W
        /// </summary>
        public double WaitingTime { get; set; }

        /// <summary>
        /// Departure Time - D
        /// </summary>
        public double DepartureTime { get; set; }


        /// <summary>
        /// Slack Time - F
        /// </summary>
        public double SlackTime { get; set; }


        /// <summary>
        /// Ride Time - L
        /// </summary>
        public double RideTime { get; set; }


        public Request(int id, TypeOfRequest reqType, double x, double y, int lb, int ub, int servTime, int reqLoad, bool critical, bool origin, int pairID)
        {
            this.ID_Unique = id;
            this.PairID = pairID;
            this.RequestType = reqType;
            Coordinates = new RequestCoordinates(x, y);
            TimeWindow = new RequestTimeWindow(lb, ub);
            this.ServiceTime = servTime;
            this.RequiredLoad = reqLoad;
            this.Arrival = 0;
            this.DepartureTime = 0;
            this.StartService = 0;
            this.WaitingTime = 0;
            this.IsCritical = critical;
            this.Origin = origin;       

        }

    }
}
