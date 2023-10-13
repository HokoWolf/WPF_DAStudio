using DataAnalyzer.Models;
using DataAnalyzer.MVVM;
using DataAnalyzer.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DataAnalyzer.ViewModels
{
    public class EditorWindowViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Variaty> Variaties { get; set; }
        public ObservableCollection<VariatyClass> VariatyClasses { get; set; }
        public ObservableCollection<Characteristic> Characteristics { get; set; }


        private int classCount;
        public int ClassCount
        {
            get { return classCount; }
            set
            {
                classCount = value > 0 ? value : 5;

                ClassesDistribution();
                CountVariatyClassesECDF();

                OnPropertyChanged();
            }
        }


        public RelayCommand ChangeClassCountCommand => new (execute => { ChangeClassCount(); });


        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public EditorWindowViewModel(List<double> variaties)
        {
            Variaties = new();
            VariatyClasses = new();
            Characteristics = new();
            
            FillVariaties(variaties);
            CountVariatiesECDF();

            ClassCount = 5;

            FillCharacteristics();
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
            VariatyClasses.Clear();

            double minX = Variaties[0].Value;
            double h = (Variaties[^1].Value - minX) / ClassCount;
            int counter = 0;

            for (int i = 0; i < ClassCount; i++)
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

                VariatyClass current = VariatyClasses[^1];

                while (Variaties[counter].Value < minX + h)
                {
                    current.Frequency++;
                    counter++;
                }
                current.RelativeFrequency = (double)current.Frequency / Variaties.Count;
                minX += h;
            }

            VariatyClass temp = VariatyClasses[^1];
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

        private void FillCharacteristics()
        {
            Characteristics.Add(new Characteristic()
            {
                Name = "Середнє арифметичне",
                Evaluatioin = 0,
                EvaluationDiviation = 0,
                ConfidenceIntervalMin = 0,
                ConfidenceIntervalMax = 0,
                ConfidenceInterval = "[ ; ]"
            });

            Characteristics.Add(new Characteristic()
            {
                Name = "Медіана",
                Evaluatioin = 0,
                EvaluationDiviation = 0,
                ConfidenceIntervalMin = 0,
                ConfidenceIntervalMax = 0,
                ConfidenceInterval = "[ ; ]"
            });

            Characteristics.Add(new Characteristic()
            {
                Name = "Середньоквадратичне відхилення",
                Evaluatioin = 0,
                EvaluationDiviation = 0,
                ConfidenceIntervalMin = 0,
                ConfidenceIntervalMax = 0,
                ConfidenceInterval = "[ ; ]"
            });

            Characteristics.Add(new Characteristic()
            {
                Name = "Коефіцієнт асиметрії",
                Evaluatioin = 0,
                EvaluationDiviation = 0,
                ConfidenceIntervalMin = 0,
                ConfidenceIntervalMax = 0,
                ConfidenceInterval = "[ ; ]"
            });

            Characteristics.Add(new Characteristic()
            {
                Name = "Коефіцієнт ексцесу",
                Evaluatioin = 0,
                EvaluationDiviation = 0,
                ConfidenceIntervalMin = 0,
                ConfidenceIntervalMax = 0,
                ConfidenceInterval = "[ ; ]"
            });

            Characteristics.Add(new Characteristic()
            {
                Name = "Мінімум",
                Evaluatioin = 0,
                EvaluationDiviation = 0,
                ConfidenceIntervalMin = 0,
                ConfidenceIntervalMax = 0,
                ConfidenceInterval = "[ ; ]"
            });

            Characteristics.Add(new Characteristic()
            {
                Name = "Максимум",
                Evaluatioin = 0,
                EvaluationDiviation = 0,
                ConfidenceIntervalMin = 0,
                ConfidenceIntervalMax = 0,
                ConfidenceInterval = "[ ; ]"
            });
        }


        private void ChangeClassCount()
        {
            ChangeClassCountWindow changeWindow = new(this);
            changeWindow.ShowDialog();
        }
    }
}
