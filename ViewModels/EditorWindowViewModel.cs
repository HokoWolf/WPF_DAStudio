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
        bool _isOutliersGenerated;

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

        // OutliersChart
        public ChartValues<ObservablePoint>? OutliersChartNormalValues { get; set; }
        public ChartValues<ObservablePoint>? OutliersChartOutlierValues { get; set; }
        public SeriesCollection? OutliersChartSeries { get; set; }
        public AxesCollection? OutliersChartAxesX { get; set; }
        public AxesCollection? OutliersChartAxesY { get; set; }


        // IdentifyWindow
        public ObservableCollection<Characteristic>? SkewnessAndKurtosis { get; set; }
        public string NormalDeviationLabel { get; private set; }
        public string SkewnessEquation { get; private set; }
        public string SkewnessResult { get; private set; }
        public SolidColorBrush SkewnessResultColor { get; private set; }
        public string KurtosisEquation { get; private set; }
        public string KurtosisResult { get; private set; }
        public SolidColorBrush KurtosisResultColor { get; private set; }
        public string IdentifyResult { get; private set; }
        public SolidColorBrush IdentifyResultColor { get; private set; }


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

                MainChartSeries[1].Values = new ChartValues<ObservablePoint>(Data.Select((x, i) =>
                        new ObservablePoint(x, diagramsCalculations.KDEValues[i] * variatyProcessing.ClassWidth)));
                MainChartSeries[1].LabelPoint = value => $"f(x)={value.Y / variatyProcessing.ClassWidth}";
                (MainChartSeries[1] as LineSeries)!.PointGeometrySize = Data.Count < 100 ? 10 : 0;

                MainChartAxesX[0].Labels = VariatyClasses.Select(x => x.Value).ToList();
                MainChartAxesX[2].MaxValue = VariatyClasses.Count;

                // ECDFChart
                ECDFChartSeries[0].Values = new ChartValues<ObservablePoint>(Variaties.Select((x, i) =>
                        new ObservablePoint(x.Value, Variaties[i].ECDFValue)));
                (ECDFChartSeries[0] as StepLineSeries)!.PointGeometrySize = Variaties.Count < 50 ? 10 : 0;
                (ECDFChartSeries[0] as StepLineSeries)!.StrokeThickness = Variaties.Count < 50 ? 3 : 1;

                // OutliersChart
                _isOutliersGenerated = false;
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

                // MainChart
                MainChartSeries[0].Values = new ChartValues<double>(VariatyClasses.Select(vc => vc.RelativeFrequency));
                MainChartAxesX[0].Labels = VariatyClasses.Select(x => x.Value).ToList();

                MainChartSeries[1].Values = new ChartValues<ObservablePoint>(Data.Select((x, i) =>
                        new ObservablePoint(x, diagramsCalculations.KDEValues[i] * variatyProcessing.ClassWidth)));
                MainChartSeries[1].LabelPoint = value => $"f(x)={value.Y / variatyProcessing.ClassWidth}";

                MainChartAxesX[0].Labels = VariatyClasses.Select(x => x.Value).ToList();
                MainChartAxesX[2].MaxValue = VariatyClasses.Count;

                OnPropertyChanged();
            }
        }

        public double Bandwidth
        {
            get { return diagramsCalculations.Bandwidth; }
            set
            {
                diagramsCalculations.Bandwidth = value;

                MainChartSeries[1].Values = new ChartValues<ObservablePoint>(Data.Select((x, i) =>
                        new ObservablePoint(x, diagramsCalculations.KDEValues[i] * variatyProcessing.ClassWidth)));
                MainChartSeries[1].LabelPoint = value => $"f(x)={value.Y / variatyProcessing.ClassWidth}";

                OnPropertyChanged();
            }
        }


        // Commands
        public RelayCommand ChangeClassCountCommand => new(execute => { ChangeClassCount(); });
        public RelayCommand ChangeBandwidthCommand => new(execute => { ChangeBandwidth(); });
        public RelayCommand DeleteOutliers => new(execute => { DeleteOutliersCall(); });
        public RelayCommand IdentifyNormalDestribution => new(execute => { ShowIdenitifeWindow(); });


        // OnPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        // Ctors
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
                },
                new Axis
                {
                    LabelFormatter = x => "",
                    MinValue = 0,
                    MaxValue = VariatyClasses.Count,
                    Separator = new Separator
                    {
                        Step = 1,
                        IsEnabled = true,
                        StrokeThickness = 1,
                        StrokeDashArray = new DoubleCollection(new double[] {2}),
                        Stroke = new SolidColorBrush(Color.FromArgb(255, 131, 131, 131))
                    }
                }
            };

            MainChartAxesY = new AxesCollection
            {
                new Axis
                {
                    Title = "Відносна частота, p",
                    LabelFormatter = value => value.ToString("N"),
                    MinValue = 0,
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
                    Values = new ChartValues<ObservablePoint>(Variaties.Select((x, i) => 
                        new ObservablePoint(x.Value, Variaties[i].ECDFValue))),
                    LabelPoint = value => $"Fn(X)={value.Y}",
                    ScalesXAt = 0,
                    ScalesYAt = 0,
                    PointGeometrySize = Variaties.Count < 50 ? 10 : 0,
                    StrokeThickness = Variaties.Count < 50 ? 3 : 1,
                    Stroke = new SolidColorBrush(Color.FromArgb(255, 206, 185, 102)),
                    AlternativeStroke = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)),
                }
            };

            ECDFChartAxesX = new AxesCollection
            {
                new Axis
                {
                    Title = "Значення варіанти, х",
                    ShowLabels = false,
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

            // OutliersChart
            _isOutliersGenerated = false;
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


        private void DeleteOutliersCall()
        {
            GenerateOutliersChart();

            DeleteOutliersWindow deleteWindow = new(this);

            bool? result = deleteWindow.ShowDialog();
            if (result == true)
            {
                Data = DiagramsCalculations.DeleteOutliers(diagramsCalculations.UnsortedData);
            }
        }

        private void GenerateOutliersChartValues()
        {
            if (!_isOutliersGenerated)
            {
                OutliersChartNormalValues = new(diagramsCalculations.UnsortedData.Select(
                        (x, i) => new ObservablePoint(i, x)).Where(
                        x => x.Y >= diagramsCalculations.OutlierMinEdge &&
                        x.Y <= diagramsCalculations.OutlierMaxEdge));

                OutliersChartOutlierValues = new(diagramsCalculations.UnsortedData.Select(
                        (x, i) => new ObservablePoint(i, x)).Where(
                        x => x.Y < diagramsCalculations.OutlierMinEdge ||
                        x.Y > diagramsCalculations.OutlierMaxEdge));

                _isOutliersGenerated = true;
            }
        }

        private void GenerateOutliersChart()
        {
            GenerateOutliersChartValues();

            OutliersChartSeries = new SeriesCollection
            {
                new ScatterSeries
                {
                    Title = "Нормальні",
                    Values = OutliersChartNormalValues,
                    LabelPoint = value => $"{value.X} - {value.Y}",
                    PointGeometry = DefaultGeometries.Diamond,
                    Stroke = new SolidColorBrush(Colors.Black),
                    Fill = new SolidColorBrush(Colors.Black)
                },
                new ScatterSeries
                {
                    Title = "Аномальні",
                    Values = OutliersChartOutlierValues,
                    LabelPoint = value => $"{value.X} - {value.Y}",
                    PointGeometry = DefaultGeometries.Diamond,
                    Stroke = new SolidColorBrush(Colors.Red),
                    Fill = new SolidColorBrush(Colors.Red)
                }
            };

            OutliersChartAxesX = new AxesCollection
            {
                new Axis
                {
                    Title = "Індекси, i",
                    LabelFormatter = value => value.ToString(),
                    Separator = new Separator
                    {
                        IsEnabled = true,
                        StrokeThickness = 1,
                        StrokeDashArray = new DoubleCollection(new double[] {2}),
                        Stroke = new SolidColorBrush(Color.FromArgb(255, 131, 131, 131))
                    }
                }
            };

            OutliersChartAxesY = new AxesCollection
            {
                new Axis
                {
                    Title = "Значення, x",
                    LabelFormatter = value => value.ToString(),
                    MinValue = Math.Min(Data[0], diagramsCalculations.OutlierMinEdge) - 1,
                    Separator = new Separator
                    {
                        IsEnabled = true,
                        StrokeThickness = 1,
                        StrokeDashArray = new DoubleCollection(new double[] {2}),
                        Stroke = new SolidColorBrush(Color.FromArgb(255, 131, 131, 131))
                    },
                    Sections = new SectionsCollection
                    {
                        new AxisSection
                        {
                            Value = diagramsCalculations.OutlierMinEdge,
                            SectionWidth = diagramsCalculations.OutlierMaxEdge - diagramsCalculations.OutlierMinEdge,
                            Fill = new SolidColorBrush(Colors.Transparent),
                            Stroke = new SolidColorBrush(Colors.Red),
                            StrokeThickness = 2
                        }
                    }
                }
            };
        }


        private void ShowIdenitifeWindow()
        {
            SkewnessAndKurtosis = new()
            {
                Characteristics[3],
                Characteristics[4]
            };

            double u = Math.Round(MathCalculations.CalculateNormalQuantile(mathCalculations.ConfidenceLevel), _maxPrecision);
            double uA = Math.Round(Convert.ToDouble(SkewnessAndKurtosis[0].Evaluation / SkewnessAndKurtosis[0].EvaluationDiviation), _maxPrecision);
            double uE = Math.Round(Convert.ToDouble(SkewnessAndKurtosis[1].Evaluation / SkewnessAndKurtosis[1].EvaluationDiviation), _maxPrecision);

            bool uAFlag = Math.Abs(uA) > u;
            bool uEFlag = Math.Abs(uE) > u;

            NormalDeviationLabel = $"a = {mathCalculations.ConfidenceLevel}, u(1 - a/2) = {u}";

            SkewnessEquation = $"u(A) = {SkewnessAndKurtosis[0].Evaluation} / {SkewnessAndKurtosis[0].EvaluationDiviation} = {uA}";
            SkewnessResult = uAFlag ? $"|u(A)| > {u}" : $"|u(A)| <= {u}";
            SkewnessResultColor = uAFlag ? Brushes.Red : Brushes.Green;

            KurtosisEquation = $"u(E) = {SkewnessAndKurtosis[1].Evaluation} / {SkewnessAndKurtosis[1].EvaluationDiviation} = {uE}";
            KurtosisResult = uEFlag ? $"|u(E)| > {u}" : $"|u(E)| <= {u}";
            KurtosisResultColor = uEFlag ? Brushes.Red : Brushes.Green;

            IdentifyResult = !uAFlag && !uEFlag ? "Так, ідентифікується" : "Ні, не ідентифікується";
            IdentifyResultColor = !uAFlag && !uEFlag ? Brushes.Green : Brushes.Red;

            IdentifyDestributionWindow identifyWindow = new(this);
            identifyWindow.ShowDialog();
        }
    }
}
