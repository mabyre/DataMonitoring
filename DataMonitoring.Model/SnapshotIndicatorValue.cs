using System.ComponentModel.DataAnnotations.Schema;

namespace DataMonitoring.Model
{
    public class SnapshotIndicatorValue : IndicatorValue
    {
        public string TableValue { get; set; }

        [NotMapped]
        public override object Value => TableValue;
    }
}
