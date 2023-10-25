using DataAnalyzer.Domain.MathLogic;
using DataAnalyzer.Models;
using LiveCharts.Wpf;
using LiveCharts;
using System.Collections.ObjectModel;
using System.Windows.Media;
using LiveCharts.Defaults;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAnalyzer.ViewModels
{
    public class IdentifyWindowViewModel
    {
        // IdentifyWindow SkewnessAndKurtosis - maybe
        public ObservableCollection<Characteristic> SkewnessAndKurtosis { get; set; }
        public string NormalDeviationLabel { get; private set; }
        public string SkewnessEquation { get; private set; }
        public string SkewnessResult { get; private set; }
        public SolidColorBrush SkewnessResultColor { get; private set; }
        public string KurtosisEquation { get; private set; }
        public string KurtosisResult { get; private set; }
        public SolidColorBrush KurtosisResultColor { get; private set; }
        public string IdentifyResult { get; private set; }
        public SolidColorBrush IdentifyResultColor { get; private set; }

        // IdentifyWindow PossibilityPaper - maybe
        public SeriesCollection PossibilityPaperSeries { get; set; }
        public AxesCollection PossibilityPaperAxesX { get; set; }
        public AxesCollection PossibilityPaperAxesY { get; set; }


        public IdentifyWindowViewModel(List<Variaty> variaties, MathCalculations mathCalculations)
        {
            // SkewnessAndKurtosis
            SkewnessAndKurtosis = new()
            {
                mathCalculations.Characteristics[3],
                mathCalculations.Characteristics[4]
            };

            int maxPrecision = BitConverter.GetBytes(decimal.GetBits((decimal)SkewnessAndKurtosis[0].Evaluation)[3])[2];

            double u = Math.Round(MathCalculations.CalculateNormalQuantile(mathCalculations.ConfidenceLevel), maxPrecision);
            double uA = Math.Round(Convert.ToDouble(SkewnessAndKurtosis[0].Evaluation / SkewnessAndKurtosis[0].EvaluationDiviation), maxPrecision);
            double uE = Math.Round(Convert.ToDouble(SkewnessAndKurtosis[1].Evaluation / SkewnessAndKurtosis[1].EvaluationDiviation), maxPrecision);

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


            // PossibilityPaper
            ChartValues<ObservablePoint> points = new(from x in variaties where x.ECDFValue < 1
                select new ObservablePoint(x.Value, MathCalculations.CalculateNormalQuantile(x.ECDFValue)));

            PossibilityPaperSeries = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "",
                    Values = new ChartValues<ObservablePoint>
                    {
                        new ObservablePoint(points[0].X, points[0].Y),
                        new ObservablePoint(points[^1].X, points[^1].Y)
                    },
                    PointGeometrySize = 0,
                    Stroke = new SolidColorBrush(Colors.Red),
                    Fill = new SolidColorBrush(Colors.Transparent),
                    DataLabels = false,
                    DataLabelsTemplate = null
                },
                new ScatterSeries
                {
                    Title = "",
                    Values = points,
                    LabelPoint = value => $"z({value.X}) = {value.Y}",
                    PointGeometry = DefaultGeometries.Diamond,
                    Stroke = new SolidColorBrush(Colors.Black),
                    Fill = new SolidColorBrush(Colors.Black)
                }
            };

            PossibilityPaperAxesX = new AxesCollection
            {
                new Axis
                {
                    Title = "t = x",
                    LabelFormatter = value => "",
                    MinValue = points[0].X,
                    Separator = new Separator
                    {
                        IsEnabled = true,
                        StrokeThickness = 1,
                        StrokeDashArray = new DoubleCollection(new double[] {2}),
                        Stroke = new SolidColorBrush(Color.FromArgb(255, 131, 131, 131))
                    }
                }
            };

            PossibilityPaperAxesY = new AxesCollection
            {
                new Axis
                {
                    Title = "z = u(F(x))",
                    LabelFormatter = value => value.ToString("N"),
                    MinValue = points[0].Y,
                    Separator = new Separator
                    {
                        IsEnabled = true,
                        StrokeThickness = 1,
                        StrokeDashArray = new DoubleCollection(new double[] {2}),
                        Stroke = new SolidColorBrush(Color.FromArgb(255, 131, 131, 131)),
                    }
                }
            };
        }
    }
}
