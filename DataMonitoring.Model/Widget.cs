using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMonitoring.Model
{
    public enum WidgetType
    {
        Table,
        MultiHorizontalTable,
        MultiVerticalTable,
        Chart,
        Bar,
        Gauge,
        Line
    }

    public static class HelperWidgetType
    {
        public static bool IsTable( WidgetType type )
        {
            return type == WidgetType.Table || type == WidgetType.MultiVerticalTable || type == WidgetType.MultiHorizontalTable;
        }
    }

    public class Widget
    {
        public long Id { get; set; }

        [StringLength(200)]
        public string Title { get; set; }

        [DefaultValue(15)]
        public int TitleFontSize { get; set; }

        [Required]
        [StringLength(50)]
        public string TitleColorName { get; set; }

        public bool TitleDisplayed { get; set; }

        [Required]
        public int RefreshTime { get; set; }

        [Required]
        public WidgetType Type { get; set; }

        public ICollection<WidgetLocalization> WidgetLocalizations { get; set; }

        public ICollection<DashboardWidget> Dashboards { get; set; }

        public ICollection<IndicatorWidget> Indicators { get; set; }

        public long? TimeManagementId { get; set; }

        [ForeignKey("TimeManagementId")]
        public TimeManagement TimeManagement { get; set; }


        public bool CurrentTimeManagementDisplayed { get; set; }

        public bool LastRefreshTimeIndicator { get; set; }

        [NotMapped]
        public string TitleToDisplay { get; set; }

        [NotMapped]
        public DateTime? LastUpdateUtc { get; set; }
    }

    public class WidgetLocalization
    {
        public long Id { get; set; }

        [Required]
        [StringLength(10)]
        public string LocalizationCode { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        public long WidgetId { get; set; }

        [Required]
        [ForeignKey("WidgetId")]
        public Widget Widget { get; set; }
    }
}
