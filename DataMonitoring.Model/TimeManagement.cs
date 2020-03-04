using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMonitoring.Model
{
    public class TimeManagement
    {
        public long Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        public SlipperyTime SlipperyTime { get; set; }

        public ICollection<TimeRange> TimeRanges { get; set; }
    }

    public class TimeRange
    {
        public long Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [Required]
        public DateTime StartTimeUtc { get; set; }

        public DateTime? EndTimeUtc { get; set; }

        public long TimeManagementId { get; set; }

        [Required]
        [ForeignKey("TimeManagementId")]
        public TimeManagement TimeManagement { get; set; }
    }

    public class SlipperyTime
    {
        public long Id { get; set; }

        [Required]
        public int TimeBack { get; set; }

        [Required]
        public UnitOfTime UnitOfTime { get; set; }

        public long TimeManagementId { get; set; }

        [Required]
        [ForeignKey("TimeManagementId")]
        public TimeManagement TimeManagement { get; set; }
    }

    public enum UnitOfTime
    {
        Hour,
        Day,
        Month,
        Year,
    }
}
