using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMonitoring.Model
{
    public class IndicatorQuery
    {
        public long Id { get; set; }

        public string Query { get; set; }

        public long ConnectorId { get; set; }

        [Required]
        [ForeignKey("ConnectorId")]
        public Connector Connector { get; set; }

        public long IndicatorDefinitionId { get; set; }

        [Required]
        [ForeignKey("IndicatorDefinitionId")]
        public IndicatorDefinition IndicatorDefinition { get; set; }
    }
}
