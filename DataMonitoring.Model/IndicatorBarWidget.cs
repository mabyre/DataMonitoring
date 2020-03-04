using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataMonitoring.Model
{
    public class IndicatorBarWidget : IndicatorWidget
    {
        public bool DisplayAxeX { get; set; }

        public bool DisplayAxeY { get; set; }

        public bool DisplayDataAxeY { get; set; }

        [Required]
        [StringLength(50)]
        public string TextDataAxeYColor { get; set; }

        public bool DisplayGridLinesAxeY { get; set; }

        [Required]
        [StringLength(60)]
        public string LabelColumnCode { get; set; }

        [Required]
        [StringLength(50)]
        public string LabelColorText { get; set; }

        public int LabelFontSize { get; set; }
       
        [Required]
        [StringLength(60)]
        public string DataColumnCode { get; set; }

        public bool DisplayDataLabelInBar { get; set; }

        [Required]
        [StringLength(50)]
        public string DataLabelInBarColor { get; set; }

        public int FontSizeDataLabel { get; set; }

        [StringLength(60)]
        public string DecimalMask { get; set; }
        
        public bool AddTargetBar { get; set; }

        [StringLength(50)]
        public string TargetBarColor { get; set; }

        [StringLength(50)]
        public string BarColor { get; set; }

        public bool SetSumDataToTarget { get; set; }

        public bool AddBarStackedToTarget { get; set; }

        [Required]
        [StringLength(60)]
        public string BarColorStackedToTarget { get; set; }
        
        public ICollection<IndicatorBarWidgetColumn> IndicatorBarWidgetColumns { get; set; }

        public ICollection<BarLabelWidget> BarLabelWidgets { get; set; }
    }
}
