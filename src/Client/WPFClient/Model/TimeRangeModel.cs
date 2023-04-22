using System;

namespace WPFClient.Model
{
    public class TimeRangeModel
    {
        public readonly TimeSpan StartTime;
        public readonly TimeSpan EndTime;

        public TimeRangeModel(int startTimeMinutes, int endTimeMinutes)
        {
            StartTime = TimeSpan.FromMinutes(startTimeMinutes);
            EndTime = TimeSpan.FromMinutes(endTimeMinutes);
        }

        public TimeRangeModel(TimeSpan startTime, TimeSpan endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
        }

        public override bool Equals(object? other)
        {
            return other != null
                && other is TimeRangeModel castOther
                && castOther.StartTime == StartTime
                && castOther.EndTime == EndTime;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(StartTime, EndTime);
        }
    }
}
