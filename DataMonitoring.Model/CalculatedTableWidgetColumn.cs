namespace DataMonitoring.Model
{
    public class CalculatedTableWidgetColumn : TableWidgetColumn
    {
        public CalculatedTableWidgetColumn() { }

        public CalculatedTableWidgetColumn(TableWidgetColumn tableWidgetColumn)
            : base(tableWidgetColumn) { }

        public string PartialValueColumn { get; set; }

        public string TotalValueColumn { get; set; }
    }
}
