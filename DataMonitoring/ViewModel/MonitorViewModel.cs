using System.Collections.Generic;

namespace DataMonitoring.ViewModel
{
    public class MonitorViewModel
    {
        public string Key { get; set; }

        public string Title { get; set; }

        public bool DisplayTitle { get; set; }

        public string ClassColorTitle { get; set; }

        public string CodeLangue { get; set; }

        public string TimeZone { get; set; }

        public bool IsTestMode { get; set; }

        public string Message { get; set; }

        public string Skin { get; set; }

        public string Version { get; set; }

        public List<MonitorWidgetViewModel> Widgets { get; set; }
     }

    public class MonitorWidgetViewModel
    {
        public long Id { get; set; }

        public int Column { get; set; }

        public int Position { get; set; }
    }
}
