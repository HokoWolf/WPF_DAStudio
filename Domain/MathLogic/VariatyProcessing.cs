using DataAnalyzer.Models;
using System;
using System.Collections.Generic;

namespace DataAnalyzer.Domain.MathLogic
{
    public class VariatyProcessing
    {
        public List<Variaty> Variaties { get; private set; }
        public List<VariatyClass> VariatyClasses { get; private set; }

        public double ClassWidth { get; private set; }


        private int classCount;
        public int ClassCount
        {
            get { return classCount; }
            set
            {
                if (value > 0)
                {
                    classCount = value;
                    VariatyClasses = ClassesDistribution(Variaties, classCount);
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

                Variaties = FillVariaties(data);
                VariatyClasses = ClassesDistribution(Variaties, ClassCount);
            }
        }


        public VariatyProcessing(List<double> inputData)
        {
            data = inputData;
            data.Sort();

            Variaties = FillVariaties(inputData);

            VariatyClasses = new();
            ClassCount = CalculateDefaultClassCount();
        }


        public void SetDefaultClassCount()
        {
            ClassCount = CalculateDefaultClassCount();
        }


        private int CalculateDefaultClassCount()
        {
            return (int)(1 + 3.32 * Math.Log(Data.Count));
        }

        private List<Variaty> FillVariaties(List<double> data)
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

        private List<VariatyClass> ClassesDistribution(List<Variaty> variaties, int classesCount)
        {
            List<VariatyClass> variatyClasses = new();

            double minX = variaties[0].Value;
            ClassWidth = (variaties[^1].Value - minX) / classesCount;
            int counter = 0;

            for (int i = 0; i < classesCount; i++)
            {
                variatyClasses.Add(new VariatyClass()
                {
                    Id = i + 1,
                    MinValue = minX,
                    MaxValue = minX + ClassWidth,
                    Value = $"[{minX}; {minX + ClassWidth})",
                    Frequency = 0,
                    RelativeFrequency = 0.0,
                    ECDFValue = 0.0
                });

                VariatyClass current = variatyClasses[^1];

                while (variaties[counter].Value < minX + ClassWidth)
                {
                    current.Frequency++;
                    counter++;
                }
                current.RelativeFrequency = (double)current.Frequency / variaties.Count;
                minX += ClassWidth;
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
