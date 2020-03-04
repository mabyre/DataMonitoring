using System.Collections.Generic;

namespace DataMonitoring.ViewModel
{
    public class DashboardLightViewModel
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public bool TitleDisplayed { get; set; }
        public string TitleColorName { get; set; }
        public bool CurrentTimeManagementDisplayed { get; set; }
        public List<DashboardLocalizationViewModel> DashboardLocalizations { get; set; }

    }
}
