using System.Collections.Generic;

namespace DataMonitoring.ViewModel
{
    public class DashboardViewModel
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public bool TitleDisplayed { get; set; }
        public string TitleColorName { get; set; }
        public bool CurrentTimeManagementDisplayed { get; set; }

        public List<DashboardWidgetViewModel> Widgets { get; set; }
        public List<DashboardLocalizationViewModel> DashboardLocalizations { get; set; }
        public List<SharedDashboardViewModel> SharedDashboards { get; set; }

        public string WidgetTitleListToDisplayed { get; set; }
    }

    public class DashboardLocalizationViewModel
    {
        public long Id { get; set; }
        public string LocalizationCode { get; set; }
        public string Title { get; set; }
    }

    public class DashboardWidgetViewModel
    {
        public long WidgetId { get; set; }
        public int Column { get; set; }
        public int Position { get; set; }
    }

    public class SharedDashboardViewModel
    {
        public long Id { get; set; }
        public string Key { get; set; }
        public string CodeLangue { get; set; }
        public string TimeZone { get; set; }
        public string Skin { get; set; }
        public bool IsTestMode { get; set; }
        public string Message { get; set; }
    }
}
