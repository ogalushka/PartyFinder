using System;

namespace WPFClient.TimeEditor.ViewModel
{
    public class TimeSelectionViewModel : ViewModelBase
    {
        private readonly string[] days;

        public TimeSelectionViewModel(TimeSpan time)
        {
            Time = time;
            days = new[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
        }

        private void SetHour(string value)
        {
            if (int.TryParse(value, out int hours))
            {
                hours = Math.Min(23, hours);
                hours = Math.Max(0, hours);
                Time = new TimeSpan(Time.Days, hours, Time.Minutes, 0);
            }
            else
            {
                Time = new TimeSpan(Time.Days, 0, Time.Minutes, 0);
            }
        }

        private void SetMinutes(string value)
        {
            if (int.TryParse(value, out int minutes))
            {
                minutes = Math.Min(59, minutes);
                minutes = Math.Max(0, minutes);
                Time = new TimeSpan(Time.Days, Time.Hours, minutes, 0);
            }
            else
            {
                Time = new TimeSpan(Time.Days, Time.Hours, 0, 0);
            }
        }

        public TimeSpan Time { get; private set; }

        private bool editable = false;
        public bool Editable {
            get { return editable; }
            set { SetField(ref editable, value); }
        }

        public string[] DaysList => days;


        // TODO it might be more appropriate to do validation elsewhere, check Behaviours and Validators
        // TODO find a way to switch trigger to propperty changed
        public string Hours {
            get { return Time.Hours.ToString(); }
            set {
                SetHour(value);
                InvokeProertyChange();
            }
        }

        public string Minutes {
            get { return Time.Minutes.ToString().PadLeft(2, '0'); }
            set {
                SetMinutes(value);
                InvokeProertyChange();
            }
        }

        public string Day {
            get { return days[Time.Days % days.Length]; } 
            set {
                var dayIndex = Array.IndexOf(days, value);
                if (dayIndex < 0)
                {
                    return;
                }

                Time = new TimeSpan(dayIndex, Time.Hours, Time.Minutes, 0);
                InvokeProertyChange();
            } 
        }
    }
}
