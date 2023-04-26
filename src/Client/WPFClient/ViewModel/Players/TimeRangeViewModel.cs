using WPFClient.Model;
using WPFClient.TimeUtils;

namespace WPFClient.ViewModel.Players
{
    public class TimeRangeViewModel : ViewModelBase
    {
        public string StartTime { get; }
        public string EndTime { get; }
        public string StartDay { get; }
        public string EndDay { get; }

        public TimeRangeViewModel(string startDay, string endDay, string startTime, string endTime)
        {
            StartDay = startDay;
            EndDay = endDay;
            StartTime = startTime;
            EndTime = endTime;
        }

        public TimeRangeViewModel()
        {
            StartDay = "";
            EndDay = "";
            StartTime = "";
            EndTime = "";
        }

        public TimeRangeViewModel(TimeRangeModel timeRange)
            : this(DaysNamer.GetDayName(timeRange.StartTime.Days),
                  DaysNamer.GetDayName(timeRange.EndTime.Days),
                  timeRange.StartTime.ToString(@"hh\:mm"),
                  timeRange.EndTime.ToString(@"hh\:mm"))
        {
        }
    }
}
