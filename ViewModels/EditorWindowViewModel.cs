using DataAnalyzer.Domain.MathLogic;
using DataAnalyzer.Models;
using DataAnalyzer.MVVM;
using DataAnalyzer.Views;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace DataAnalyzer.ViewModels
{
    public class EditorWindowViewModel : INotifyPropertyChanged
    {
        // MathLogic classes
        private readonly MathCalculations mathCalculations;
        private readonly VariatyProcessing variatyProcessing;
        private readonly DiagramsCalculations diagramsCalculations;


        // ObservableCollections
        public ObservableCollection<Variaty> Variaties { get; set; }
        public ObservableCollection<VariatyClass> VariatyClasses { get; set; }
        public ObservableCollection<Characteristic> Characteristics { get; set; }


        // MainChart
        public SeriesCollection MainChartSeries { get; set; }
        public AxesCollection MainChartAxesX { get; set; }
        public AxesCollection MainChartAxesY { get; set; }


        // MainProperties
        public List<double> Data
        {
            get { return variatyProcessing.Data; }
            set
            {
                variatyProcessing.Data = value;
                mathCalculations.Data = value;
                diagramsCalculations.Data = value;

                Variaties.Clear();
                foreach (var item in variatyProcessing.Variaties)
                {
                    Variaties.Add(item);
                }

                VariatyClasses.Clear();
                foreach (var item in variatyProcessing.VariatyClasses)
                {
                    VariatyClasses.Add(item);
                }

                Characteristics.Clear();
                foreach (var item in mathCalculations.Characteristics)
                {
                    Characteristics.Add(item);
                }
            }
        }

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


        // Commands
        public RelayCommand ChangeClassCountCommand => new (execute => { ChangeClassCount(); });


        // OnPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



        public EditorWindowViewModel(List<double> inputData)
        {
            // VariatyProcessing
            variatyProcessing = new(inputData);
            Variaties = new(variatyProcessing.Variaties);
            VariatyClasses = new(variatyProcessing.VariatyClasses);

            // CharacteristicsCalculations
            mathCalculations = new(inputData);
            Characteristics = new(mathCalculations.Characteristics);
            RoundCharacteristicsValues();


            // Charts
            diagramsCalculations = new(inputData);

            // MainChart
            MainChartSeries = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Частота",
                    Values = new ChartValues<double>(VariatyClasses.Select(vc => vc.RelativeFrequency)),
                    LabelPoint = value => $"p={value.Y}",
                    ScalesXAt = 0,
                    ScalesYAt = 0,
                    MaxColumnWidth = 1000,
                    ColumnPadding = 0,
                    Fill = new SolidColorBrush(Color.FromArgb(255, 206, 185, 102)),
                    Stroke = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0)),
                    StrokeThickness = 1
                },
                new LineSeries
                {
                    Title = "Щільність",
                    Values = new ChartValues<double>(diagramsCalculations.KDEValues
                        .Select(x => x * variatyProcessing.ClassWidth)),
                    LabelPoint = value => $"f(x)={value.Y}",
                    ScalesXAt = 1,
                    ScalesYAt = 0,
                    LineSmoothness = 0,
                    Fill = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)),
                    Stroke = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0))
                }
            };

            MainChartAxesX = new AxesCollection
            {
                new Axis
                {
                    Title = "Значення, x",
                    Labels = VariatyClasses.Select(x => x.Value).ToList(),
                    ShowLabels = false,
                    Separator = new Separator
                    {
                        IsEnabled = true,
                        StrokeThickness = 1,
                        StrokeDashArray = new DoubleCollection(new double[] {2}),
                        Stroke = new SolidColorBrush(Color.FromArgb(255, 131, 131, 131))
                    }
                },
                new Axis
                {
                    Labels = Data.Select(x => x.ToString()).ToList(),
                    ShowLabels = false,
                    LabelsRotation = 0
                }
            };

            MainChartAxesY = new AxesCollection
            {
                new Axis
                {
                    Title = "Відносна частота, p",
                    LabelFormatter = value => value.ToString("N"),
                    Separator = new Separator
                    {
                        IsEnabled = true,
                        StrokeThickness = 1,
                        StrokeDashArray = new DoubleCollection(new double[] {2}),
                        Stroke = new SolidColorBrush(Color.FromArgb(255, 131, 131, 131))
                    }
                }
            };
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
