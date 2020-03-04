using System;
using System.Collections.Generic;
using DataMonitoring.Model;

namespace DataMonitoring.ViewModel
{
    public class TimeManagementViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public SlipperyTimeViewModel SlipperyTime { get; set; }
        public List<TimeRangeViewModel> TimeRanges { get; set; }
    }

    public class SlipperyTimeViewModel
    {
        public int TimeBack { get; set; }
        public UnitOfTime UnitOfTime { get; set; }
    }

    public class TimeRangeViewModel 
    {
        public string Name { get; set; }
        public DateTime StartTimeUtc { get; set; }
        public DateTime? EndTimeUtc { get; set; }
    }

    public enum TimeManagementType
    {
        SlipperyTime,
        TimeRange,
    }
}
