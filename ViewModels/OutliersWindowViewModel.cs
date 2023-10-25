using DataAnalyzer.Domain.MathLogic;
using LiveCharts.Wpf;
using LiveCharts;
using LiveCharts.Defaults;
using System.Linq;
using System.Windows.Media;
using System;

namespace DataAnalyzer.ViewModels
{
    public class OutliersWindowViewModel
    {
        public SeriesCollection OutliersChartSeries { get; set; }
        public AxesCollection OutliersChartAxesX { get; set; }
        public AxesCollection OutliersChartAxesY { get; set; }


        public OutliersWindowViewModel(ChartsCalculations chartsCalculations)
        {
            OutliersChartSeries = new SeriesCollection
            {
                new ScatterSeries
                {
                    Title = "Нормальні",
                    Values = new ChartValues<ObservablePoint>(chartsCalculations.DataWithoutOutliers
                        .Select(x => new ObservablePoint(x.Key, x.Value))),
                    LabelPoint = value => $"{value.X} - {value.Y}",
                    PointGeometry = DefaultGeometries.Diamond,
                    Stroke = new SolidColorBrush(Colors.Black),
                    Fill = new SolidColorBrush(Colors.Black)
                },
                new ScatterSeries
                {
                    Title = "Аномальні",
                    Values = new ChartValues<ObservablePoint>(chartsCalculations.Outliers
                        .Select(x => new ObservablePoint(x.Key, x.Value))),
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
                    LabelFormatter = value => value.ToString("N"),
                    MinValue = Math.Min(chartsCalculations.Data[0], chartsCalculations.OutlierMinEdge) - 1,
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
                            Value = chartsCalculations.OutlierMinEdge,
                            SectionWidth = chartsCalculations.OutlierMaxEdge - chartsCalculations.OutlierMinEdge,
                            Fill = new SolidColorBrush(Colors.Transparent),
                            Stroke = new SolidColorBrush(Colors.Red),
                            StrokeThickness = 2
                        }
                    }
                }
            };
        }
    }
}
