using DataAnalyzer.Domain.MathLogic;
using DataAnalyzer.Models;
using DataAnalyzer.MVVM;
using DataAnalyzer.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DataAnalyzer.ViewModels
{
    public class EditorWindowViewModel : INotifyPropertyChanged
    {
        private readonly MathCalculations mathCalculations;
        private readonly VariatyProcessing variatyProcessing;

        public ObservableCollection<Variaty> Variaties { get; set; }
        public ObservableCollection<VariatyClass> VariatyClasses { get; set; }
        public ObservableCollection<Characteristic> Characteristics { get; set; }

        public int ClassCount
        {
            get { return variatyProcessing.ClassCount; }
            set
            {
                variatyProcessing.ClassCount = value;

                VariatyClasses.Clear();
                foreach (VariatyClass item in variatyProcessing.VariatyClasses)
                {
                    VariatyClasses.Add(item);
                }

                OnPropertyChanged();
            }
        }


        public RelayCommand ChangeClassCountCommand => new (execute => { ChangeClassCount(); });


        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public EditorWindowViewModel(List<double> inputData)
        {
            variatyProcessing = new(inputData);
            Variaties = new(variatyProcessing.Variaties);
            VariatyClasses = new(variatyProcessing.VariatyClasses);

            mathCalculations = new(inputData);
            Characteristics = new(mathCalculations.Characteristics);
            RoundCharacteristicsValues();
        }


        private void RoundCharacteristicsValues()
        {
            foreach (var item in Characteristics)
            {
                item.Evaluation = Math.Round(item.Evaluation, 4);
            }

            Characteristics[0].EvaluationDiviation =
                Math.Round(Convert.ToDouble(Characteristics[0].EvaluationDiviation), 4);
            Characteristics[2].EvaluationDiviation =
                Math.Round(Convert.ToDouble(Characteristics[2].EvaluationDiviation), 4);
            Characteristics[3].EvaluationDiviation =
                Math.Round(Convert.ToDouble(Characteristics[3].EvaluationDiviation), 4);
            Characteristics[4].EvaluationDiviation =
                Math.Round(Convert.ToDouble(Characteristics[4].EvaluationDiviation), 4);

            for (int i = 0; i < 5; i++)
            {
                Characteristics[i].ConfidenceInterval =
                    $"[{Math.Round(Convert.ToDouble(Characteristics[i].ConfidenceIntervalMin), 4)} ; " +
                    $"{Math.Round(Convert.ToDouble(Characteristics[i].ConfidenceIntervalMax), 4)}]";
            }
        }

        private void ChangeClassCount()
        {
            ChangeClassCountWindow changeWindow = new(this);
            changeWindow.ShowDialog();
        }
    }
}
