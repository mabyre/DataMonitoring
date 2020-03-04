using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMonitoring.Model
{
    public class IndicatorGaugeWidget : IndicatorWidget
    {
        public bool TargetDisplayed { get; set; }
        
        public string Group1 { get; set; }
        public string Group2 { get; set; }
        public string Group3 { get; set; }
        public string Group4 { get; set; }
        public string Group5 { get; set; }

        [StringLength(50)]
        public string GaugeTargetColor { get; set; }

        [StringLength(50)]
        public string GaugeRange1Color { get; set; }
        public decimal? GaugeRange1MinValue { get; set; }
        public decimal? GaugeRange1MaxValue { get; set; }

        public bool? GaugeRange2Displayed { get; set; }
        [StringLength(50)]
        public string GaugeRange2Color { get; set; }
        public decimal? GaugeRange2MinValue { get; set; }
        public decimal? GaugeRange2MaxValue { get; set; }

        public bool? GaugeRange3Displayed { get; set; }
        [StringLength(50)]
        public string GaugeRange3Color { get; set; }
        public decimal? GaugeRange3MinValue { get; set; }
        public decimal? GaugeRange3MaxValue { get; set; }

        [NotMapped]
        public List<IndicatorGaugeWidgetColumn> IndicatorGaugeWidgetColumns
        {
            get
            {
                var list = new List<IndicatorGaugeWidgetColumn>();
                list.Add(new IndicatorGaugeWidgetColumn
                {
                    Code = "Group1",
                    IsNumericFormat = false,
                    FilteredValue = Group1,
                    Filtered = !string.IsNullOrEmpty(Group1)
                });
                list.Add(new IndicatorGaugeWidgetColumn
                {
                    Code = "Group2",
                    IsNumericFormat = false,
                    FilteredValue = Group2,
                    Filtered = !string.IsNullOrEmpty(Group2)
                });
                list.Add(new IndicatorGaugeWidgetColumn
                {
                    Code = "Group3",
                    IsNumericFormat = false,
                    FilteredValue = Group3,
                    Filtered = !string.IsNullOrEmpty(Group3)
                });
                list.Add(new IndicatorGaugeWidgetColumn
                {
                    Code = "Group4",
                    IsNumericFormat = false,
                    FilteredValue = Group4,
                    Filtered = !string.IsNullOrEmpty(Group4)
                });
                list.Add(new IndicatorGaugeWidgetColumn
                {
                    Code = "Group5",
                    IsNumericFormat = false,
                    FilteredValue = Group5,
                    Filtered = !string.IsNullOrEmpty(Group5)
                });
                list.Add(new IndicatorGaugeWidgetColumn
                {
                    Code = "Value",
                    IsNumericFormat = true,
                    Filtered = false
                });

                return list;
            }
        }
    }
}
