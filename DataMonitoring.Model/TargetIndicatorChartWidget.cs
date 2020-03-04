using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMonitoring.Model
{
    public class TargetIndicatorChartWidget
    {
        public long Id { get; set; }

        public DateTime StartDateUtc { get; set; }

        [Column(TypeName = "decimal(10,4)")]
        public decimal StartTargetValue { get; set; }

        public DateTime EndDateUtc { get; set; }

        [Column(TypeName = "decimal(10,4)")]
        public decimal EndTargetValue { get; set; }

        public long IndicatorChartWidgetId { get; set; }

        [Required]
        [ForeignKey("IndicatorChartWidgetId")]
        public IndicatorChartWidget IndicatorChartWidget { get; set; }

    }
}
