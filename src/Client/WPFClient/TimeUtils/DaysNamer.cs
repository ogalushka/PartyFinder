namespace WPFClient.TimeUtils
{
    public class DaysNamer
    {
        public static string[] Days = new[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };

        public static string GetDayName(int dayIndex)
        {
            return Days[dayIndex % Days.Length];
        }
    }
}
