using DataAnalyzer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DataAnalyzer.ViewModel
{
    public class EditorWindowController
    {
        private int classesCount;

        public int ClassesCount
        {
            get { return classesCount; }
            set
            {
                classesCount = value > 0 ? value : 5;

                ClassesDistribution();
                CountVariatyClassesECDF();
            }
        }


        public ObservableCollection<Variaty> Variaties { get; set; }
        public ObservableCollection<VariatyClass> VariatyClasses { get; set; }


        public EditorWindowController(List<double> variaties)
        {
            Variaties = new();
            VariatyClasses = new();
            
            FillVariaties(variaties);
            CountVariatiesECDF();

            ClassesCount = 5;
        }

        private void FillVariaties(List<double> variaties)
        {
            variaties.Sort();

            for (int i = 0; i < variaties.Count; i++)
            {
                Variaty? item = Variaties.FirstOrDefault(item => item.Value == variaties[i]);

                if (item != null)
                {
                    int freq = ++Variaties[item.Id - 1].Frequency;
                    Variaties[item.Id - 1].RelativeFrequency = (double)freq / variaties.Count;
                }
                else
                {
                    Variaties.Add(new Variaty()
                    {
                        Id = Variaties.Count + 1,
                        Value = variaties[i],
                        Frequency = 1,
                        RelativeFrequency = (double)1 / variaties.Count,
                        ECDFValue = 0.0
                    });
                }
            }
        }

        private void ClassesDistribution()
        {
            double minX = Variaties[0].Value;
            double h = (Variaties[Variaties.Count - 1].Value - minX) / ClassesCount;
            int counter = 0;

            for (int i = 0; i < ClassesCount; i++)
            {
                VariatyClasses.Add(new VariatyClass()
                {
                    Id = i + 1,
                    MinValue = minX,
                    MaxValue = minX + h,
                    Value = $"[{minX}; {minX + h})",
                    Frequency = 0,
                    RelativeFrequency = 0.0,
                    ECDFValue = 0.0
                });

                VariatyClass current = VariatyClasses[VariatyClasses.Count - 1];

                while (Variaties[counter].Value < minX + h)
                {
                    current.Frequency++;
                    counter++;
                }
                current.RelativeFrequency = (double)current.Frequency / Variaties.Count;
                minX += h;
            }

            VariatyClass temp = VariatyClasses[VariatyClasses.Count - 1];
            temp.Frequency++;
            temp.RelativeFrequency = (double)temp.Frequency / Variaties.Count;
        }

        private void CountVariatiesECDF()
        {
            double step = 0.0;

            foreach (var item in Variaties)
            {
                item.ECDFValue = item.RelativeFrequency + step;
                step += item.RelativeFrequency;

                item.RelativeFrequency = Math.Round(item.RelativeFrequency, 4);
                item.ECDFValue = Math.Round(item.ECDFValue, 4);
            }
        }

        private void CountVariatyClassesECDF()
        {
            double step = 0.0;

            foreach (var item in VariatyClasses)
            {
                item.ECDFValue = item.RelativeFrequency + step;
                step += item.RelativeFrequency;

                item.RelativeFrequency = Math.Round(item.RelativeFrequency, 4);
                item.ECDFValue = Math.Round(item.ECDFValue, 4);
            }
        }
    }
}
