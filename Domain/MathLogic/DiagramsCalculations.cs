using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAnalyzer.Domain.MathLogic
{
    public class DiagramsCalculations
    {
        private double bandwidth;
        public double Bandwidth
        {
            get { return bandwidth; }
            set 
            {
                if (value > 0)
                {
                    bandwidth = value;
                    KDEValues = CalculateKDEValues(Data, bandwidth);
                }
            }
        }


        private List<double> data;
        public List<double> Data
        {
            get { return data; }
            set
            {
                data = value;
                data.Sort();

                Bandwidth = CalculateDefaultBandwidth(data);
            }
        }


        public List<double> KDEValues { get; private set; } 


        public DiagramsCalculations(List<double> inputData)
        {
            data = inputData;
            data.Sort();

            KDEValues = new();
            double bw = CalculateDefaultBandwidth(data);
            Bandwidth = bw;
        }


        public static double CalculateDefaultBandwidth(List<double> data)
        {
            int N = data.Count;
            double S = Math.Sqrt(data.Sum(x => Math.Pow(x - data.Average(), 2)) / (N - 1));
            return S * Math.Pow(0.75 * N, -1.0 / 5.0);
        }

        public static double CalculateKernel(double u)
        {
            return Math.Abs(u) <= Math.Sqrt(5.0) ?
                (3.0 / (4.0 * Math.Sqrt(5.0)) * (1 - u * u / 5)) : 0;
        }

        public static List<double> CalculateKDEValues(List<double> data, double bandwidth)
        {
            List<double> values = new();
            int N = data.Count;


            foreach (var x in data)
            {
                double sumKernel = 0.0;

                foreach (var xi in data)
                {
                    sumKernel += CalculateKernel((x - xi) / bandwidth);
                }

                values.Add(sumKernel / N / bandwidth);
            }

            return values;
        }
    }
}
