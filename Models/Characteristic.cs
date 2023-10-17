namespace DataAnalyzer.Models
{
    public class Characteristic
    {
        public string Name { get; set; } = "";

        public double Evaluation { get; set; }

        public double? EvaluationDiviation { get; set; }

        public double? ConfidenceIntervalMin {  get; set; }

        public double? ConfidenceIntervalMax {  get; set; }

        public string? ConfidenceInterval { get; set; }
    }
}
