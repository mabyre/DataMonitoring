using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMonitoring.Model
{
    public class IndicatorBarWidgetColumn : IIndicatorColumn
    {
        public long Id { get; set; }
        
        [StringLength(60)]
        public string Code { get; set; }

        public bool Filtered { get; set; }

        [StringLength(60)]
        public string FilteredValue { get; set; }

        public bool IsNumericFormat { get; set; }
        
        public long IndicatorBarWidgetId { get; set; }

        [Required]
        [ForeignKey("IndicatorBarWidgetId")]
        public IndicatorBarWidget IndicatorBarWidget { get; set; }
    }
}