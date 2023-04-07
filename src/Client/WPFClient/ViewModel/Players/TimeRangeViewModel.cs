namespace WPFClient.ViewModel.Players
{
    public class TimeRangeViewModel : ViewModelBase
    {
        public string StartTime { get; }
        public string EndTime { get; }
        public string Day { get; }

        public TimeRangeViewModel(string day, string startTime, string endTime)
        {
            Day = day;
            StartTime = startTime;
            EndTime = endTime;
        }

        public TimeRangeViewModel()
        {
            Day = "";
            StartTime = "";
            EndTime = "";
        }
    }
}
