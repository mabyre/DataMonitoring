using System;
using System.Collections.Generic;

namespace DataMonitoring.ViewModel
{
    public class IndicatorChartWidgetViewModel : IndicatorWidgetViewModel
    {
        public int AxeFontSize { get; set; }
        public string DecimalMask { get; set; }

        // Axe X
        public bool AxeXDisplayed { get; set; }
        public string AxeXColor { get; set; }

        // Axe Y
        public bool AxeYDisplayed { get; set; }
        public bool AxeYDataDisplayed { get; set; }
        public string AxeYColor { get; set; }

        // Target
        public bool ChartTargetDisplayed { get; set; }
        public string ChartTargetColor { get; set; }

        // Average
        public bool ChartAverageDisplayed { get; set; }
        public string ChartAverageColor { get; set; }

        // Chart
        public string ChartDataColor { get; set; }
        public bool ChartDataFill { get; set; }

        // Groups
        public string Group1 { get; set; }
        public string Group2 { get; set; }
        public string Group3 { get; set; }
        public string Group4 { get; set; }
        public string Group5 { get; set; }

        public bool AxeYIsAutoAdjustableAccordingMinValue { get; set; }
        public int? AxeYOffsetFromMinValue { get; set; }

        public List<TargetIndicatorChartWidgetViewModel> TargetIndicatorChartWidgetList { get; set; }
    }

    public class TargetIndicatorChartWidgetViewModel
    {
        public long Id { get; set; }

        public DateTime StartDateUtc { get; set; }
        public decimal StartTargetValue { get; set; }

        public DateTime EndDateUtc { get; set; }
        public decimal EndTargetValue { get; set; }
    }
}
