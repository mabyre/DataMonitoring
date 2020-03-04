namespace DataMonitoring.Model
{
    public class IndicatorChartWidgetColumn : IIndicatorColumn
    {
        public string Code { get; set; }
        public bool Filtered { get; set; }
        public string FilteredValue { get; set; }
        public bool IsNumericFormat { get; set; }
    }
}
