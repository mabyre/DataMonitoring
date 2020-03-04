using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMonitoring.Model
{
    public class IndicatorDefinition
    {
        public long Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        public IndicatorType Type { get; set; }

        [Required]
        public int RefreshTime { get; set; }

        [Required]
        [DefaultValue(5)]
        public int DelayForDelete { get; set; }

        public DateTime? LastRefreshUtc { get; set; }

        public ICollection<IndicatorLocalization> IndicatorLocalizations { get; set; }

        public ICollection<IndicatorValue> IndicatorValues { get; set; }

        public ICollection<IndicatorWidget> Widgets { get; set; }

        public ICollection<IndicatorQuery> Queries { get; set; }

        public long? TimeManagementId { get; set; }

        [ForeignKey("TimeManagementId")]
        public TimeManagement TimeManagement { get; set; }
    }

    public class IndicatorLocalization
    {
        public long Id { get; set; }

        [Required]
        [StringLength(10)]
        public string LocalizationCode { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        public long IndicatorDefinitionId { get; set; }

        [Required]
        [ForeignKey("IndicatorDefinitionId")]
        public IndicatorDefinition Indicator { get; set; }
    }
}
