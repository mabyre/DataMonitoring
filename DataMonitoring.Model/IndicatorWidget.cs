using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMonitoring.Model
{
    public class IndicatorWidget
    {
        [Key]
        public long Id { get; set; }

        public long WidgetId { get; set; }

        [DefaultValue(false)]
        public bool TitleIndicatorDisplayed { get; set; }

        [StringLength(50)]
        [DefaultValue("Black")]
        public string TitleIndicatorColor { get; set; }

        [Column(TypeName = "decimal(10,4)")]
        public decimal? TargetValue { get; set; }

        [Required]
        [ForeignKey("WidgetId")]
        public Widget Widget { get; set; }

        public long IndicatorDefinitionId { get; set; }

        [Required]
        [ForeignKey("IndicatorDefinitionId")]
        public IndicatorDefinition IndicatorDefinition { get; set; }
    }
}
