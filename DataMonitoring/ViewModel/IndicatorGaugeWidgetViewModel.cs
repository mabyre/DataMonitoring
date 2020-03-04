namespace DataMonitoring.ViewModel
{
    public class IndicatorGaugeWidgetViewModel : IndicatorWidgetViewModel
    {
        public bool TargetDisplayed { get; set; }

        // Groups
        public string Group1 { get; set; }
        public string Group2 { get; set; }
        public string Group3 { get; set; }
        public string Group4 { get; set; }
        public string Group5 { get; set; }

        public string GaugeTargetColor { get; set; }

        public string GaugeRange1Color { get; set; }
        public decimal? GaugeRange1MinValue { get; set; }
        public decimal? GaugeRange1MaxValue { get; set; }

        public bool? GaugeRange2Displayed { get; set; }
        public string GaugeRange2Color { get; set; }
        public decimal? GaugeRange2MinValue { get; set; }
        public decimal? GaugeRange2MaxValue { get; set; }

        public bool? GaugeRange3Displayed { get; set; }
        public string GaugeRange3Color { get; set; }
        public decimal? GaugeRange3MinValue { get; set; }
        public decimal? GaugeRange3MaxValue { get; set; }
    }
}
