using System.ComponentModel.DataAnnotations;

namespace DataMonitoring.Model
{
    public class TranspositionColumnTableWidgetColumn : TableWidgetColumn 
    {
        public TranspositionColumnTableWidgetColumn() { }

        public TranspositionColumnTableWidgetColumn(TableWidgetColumn tableWidgetColumn)
            : base(tableWidgetColumn) { }
    }
}
