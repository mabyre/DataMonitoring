using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMonitoring.Model
{
    public class IndicatorCalculated : IndicatorDefinition
    {
        public long IndicatorDefinitionId1 { get; set; }

        [Required]
        [ForeignKey("IndicatorDefinitionId1")]
        public IndicatorDefinition Indicator1 { get; set; }

        public long IndicatorDefinitionId2 { get; set; }

        [Required]
        [ForeignKey("IndicatorDefinitionId2")]
        public IndicatorDefinition Indicator2 { get; set; }
    }
}
