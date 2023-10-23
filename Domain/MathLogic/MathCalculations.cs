using DataAnalyzer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAnalyzer.Domain.MathLogic
{
    public class MathCalculations
    {
        private double normalQuantile;
        private double studentQuantile;


        private double confidenceLevel;
        public double ConfidenceLevel
        {
            get { return confidenceLevel; }
            set
            {
                if (value >= 0.5)
                {
                    confidenceLevel = value;

                    normalQuantile = CalculateNormalQuantile(confidenceLevel);
                    studentQuantile = CalculateStudentQuantile(normalQuantile, data.Count - 1);

                    Characteristics = FillCharacteristics();
                }
            }
        }


        private List<double> data;
        public List<double> Data
        {
            get { return data; }
            set
            {
                data = new(value);
                data.Sort();

                normalQuantile = CalculateNormalQuantile(ConfidenceLevel);
                studentQuantile = CalculateStudentQuantile(ConfidenceLevel, data.Count - 1);

                Characteristics = FillCharacteristics();
            }
        }


        public List<Characteristic> Characteristics { get; private set; }


        public MathCalculations(List<double> inputData, double intervalConfidenceLevel = 0.975)
        {
            data = new(inputData);
            data.Sort();

            Characteristics = new();
            ConfidenceLevel = intervalConfidenceLevel;
        }


        public static double CalculateNormalQuantile(double confidenceLevel = 0.975)
        {
            double a = 1 - confidenceLevel;

            double t = Math.Sqrt(-2 * Math.Log(a));
            double c0 = 2.515517;
            double c1 = 0.802853;
            double c2 = 0.010328;
            double d1 = 1.432788;
            double d2 = 0.1892659;
            double d3 = 0.001308;

            double u = t - (c0 + c1 * t + c2 * t * t) /
                (1 + d1 * t + d2 * t * t + d3 * t * t * t);

            return u;
        }

        public static double CalculateStudentQuantile(double normalQuantile = 0.975, int freedomLevel = 5)
        {
            double v = Convert.ToDouble(freedomLevel);
            double u = normalQuantile;

            double g1 = 1.0 / 4.0 * (Math.Pow(u, 3) + u);
            double g2 = 1.0 / 96.0 * (5 * Math.Pow(u, 5) + 16 * Math.Pow(u, 3) + 3 * u);
            double g3 = 1.0 / 384.0 * (3 * Math.Pow(u, 7) +
                19 * Math.Pow(u, 5) + 17 * Math.Pow(u, 3) - 15 * u);
            double g4 = 1.0 / 92160.0 * (79 * Math.Pow(u, 9) + 779 * Math.Pow(u, 7) +
                1482 * Math.Pow(u, 5) - 1920 * Math.Pow(u, 3) - 945 * u);

            double t = u + 1 / v * g1 + 1 / Math.Pow(v, 2) * g2 +
                1 / Math.Pow(v, 3) * g3 + 1 / Math.Pow(v, 4) * g4;

            return t;
        }

        public static double CalculateAverage(List<double> data)
        {
            return data.Average();
        }

        public static double CalculateStandartDeviation(List<double> data)
        {
            double av = data.Average();
            int N = data.Count;

            return Math.Sqrt(data.Sum(x => Math.Pow(x - av, 2)) / (N - 1));
        }


        private List<Characteristic> FillCharacteristics()
        {
            List<Characteristic> characteristics = new();
            int N = Data.Count;

            // Fill base characteristics
            characteristics.Add(new Characteristic()
            {
                Name = "Середнє арифметичне",
                Evaluation = data.Average(),
                EvaluationDiviation = 0,
                ConfidenceIntervalMin = 0,
                ConfidenceIntervalMax = 0,
                ConfidenceInterval = "[ ; ]"
            });

            characteristics.Add(new Characteristic()
            {
                Name = "Медіана",
                Evaluation = N % 2 == 0 ?
                    (data[N / 2 - 1] + data[N / 2]) / 2 :
                    data[N / 2],
                ConfidenceIntervalMin = 0,
                ConfidenceIntervalMax = 0,
                ConfidenceInterval = "[ ; ]"
            });

            characteristics.Add(new Characteristic()
            {
                Name = "Середньоквадратичне відхилення",
                Evaluation = Math.Sqrt(data.Sum(x =>
                    Math.Pow(x - characteristics[0].Evaluation, 2)) / (N - 1)),
                EvaluationDiviation = 0,
                ConfidenceIntervalMin = 0,
                ConfidenceIntervalMax = 0,
                ConfidenceInterval = "[ ; ]"
            });

            characteristics.Add(new Characteristic()
            {
                Name = "Коефіцієнт асиметрії",
                Evaluation = (data.Sum(x =>
                    Math.Pow(x - characteristics[0].Evaluation, 3)) /
                    (N * Math.Pow(characteristics[2].Evaluation * (N - 1) / N, 3))) *
                    Math.Sqrt(N * (N - 1)) / (N - 2),
                EvaluationDiviation = 0,
                ConfidenceIntervalMin = 0,
                ConfidenceIntervalMax = 0,
                ConfidenceInterval = "[ ; ]"
            });

            characteristics.Add(new Characteristic()
            {
                Name = "Коефіцієнт ексцесу",
                Evaluation = (data.Sum(x =>
                    Math.Pow(x - characteristics[0].Evaluation, 4)) /
                    (N * Math.Pow(characteristics[2].Evaluation * (N - 1) / N, 4)) - 3
                    + 6 / (N + 1)) * (N * N - 1) / (N - 2) / (N - 3),
                EvaluationDiviation = 0,
                ConfidenceIntervalMin = 0,
                ConfidenceIntervalMax = 0,
                ConfidenceInterval = "[ ; ]"
            });

            characteristics.Add(new Characteristic()
            {
                Name = "Мінімум",
                Evaluation = data[0],
            });

            characteristics.Add(new Characteristic()
            {
                Name = "Максимум",
                Evaluation = data[^1],
            });


            // Fill characteristics standart deviation
            characteristics[0].EvaluationDiviation =
                characteristics[2].Evaluation / Math.Sqrt(N);

            characteristics[2].EvaluationDiviation =
                characteristics[2].Evaluation / Math.Sqrt(N * 2);

            characteristics[3].EvaluationDiviation = Math.Sqrt((6.0 / (N - 2)) * ((double)N / (N + 1)) * 
                ((double)(N - 1) / (N + 3)));

            characteristics[4].EvaluationDiviation = Math.Sqrt((24.0 / (N - 3)) * ((double)N / (N  - 2)) * 
                ((double)(N - 1) / (N + 3)) * ((double)(N - 1) / (N + 5)));


            // Fill characteristics confidence interval
            characteristics[0].ConfidenceIntervalMin =
                characteristics[0].Evaluation - studentQuantile * characteristics[0].EvaluationDiviation;
            characteristics[0].ConfidenceIntervalMax =
                characteristics[0].Evaluation + studentQuantile * characteristics[0].EvaluationDiviation;

            characteristics[1].ConfidenceIntervalMin =
                Data[Convert.ToInt32(N / 2 - normalQuantile * Math.Sqrt(N) / 2)];
            characteristics[1].ConfidenceIntervalMax =
                Data[Convert.ToInt32(N / 2 + 1 + normalQuantile * Math.Sqrt(N) / 2)];

            characteristics[2].ConfidenceIntervalMin =
                characteristics[2].Evaluation - studentQuantile * characteristics[2].EvaluationDiviation;
            characteristics[2].ConfidenceIntervalMax =
                characteristics[0].Evaluation + studentQuantile * characteristics[2].EvaluationDiviation;

            characteristics[3].ConfidenceIntervalMin =
                characteristics[3].Evaluation - studentQuantile * characteristics[3].EvaluationDiviation;
            characteristics[3].ConfidenceIntervalMax =
                characteristics[3].Evaluation + studentQuantile * characteristics[3].EvaluationDiviation;

            characteristics[4].ConfidenceIntervalMin =
                characteristics[4].Evaluation - studentQuantile * characteristics[4].EvaluationDiviation;
            characteristics[4].ConfidenceIntervalMax =
                characteristics[4].Evaluation + studentQuantile * characteristics[4].EvaluationDiviation;

            for (int i = 0; i < 5; i++)
            {
                characteristics[i].ConfidenceInterval =
                    $"[{Convert.ToDouble(characteristics[i].ConfidenceIntervalMin)} ; " +
                    $"{Convert.ToDouble(characteristics[i].ConfidenceIntervalMax)}]";
            }

            return characteristics;
        }
    }
}
