namespace WPFClient.Model
{
    public class TimeRangeModel
    {
        public readonly int StartTime;
        public readonly int EndTime;

        public TimeRangeModel(int startTime, int endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}
