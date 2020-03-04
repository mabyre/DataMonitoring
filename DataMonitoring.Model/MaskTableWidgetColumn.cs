namespace DataMonitoring.Model
{
    public class MaskTableWidgetColumn : TableWidgetColumn
    {
        public MaskTableWidgetColumn() { }

        public MaskTableWidgetColumn(TableWidgetColumn tableWidgetColumn)
            : base(tableWidgetColumn) { }

        public string DisplayModel { get; set; }
    }
}
