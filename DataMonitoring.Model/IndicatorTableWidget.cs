using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataMonitoring.Model
{
    public class IndicatorTableWidget : IndicatorWidget
    {
        public bool HeaderDisplayed { get; set; }

        public int Sequence { get; set; }
        
        [StringLength(100)]
        public string RowStyleWhenEqualValue { get; set; }

        [StringLength(60)]
        public string ColumnCode { get; set; }

        [StringLength(60)]
        public string EqualsValue { get; set; }
        
        public ICollection<TableWidgetColumn> TableWidgetColumns { get; set; }
    }
}
