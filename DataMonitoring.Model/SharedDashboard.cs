using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMonitoring.Model
{
    public class SharedDashboard
    {
        public long Id { get; set; }

        public string Key { get; set; }

        public string CodeLangue { get; set; }

        public string TimeZone { get; set; }

        public string Skin { get; set; }

        public bool IsTestMode { get; set; }

        public string SecurityStamp { get; set; }

        public string Message { get; set; }

        public long DashboardId { get; set; }

        [Required]
        [ForeignKey("DashboardId")]
        public Dashboard Dashboard { get; set; }
    }
}
