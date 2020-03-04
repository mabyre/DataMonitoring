using DataMonitoring.Model;

namespace DataMonitoring.ViewModel
{
    public class WidgetContentViewModel
    {
        public string Title { get; set; }

        public int TitleFontSize { get; set; }

        public string LastUpdateToDisplay { get; set; }

        public int RefreshTime { get; set; }

        public WidgetType WidgetType { get; set; }

        public string TitleClassColor { get; set; }

        public string Content { get; set; }
    }
}
