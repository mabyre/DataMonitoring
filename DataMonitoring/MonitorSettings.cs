using Sodevlog.Common.Http;
using System.Collections.Generic;

namespace DataMonitoring
{
    public class MonitorSettings : ApplicationSettings
    {
        public int WaitIntervalMonitor { get; set; }
        public int WaitIntervalQueryBackgroundTask { get; set; }

        public string DefaultSkin { get; set; }
        public List<MonitorSkinSetting> Skins { get; set; }
    }

    public class MonitorSkinSetting
    {
        public string Name { get; set; }
        public string Logo { get; set; }
        public string Label { get; set; }
    }
}
