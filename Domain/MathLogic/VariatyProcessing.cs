using DataAnalyzer.Models;
using System;
using System.Collections.Generic;

namespace DataAnalyzer.Domain.MathLogic
{
    public class VariatyProcessing
    {
        public List<Variaty> Variaties { get; private set; }
        public List<VariatyClass> VariatyClasses { get; private set; }


        private int classCount;
        public int ClassCount
        {
            get { return classCount; }
            set
            {
                if (value > 0)
                {
                    classCount = value;
                    ClassWidth = CalculateClassWidth(Variaties, classCount);
                    VariatyClasses = ClassesDistribution(Variaties, classCount);
                }
            }
        }


        private double classWidth;
        public double ClassWidth
        {
            get { return classWidth; }
            private set
            {
                classWidth = value > 0 ? value : classWidth;
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

                Variaties = FillVariaties(data);
                ClassCount = CalculateDefaultClassCount(data);
            }
        }


        public VariatyProcessing(List<double> inputData)
        {
            data = inputData;
            data.Sort();

            Variaties = FillVariaties(inputData);

            VariatyClasses = new();
            ClassCount = CalculateDefaultClassCount(data);
        }


        public static int CalculateDefaultClassCount(List<double> data)
        {
            return (int)(1 + 3.32 * Math.Log(data.Count));
        }

        public static List<Variaty> FillVariaties(List<double> data)
        {
            List<Variaty> variaties = new();

            for (int i = 0; i < data.Count; i++)
            {
                Variaty? item = variaties.Find(item => item.Value == data[i]);

                if (item != null)
                {
                    int freq = ++variaties[item.Id - 1].Frequency;
                    variaties[item.Id - 1].RelativeFrequency = (double)freq / data.Count;
                }
                else
                {
                    variaties.Add(new Variaty()
                    {
                        Id = variaties.Count + 1,
                        Value = data[i],
                        Frequency = 1,
                        RelativeFrequency = (double)1 / data.Count,
                        ECDFValue = 0.0
                    });
                }
            }

            // Count Variaties ECDF
            double step = 0.0;

            foreach (Variaty item in variaties)
            {
                item.ECDFValue = item.RelativeFrequency + step;
                step += item.RelativeFrequency;
            }

            return variaties;
        }

        public static double CalculateClassWidth(List<Variaty> variaties, int classesCount)
        {
            return (variaties[^1].Value - variaties[0].Value) / classesCount;
        }

        public static List<VariatyClass> ClassesDistribution(List<Variaty> variaties, int classesCount)
        {
            List<VariatyClass> variatyClasses = new();

            double minX = variaties[0].Value;
            double classesWidth = (variaties[^1].Value - minX) / classesCount;
            int counter = 0;

            for (int i = 0; i < classesCount; i++)
            {
                variatyClasses.Add(new VariatyClass()
                {
                    Id = i + 1,
                    MinValue = minX,
                    MaxValue = minX + classesWidth,
                    Value = $"[{minX}; {minX + classesWidth})",
                    Frequency = 0,
                    RelativeFrequency = 0.0,
                    ECDFValue = 0.0
                });

                VariatyClass current = variatyClasses[^1];

                while (counter < variaties.Count && variaties[counter].Value < minX + classesWidth)
                {
                    current.Frequency++;
                    counter++;
                }
                current.RelativeFrequency = (double)current.Frequency / variaties.Count;
                minX += classesWidth;
            }

            VariatyClass temp = variatyClasses[^1];
            temp.Frequency++;
            temp.RelativeFrequency = (double)temp.Frequency / variaties.Count;


            // Count Variaty Classes ECDF
            double step = 0.0;

            for (int i = 0; i < variatyClasses.Count; i++)
            {
                VariatyClass item = variatyClasses[i];
                item.ECDFValue = item.RelativeFrequency + step;
                step += item.RelativeFrequency;
            }

            return variatyClasses;
        }
    }
}
