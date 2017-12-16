using API.DARP.Calculations.Metaheuristics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DARP.Data.Model
{
    public class Problem
    {

        public string ID_Problem { get; set; }
        public int NumVehicles { get; set; }

        public int TotalCustomers { get; private set; }

        private int numRequests;
        public int NumRequests { get { return numRequests; } set { numRequests = value; TotalCustomers = value / 2; } }

        public double MaxVehDistance { get; set; }

        public int MaxLoad { get; set; }

        public double MaxRideTime { get; set; }

        public double ServiceTime { get; set; }

        public double[][] Distances { get; private set; }

        public bool[][] FeasibleTravel { get; set; }

        public bool[][] NeighborhoodTW { get; set; }

        public bool[][] NeighborhoodDistances { get; set; }

        public double BestKnownSolution { get; set; }

        public List<Request> Requests { get; set; } = new List<Request>();

        public Dictionary<int, List<int>> CloseTWMatrix { get; set; }

        public Dictionary<int, List<int>> CloseDistMatrix { get; set; }


        //Representa un array de tiempo medio de ventana para cada petición (de 1 a N).
        public Dictionary<int, double> AvgTimeWindow { get; set; }



        internal void CalculateDistanceTimeWindow()
        {
            CloseDistMatrix = new Dictionary<int, List<int>>();
            List<KeyValuePair<int, double>> listOfPairs = new List<KeyValuePair<int, double>>(); // List of pairs (city, distance)
            for (int i = 0; i < Requests.Count; i++)
            {
                listOfPairs.Clear();
                for (int j = 0; j < Requests.Count; j++)
                {
                    if (i != j)
                    {
                        var id = Requests[j].ID_Unique;
                        listOfPairs.Add(new KeyValuePair<int, double>(id, Distances[i][id]));
                    }
                }
                //Sort the list
                listOfPairs.Sort(Compare);
                //Insertamos los datos ordenados en la matriz
                List<int> sorted = new List<int>();
                for (int j = 0; j < Requests.Count - 2; j++)
                    sorted.Add(listOfPairs[j].Key);

                CloseDistMatrix[Requests[i].ID_Unique] = sorted;// listOfPairs.Find(t=>t.Key.Equals(Requests[j].ID_Request)).Key;

            }
            CalculateDistanceNeighborhood((int)Math.Ceiling(this.NumRequests * 0.95));
        }

        internal void CalculateAvgTimeWindow(bool complete = false)
        {
            Preprocessing();
            AvgTimeWindow = new Dictionary<int, double>();
            AvgTimeWindow.Add(0, 0); //Depot.
            if (!complete)
            {
                for (int i = 1; i < Requests.Count; i++)
                {

                    AvgTimeWindow.Add(Requests[i].ID_Unique, (Requests[i].TimeWindow.UpperBound + Requests[i].TimeWindow.LowerBound) / 2);

                }
                CalculateDistanceTimeWindow();
                CalculateCloseTimeWindow();

            }
            else
            {
                for (int i = 1; i < Requests.Count; i++)
                {
                    AvgTimeWindow.Add(Requests[i].ID_Unique, (Requests[i].TimeWindow.UpperBound + Requests[i].TimeWindow.LowerBound) / 2);
                }
            }
        }

        internal void CalculateNeighborhood(int neighborhoodSize)
        {
            NeighborhoodTW = new bool[this.NumRequests + 1][]; //Num requests.
            NeighborhoodDistances = new bool[this.NumRequests + 1][]; //Num requests.
            var max = neighborhoodSize;
            for (int i = 0; i < NeighborhoodTW.Length; i++)
            {
                NeighborhoodTW[i] = Enumerable.Repeat(false, this.NumRequests + 1).ToArray();
                NeighborhoodDistances[i] = Enumerable.Repeat(false, this.NumRequests + 1).ToArray();
                if (i == 0) continue; //Don't fill the ZERO row.
                if (neighborhoodSize > this.CloseTWMatrix[i].Count)
                    max = this.CloseTWMatrix[i].Count;
                for (int j = 0; j < max; j++)
                {
                    NeighborhoodTW[i][this.CloseTWMatrix[i][j]] = true;
                    NeighborhoodDistances[i][this.CloseDistMatrix[i][j]] = true;
                }
                max = neighborhoodSize;
            }
            CalculateFeasibleArcs(0);
        }

        internal void CalculateDistanceNeighborhood(int neighborhoodSize)
        {
            NeighborhoodDistances = new bool[this.NumRequests + 1][]; //Num requests.
            for (int i = 0; i < NeighborhoodDistances.Length; i++)
            {
                NeighborhoodDistances[i] = Enumerable.Repeat(false, this.NumRequests + 1).ToArray();
                //if (i == 0) continue; //Don't fill the ZERO row.
                for (int j = 0; j < neighborhoodSize; j++)
                {
                    NeighborhoodDistances[i][this.CloseDistMatrix[i][j]] = true;
                }
            }
        }

        internal void CalculateCloseTimeWindow()
        {
            CloseTWMatrix = new Dictionary<int, List<int>>();
            List<KeyValuePair<int, double>> listOfPairs = new List<KeyValuePair<int, double>>(); // List of pairs (city, distance)
            for (int i = 1; i < Requests.Count; i++)
            {
                listOfPairs.Clear();
                for (int j = 1; j < Requests.Count; j++)
                {
                    if (i != j && ((Requests[i].TimeWindow.LowerBound + this.Distances[Requests[i].ID_Unique][Requests[j].ID_Unique]) < Requests[j].TimeWindow.UpperBound))
                    {
                        var id = Requests[j].ID_Unique;
                        listOfPairs.Add(new KeyValuePair<int, double>(id, EuclideanDistance(Requests[i].TimeWindow.LowerBound, Requests[i].TimeWindow.UpperBound, Requests[j].TimeWindow.LowerBound, Requests[j].TimeWindow.UpperBound)));
                    }
                }
                //Sort the list
                listOfPairs.Sort(Compare);
                //Insertamos los datos ordenados en la matriz
                List<int> sorted = new List<int>();
                for (int j = 0; j < listOfPairs.Count; j++)
                    sorted.Add(listOfPairs[j].Key);

                CloseTWMatrix[Requests[i].ID_Unique] = sorted;// listOfPairs.Find(t=>t.Key.Equals(Requests[j].ID_Request)).Key;

            }
            CalculateNeighborhood((int)Math.Ceiling(this.NumRequests * 0.95));
        }


        public static double EuclideanDistance(double lowerA, double upperA, double lowerB, double upperB)
        {
            return Distance(lowerA, lowerB, upperA, upperB);
        }



        /// <summary>
        /// Return index that indicate the less distance.
        /// </summary>
        /// <param name="a">Pair a (city-distance)</param>
        /// <param name="b">Pair b (city-distance)</param>
        private static int Compare(KeyValuePair<int, double> a, KeyValuePair<int, double> b)
        {
            return a.Value.CompareTo(b.Value);
        }


        /// <summary>
        /// Establece la matriz de distancias entre puntos.
        /// </summary>
        public void CalculateDistanceMatrix()
        {
            Distances = new double[Requests.Count][];
            FeasibleTravel = new bool[Requests.Count][];
            for (int i = 0; i < Requests.Count; i++)
            {
                Distances[i] = new double[Requests.Count];
                FeasibleTravel[i] = new bool[Requests.Count];
                for (int j = 0; j < Requests.Count; j++)
                {
                    if (i == 0 && j == 0)
                        Distances[0][0] = Double.PositiveInfinity;
                    else
                        Distances[i][j] = Distance(Requests[i].Coordinates.AxisX, Requests[j].Coordinates.AxisX, Requests[i].Coordinates.AxisY, Requests[j].Coordinates.AxisY);
                }
            }

        }

        /// <summary>
        /// Inicializa la matriz de arcos factibles eliminando posibles trayectos que no son válidos (en base al time window).
        /// </summary>
        public void CalculateFeasibleArcs(double maxDist)
        {
            FeasibleTravel = new bool[Requests.Count][];
            int totalNotFeasible = 0, totalFeasible = 0;

            //int max = (int)Math.Ceiling(Math.Sqrt(this.NumRequests));
            int max = (int)Math.Ceiling(this.NumRequests * 0.10);
            for (int i = 0; i < Requests.Count; i++)
            {
                FeasibleTravel[i] = new bool[Requests.Count];
                int neighbours = 0;
                for (int j = 0; j < Requests.Count; j++)
                {
                    if (i == 0 && j <= this.TotalCustomers)
                        FeasibleTravel[i][j] = true;
                    else if (i == 0)
                    {
                        if (FeasibleTravel[i][j - this.TotalCustomers])
                        {

                            FeasibleTravel[i][j - this.TotalCustomers] = true;

                        }
                        else
                            FeasibleTravel[i][j] = false;

                    }
                    else if (j == 0 && i > this.TotalCustomers)
                        FeasibleTravel[i][j] = true;
                    else if (j == 0)
                    {
                        FeasibleTravel[i][j] = false;
                    }
                    else
                    {
                        //if (j == 0)
                        //    FeasibleTravel[i][j] = true;
                        //else
                        //{
                        //if (Requests[i].TimeWindow.LowerBound < Requests[j].TimeWindow.UpperBound && Requests[j].TimeWindow.LowerBound < (Requests[i].TimeWindow.UpperBound+(maxDist*2)))
                        //if (Requests[i].TimeWindow.LowerBound < Requests[j].TimeWindow.UpperBound && (AvgTimeWindow[j] < AvgTimeWindow[i] + 100))                            
                        //if (Requests[i].TimeWindow.LowerBound < Requests[j].TimeWindow.UpperBound && this.NeighborhoodTW[Requests[i].ID_Unique][Requests[j].ID_Unique])
                        //if (((Requests[i].TimeWindow.LowerBound + this.Distances[Requests[i].ID_Unique][Requests[j].ID_Unique]) < Requests[j].TimeWindow.UpperBound) && this.NeighborhoodTW[Requests[i].ID_Unique][Requests[j].ID_Unique])
                        if (((Requests[i].TimeWindow.LowerBound + this.Distances[Requests[i].ID_Unique][Requests[j].ID_Unique]) < Requests[j].TimeWindow.UpperBound) && (this.NeighborhoodTW[Requests[i].ID_Unique][Requests[j].ID_Unique] || this.NeighborhoodDistances[Requests[i].ID_Unique][Requests[j].ID_Unique]))
                        {
                            totalFeasible++;
                            FeasibleTravel[i][j] = true;
                        }
                        else
                        {
                            FeasibleTravel[i][j] = false;
                            totalNotFeasible++;
                        }
                        // }
                    }
                }
            }



        }

        private static double Distance(double X, double X1, double Y, double Y1)
        {
            double xd = X - X1;
            double yd = Y - Y1;
            return Math.Round(Math.Sqrt(xd * xd + yd * yd), 2);
        }

        private void Preprocessing()
        {
            int totalCustomers = TotalCustomers;
            List<Request> requests = Requests;
            //Preprocessing inbound (1-totalCustomers/2)
            for (int i = 1; i <= totalCustomers / 2; i++)
            {
                requests[i].TimeWindow.LowerBound = Math.Max(0, requests[i + totalCustomers].TimeWindow.LowerBound - MaxRideTime - ServiceTime);
                requests[i].TimeWindow.UpperBound = Math.Min(requests[i].TimeWindow.UpperBound, requests[i + totalCustomers].TimeWindow.UpperBound - Distances[i][i + totalCustomers] - ServiceTime);
            }

            //Preprocessing outbound
            for (int i = (totalCustomers / 2) * 3 + 1; i < requests.Count; i++)
            {
                requests[i].TimeWindow.LowerBound = Math.Max(0, requests[i - totalCustomers].TimeWindow.LowerBound + Distances[i - totalCustomers][i] + ServiceTime);
                requests[i].TimeWindow.UpperBound = Math.Min(requests[i].TimeWindow.UpperBound, requests[i - totalCustomers].TimeWindow.UpperBound + MaxRideTime + ServiceTime);

            }
        }

    }
}
