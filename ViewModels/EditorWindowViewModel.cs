using DataAnalyzer.Domain.MathLogic;
using DataAnalyzer.Models;
using DataAnalyzer.MVVM;
using DataAnalyzer.Views;
using LiveCharts;
using LiveCharts.Defaults;
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
        int _maxPrecision;

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

        // ECDFChart
        public SeriesCollection ECDFChartSeries { get; set; }
        public AxesCollection ECDFChartAxesX { get; set; }
        public AxesCollection ECDFChartAxesY { get; set; }


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

                // Rounding
                _maxPrecision = CalculateMaxPrecision();
                RoundVariaties();
                RoundVariatyClasses();
                RoundCharacteristics();

                // MainChart
                MainChartSeries[0].Values = new ChartValues<double>(VariatyClasses.Select(vc => vc.RelativeFrequency));
                MainChartAxesX[0].Labels = VariatyClasses.Select(x => x.Value).ToList();
                MainChartAxesX[0].Labels = Data.Select(x => x.ToString()).ToList();

                MainChartSeries[1].Values = new ChartValues<double>(diagramsCalculations.KDEValues
                        .Select(x => x * variatyProcessing.ClassWidth));
                MainChartSeries[1].LabelPoint = value => $"f(x)={value.Y / variatyProcessing.ClassWidth}";
                (MainChartSeries[1] as LineSeries)!.PointGeometrySize = Data.Count < 100 ? 10 : 0;

                // ECDFChart
                ECDFChartSeries[0].Values = new ChartValues<double>(Variaties.Select(v => v.ECDFValue));
                ECDFChartAxesX[0].Labels = Variaties.Select(x => x.Value.ToString()).ToList();
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

                RoundVariatyClasses();

                MainChartSeries[0].Values = new ChartValues<double>(VariatyClasses.Select(vc => vc.RelativeFrequency));
                MainChartAxesX[0].Labels = VariatyClasses.Select(x => x.Value).ToList();

                MainChartSeries[1].Values = new ChartValues<double>(diagramsCalculations.KDEValues
                        .Select(x => x * variatyProcessing.ClassWidth));
                MainChartSeries[1].LabelPoint = value => $"f(x)={value.Y / variatyProcessing.ClassWidth}";

                OnPropertyChanged();
            }
        }

        public double Bandwidth
        {
            get { return diagramsCalculations.Bandwidth; }
            set
            {
                diagramsCalculations.Bandwidth = value;

                MainChartSeries[1].Values = new ChartValues<double>(diagramsCalculations.KDEValues
                        .Select(x => x * variatyProcessing.ClassWidth));
                MainChartSeries[1].LabelPoint = value => $"f(x)={value.Y / variatyProcessing.ClassWidth}";

                OnPropertyChanged();
            }
        }


        // Commands
        public RelayCommand ChangeClassCountCommand => new (execute => { ChangeClassCount(); });
        public RelayCommand ChangeBandwidthCommand => new (execute => { ChangeBandwidth(); });


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

            // Charts
            diagramsCalculations = new(inputData);

            // Rounding
            _maxPrecision = CalculateMaxPrecision();
            RoundVariaties();
            RoundVariatyClasses();
            RoundCharacteristics();

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
                    Values = new ChartValues<ObservablePoint>(Data.Select((x, i) => 
                        new ObservablePoint(x, diagramsCalculations.KDEValues[i] * variatyProcessing.ClassWidth))),
                    LabelPoint = value => $"f(x)={value.Y / variatyProcessing.ClassWidth}",
                    ScalesXAt = 1,
                    ScalesYAt = 0,
                    LineSmoothness = 1,
                    PointGeometrySize = Data.Count < 100 ? 10 : 0,
                    Fill = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)),
                    Stroke = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0))
                }
            };

            MainChartAxesX = new AxesCollection
            {
                new Axis
                {
                    Labels = VariatyClasses.Select(x => x.Value).ToList(),
                    ShowLabels = false
                },
                new Axis
                {
                    Title = "Значення, x",
                    ShowLabels = false
                }
            };

            MainChartAxesY = new AxesCollection
            {
                new Axis
                {
                    Title = "Відносна частота, p",
                    LabelFormatter = value => value.ToString("N"),
                    MinValue = 0.0,
                    Separator = new Separator
                    {
                        IsEnabled = true,
                        StrokeThickness = 1,
                        StrokeDashArray = new DoubleCollection(new double[] {2}),
                        Stroke = new SolidColorBrush(Color.FromArgb(255, 131, 131, 131))
                    }
                }
            };

            // ECDFChart
            ECDFChartSeries = new SeriesCollection
            {
                new StepLineSeries
                {
                    Title = "ECDF",
                    Values = new ChartValues<double>(Variaties.Select(v => v.ECDFValue)),
                    LabelPoint = value => $"Fn(X)={value.Y}",
                    ScalesXAt = 0,
                    ScalesYAt = 0,
                    PointGeometrySize = 10,
                    Stroke = new SolidColorBrush(Color.FromArgb(255, 206, 185, 102)),
                    AlternativeStroke = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0))
                }
            };

            ECDFChartAxesX = new AxesCollection
            {
                new Axis
                {
                    Labels = Variaties.Select(x => x.Value.ToString()).ToList(),
                    Separator = new Separator
                    {
                        IsEnabled = true,
                        StrokeThickness = 1,
                        StrokeDashArray = new DoubleCollection(new double[] {2}),
                        Stroke = new SolidColorBrush(Color.FromArgb(255, 131, 131, 131)),
                        Step = 1
                    }
                }
            };

            ECDFChartAxesY = new AxesCollection
            {
                new Axis
                {
                    Title = "ECDF, Fn(x)",
                    LabelFormatter = value => value.ToString("N"),
                    MinValue = 0.0,
                    MaxValue = 1.0,
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


        // Rounding
        private int CalculateMaxPrecision()
        {
            int maxPrecision = BitConverter.GetBytes(decimal.GetBits((decimal)Data[0])[3])[2];

            foreach (var values in Data)
            {
                int precision = BitConverter.GetBytes(decimal.GetBits((decimal)values)[3])[2];
                maxPrecision = precision > maxPrecision ? precision : maxPrecision;
            }

            return maxPrecision;
        }

        private void RoundVariaties()
        {
            foreach (var item in Variaties)
            {
                item.RelativeFrequency = Math.Round(item.RelativeFrequency, _maxPrecision);
                item.ECDFValue = Math.Round(item.ECDFValue, _maxPrecision);
            }
        }

        private void RoundVariatyClasses()
        {
            foreach (var item in VariatyClasses)
            {
                item.MinValue = Math.Round(item.MinValue, _maxPrecision);
                item.MaxValue = Math.Round(item.MaxValue, _maxPrecision);
                item.Value = $"[{item.MinValue} ; {item.MaxValue}]";
                item.RelativeFrequency = Math.Round(item.RelativeFrequency, _maxPrecision);
                item.ECDFValue = Math.Round(item.ECDFValue, _maxPrecision);
            }
        }

        private void RoundCharacteristics()
        {
            foreach (var item in Characteristics)
            {
                item.Evaluation = Math.Round(item.Evaluation, _maxPrecision);
            }

            Characteristics[0].EvaluationDiviation =
                Math.Round(Convert.ToDouble(Characteristics[0].EvaluationDiviation), _maxPrecision);
            Characteristics[2].EvaluationDiviation =
                Math.Round(Convert.ToDouble(Characteristics[2].EvaluationDiviation), _maxPrecision);
            Characteristics[3].EvaluationDiviation =
                Math.Round(Convert.ToDouble(Characteristics[3].EvaluationDiviation), _maxPrecision);
            Characteristics[4].EvaluationDiviation =
                Math.Round(Convert.ToDouble(Characteristics[4].EvaluationDiviation), _maxPrecision);

            for (int i = 0; i < 5; i++)
            {
                Characteristics[i].ConfidenceInterval =
                    $"[{Math.Round(Convert.ToDouble(Characteristics[i].ConfidenceIntervalMin), _maxPrecision)} ; " +
                    $"{Math.Round(Convert.ToDouble(Characteristics[i].ConfidenceIntervalMax), _maxPrecision)}]";
            }
        }


        // Methods for Commands
        private void ChangeClassCount()
        {
            ChangeClassCountWindow changeWindow = new(this);
            changeWindow.ShowDialog();
        }

        private void ChangeBandwidth()
        {
            ChangeBandwidthWindow changeWindow = new(this);
            changeWindow.ShowDialog();
        }
    }
}
