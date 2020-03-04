namespace DataMonitoring.Model
{
    public class IndicatorGaugeWidgetColumn : IIndicatorColumn
    {
        public string Code { get; set; }
        public bool Filtered { get; set; }
        public string FilteredValue { get; set; }
        public bool IsNumericFormat { get; set; }
    }
}
