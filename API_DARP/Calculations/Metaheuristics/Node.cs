using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DARP.Calculations.Metaheuristics
{

    [Serializable]
    public class Node
    {

        public int ID { get; set; }


        /// <summary>
        /// Indicate wheter current node is or not a origin in the sequence.
        /// </summary>
        public bool Origin { get; set; }


        public bool Critical { get; set; }



        /// <summary>
        /// Arrival - A 
        /// </summary>
        public double Arrival { get; set; } = 0;

        /// <summary>
        /// Start Service - B
        /// </summary>
        public double StartService { get; set; } = 0;


        /// <summary>
        /// Waiting Time - W
        /// </summary>
        public double WaitingTime { get; set; } = 0;

        /// <summary>
        /// Departure Time - D
        /// </summary>
        public double DepartureTime { get; set; } = 0;


        /// <summary>
        /// Slack Time - F
        /// </summary>
        public double SlackTime { get; set; } = 0;


        /// <summary>
        /// Ride Time - L
        /// </summary>
        public double RideTime { get; set; } = 0;


        public Node(int id,  bool isOrigin, bool critical)
        {
            Origin = isOrigin;
            ID = id;
            Critical = critical;
        }

    }
}
