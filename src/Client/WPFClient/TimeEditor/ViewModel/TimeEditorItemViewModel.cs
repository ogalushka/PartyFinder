using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using WPFClient.Model;
using WPFClient.Store;

namespace WPFClient.TimeEditor.ViewModel
{
    // TODO add custom component for time range selection
    // Idea: Its a line of the whole week where you edit time ranges by dragging
    public class TimeEditorItemViewModel : ViewModelBase, INotifyDataErrorInfo
    {
        private const int MinWindowTimeMinutes = 30;
        private readonly ProfileStore profileStore;

        public TimeEditorItemViewModel(ProfileStore profileStore, TimeRangeModel timeRange) :
            this(profileStore, timeRange.StartTime, timeRange.EndTime, TimeItemState.Saved)
        {
        }

        public TimeEditorItemViewModel(ProfileStore profileStore) :
            this(profileStore, TimeSpan.Zero, TimeSpan.Zero, TimeItemState.Empty)
        {
        }

        private TimeEditorItemViewModel(ProfileStore profileStore, TimeSpan startTime, TimeSpan endTime, TimeItemState state)
        {
            StartDate = new TimeSelectionViewModel(startTime);
            EndDate = new TimeSelectionViewModel(endTime);
            this.profileStore = profileStore;
            State = state;
            
            StartDate.PropertyChanged += HandleDateChange; 
            EndDate.PropertyChanged += HandleDateChange;

            errors = new();
        }

        private TimeItemState state = TimeItemState.Empty;

        public TimeItemState State {
            get { return state; }
            set { 
                SetField(ref state, value);

                var editable = state == TimeItemState.Edit;
                StartDate.Editable = editable;
                EndDate.Editable = editable;
            }
        }

        public TimeSelectionViewModel StartDate { get; private set; }
        public TimeSelectionViewModel EndDate { get; private set; }



        private Dictionary<string, List<string>> errors;

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        private void HandleDateChange(object? sender, PropertyChangedEventArgs e)
        {
            RemoveErrors(nameof(StartDate));
            RemoveErrors(nameof(EndDate));
            if (!IsTimeRangeLargerThanMin())
            {
                AddError(nameof(StartDate), $"Time window has to be larger than {MinWindowTimeMinutes} minutes");
                AddError(nameof(EndDate), $"Time window has to be larger than {MinWindowTimeMinutes} minutes");
            }

            if (IsIntersectingWithOtherRanges())
            {
                AddError(nameof(StartDate), $"Range overlaps with other ranges");
                AddError(nameof(EndDate), $"Range overlaps with other ranges");
            }

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(StartDate)));
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(EndDate)));
        }

        private void AddError(string field, string error)
        {
            if (!errors.TryGetValue(field, out var errorList))
            {
                errorList = new List<string>();
                errors.Add(field, errorList);
            }

            if (!errorList.Contains(error))
            {
                errorList.Add(error);
            }
        }

        private void RemoveErrors(string field)
        {
            if (errors.ContainsKey(field))
            {
                errors.Remove(field);
            }
        }

        private bool IsTimeRangeLargerThanMin()
        {
            var startTime = StartDate.Time;
            var endTime = EndDate.Time;
            var dt = endTime - startTime;
            return dt >= TimeSpan.FromMinutes(MinWindowTimeMinutes);
        }

        private bool IsIntersectingWithOtherRanges()
        {
            var startTime = StartDate.Time;
            var endTime = EndDate.Time;

            foreach (var savedTime in profileStore.PlayerModel.TimeRanges)
            {
                if (savedTime.StartTime < endTime && savedTime.EndTime > startTime)
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasErrors => errors.Count > 0;
        public IEnumerable GetErrors(string? propertyName)
        {
            if (propertyName != null && errors.TryGetValue(propertyName, out var errorsList))
            {
                return errorsList;
            }

            return Array.Empty<string>();
        }

    }

    public enum TimeItemState
    {
        Empty, 
        Edit, 
        Saved
    }

}
