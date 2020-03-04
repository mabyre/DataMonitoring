using System.Collections.Generic;
using DataMonitoring.Model;

namespace DataMonitoring.ViewModel
{
    public class WidgetViewModel
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public int TitleFontSize { get; set; }
        public List<WidgetTitleTranslation> TitleTranslate { get; set; }
        public string TitleColorName { get; set; }
        public int RefreshTime { get; set; }
        public WidgetType Type { get; set; }
        public bool TitleDisplayed { get; set; }
        public bool CurrentTimeManagementDisplayed { get; set; }
        public bool LastRefreshTimeIndicatorDisplayed { get; set; }
        public long? TimeManagementId { get; set; }
        public string IndicatorTitleListToDisplayed { get; set; }

        // Listes des Types
        public List<IndicatorTableWidgetViewModel> IndicatorTableWidgetList { get; set; }
        public IndicatorBarWidgetViewModel IndicatorBarWidget { get; set; }
        public IndicatorChartWidgetViewModel IndicatorChartWidget { get; set; }
        public IndicatorGaugeWidgetViewModel IndicatorGaugeWidget { get; set; }
    }

    public class WidgetTitleTranslation
    {
        public long Id { get; set; }
        public string LocalizationCode { get; set; }
        public string Title { get; set; }
    }

    public abstract class IndicatorWidgetViewModel
    {
        public long Id { get; set; }
        public long IndicatorId { get; set; }
        public decimal? TargetValue { get; set; }

        // Title Indicator
        public bool TitleIndicatorDisplayed { get; set; }
        public string TitleIndicatorColor { get; set; }
    }
}
