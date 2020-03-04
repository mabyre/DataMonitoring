using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMonitoring.Model
{
    public abstract class IndicatorValue
    {
        public long Id { get; set; }

        public long IndicatorDefinitionId { get; set; }

        [Required]
        [ForeignKey("IndicatorDefinitionId")]
        public IndicatorDefinition IndicatorDefinition { get; set; }

        [Required]
        public DateTime DateUtc { get; set; }

        [NotMapped]
        public abstract object Value { get; } 
    }
}
