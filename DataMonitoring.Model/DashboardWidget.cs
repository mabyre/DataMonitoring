using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMonitoring.Model
{
    public class DashboardWidget
    {
        [Key]
        public long DashboardId { get; set; }

        [Required]
        [ForeignKey("DashboardId")]
        public Dashboard Dashboard { get; set; }

        [Key]
        public long WidgetId { get; set; }

        [Required]
        [ForeignKey("WidgetId")]
        public Widget Widget { get; set; }

        public int Column { get; set; }

        public int Position { get; set; }
    }
}
