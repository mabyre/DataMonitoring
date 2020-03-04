using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMonitoring.Model
{
    public class BarLabelWidget
    {
        public long Id { get; set; }

        [Required]
        [StringLength(60)]
        public string Name { get; set; }

        public int Sequence { get; set; }

        [StringLength(50)]
        public string LabelTextColor { get; set; }

        public bool UseLabelColorForBar { get; set; }
        
        public long IndicatorBarWidgetId { get; set; }

        [Required]
        [ForeignKey("IndicatorBarWidgetId")]
        public IndicatorBarWidget IndicatorBarWidget { get; set; }

        public ICollection<BarLabelWidgetLocalization> BarLabelWidgetLocalizations { get; set; }
    }

    public class BarLabelWidgetLocalization
    {
        public long Id { get; set; }

        [Required]
        [StringLength(10)]
        public string LocalizationCode { get; set; }

        [Required]
        [StringLength(60)]
        public string Name { get; set; }

        public long BarLabelWidgetId { get; set; }

        [Required]
        [ForeignKey("BarLabelWidgetId")]
        public BarLabelWidget BarLabelWidget { get; set; }

    }
}
