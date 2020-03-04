using System.ComponentModel.DataAnnotations.Schema;

namespace DataMonitoring.Model
{
    public class FlowIndicatorValue : IndicatorValue
    {
        public string Group1 { get; set; }
        public string Group2 { get; set; }
        public string Group3 { get; set; }
        public string Group4 { get; set; }
        public string Group5 { get; set; }

        [Column(TypeName = "decimal(10,4)")]
        public decimal? ChartValue { get; set; }

        [NotMapped]
        public override object Value => ChartValue;
    }
}
