using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMonitoring.Model
{
    public class Dashboard
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public bool TitleDisplayed { get; set; }

        [Required]
        [StringLength(50)]
        public string TitleColorName { get; set; }

        public bool CurrentTimeManagementDisplayed { get; set; }

        public ICollection<DashboardLocalization> DashboardLocalizations { get; set; }

        public ICollection<DashboardWidget> Widgets { get; set; }

        public ICollection<SharedDashboard> SharedDashboards { get; set; }

        public string Version { get; set; }

        [NotMapped]
        public string TitleToDisplay { get; set; }
    }
    
    public class DashboardLocalization
    {
        public long Id { get; set; }

        [Required]
        [StringLength(10)]
        public string LocalizationCode { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        public long DashboardId { get; set; }

        [Required]
        [ForeignKey("DashboardId")]
        public Dashboard Dashboard { get; set; }
    }
}
