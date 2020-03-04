using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMonitoring.Model
{
    public class IndicatorChartWidget : IndicatorWidget
    {
        public bool AxeXDisplayed { get; set; }

        [Required]
        [StringLength(50)]
        public string AxeXColor { get; set; }

        public bool AxeYDisplayed { get; set; }

        public bool AxeYDataDisplayed { get; set; }

        [Required]
        [StringLength(50)]
        public string AxeYColor { get; set; }

        public int AxeFontSize { get; set; }

        [StringLength(60)]
        public string DecimalMask { get; set; }

        public bool ChartTargetDisplayed { get; set; }

        [Required]
        [StringLength(50)]
        public string ChartTargetColor { get; set; }

        public bool ChartAverageDisplayed { get; set; }

        [Required]
        [StringLength(50)]
        public string ChartAverageColor { get; set; }

        [Required]
        [StringLength(50)]
        public string ChartDataColor { get; set; }

        public bool ChartDataFill { get; set; }

        public string Group1 { get; set; }
        public string Group2 { get; set; }
        public string Group3 { get; set; }
        public string Group4 { get; set; }
        public string Group5 { get; set; }

        [DefaultValue(false)]
        public bool AxeYIsAutoAdjustableAccordingMinValue { get; set; }
        public int? AxeYOffsetFromMinValue { get; set; }

        public ICollection<TargetIndicatorChartWidget> TargetIndicatorChartWidgets { get; set; }

        [NotMapped]
        public List<IndicatorChartWidgetColumn> IndicatorChartWidgetColumns
        {
            get
            {
                var list = new List<IndicatorChartWidgetColumn>();
                list.Add(new IndicatorChartWidgetColumn
                {
                    Code = "DateUtc",
                    IsNumericFormat = false,
                    Filtered = false
                });
                list.Add(new IndicatorChartWidgetColumn
                {
                    Code = "Group1",
                    IsNumericFormat = false,
                    FilteredValue = Group1,
                    Filtered = !string.IsNullOrEmpty(Group1)
                });
                list.Add(new IndicatorChartWidgetColumn
                {
                    Code = "Group2",
                    IsNumericFormat = false,
                    FilteredValue = Group2,
                    Filtered = !string.IsNullOrEmpty(Group2)
                });
                list.Add(new IndicatorChartWidgetColumn
                {
                    Code = "Group3",
                    IsNumericFormat = false,
                    FilteredValue = Group3,
                    Filtered = !string.IsNullOrEmpty(Group3)
                });
                list.Add(new IndicatorChartWidgetColumn
                {
                    Code = "Group4",
                    IsNumericFormat = false,
                    FilteredValue = Group4,
                    Filtered = !string.IsNullOrEmpty(Group4)
                });
                list.Add(new IndicatorChartWidgetColumn
                {
                    Code = "Group5",
                    IsNumericFormat = false,
                    FilteredValue = Group5,
                    Filtered = !string.IsNullOrEmpty(Group5)
                });
                list.Add(new IndicatorChartWidgetColumn
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
