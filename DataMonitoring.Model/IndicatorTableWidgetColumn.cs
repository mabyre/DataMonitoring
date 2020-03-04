using System.ComponentModel.DataAnnotations;

namespace DataMonitoring.Model
{
    public class IndicatorTableWidgetColumn : TableWidgetColumn , IIndicatorColumn
    {
        public IndicatorTableWidgetColumn() {}

        public IndicatorTableWidgetColumn(TableWidgetColumn tableWidgetColumn)
            : base(tableWidgetColumn) {}

        public bool Filtered { get; set; }

        [StringLength(60)]
        public string FilteredValue { get; set; }

        public bool IsNumericFormat { get; set; }

        public bool TranspositionColumn { get; set; }

        public bool TranspositionValue { get; set; }

        public bool TranspositionRow { get; set; }
    }
}
