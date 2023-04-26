using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using WPFClient.Command;
using WPFClient.Factory;
using WPFClient.Infra;
using WPFClient.Model;
using WPFClient.Store;
using WPFClient.TimeEditor.Command;
using WPFClient.TimeEditor.Service;
using WPFClient.ViewModel;

namespace WPFClient.TimeEditor.ViewModel
{
    public class TimeEditorViewModel : ViewModelBase
    {
        private readonly ObservableCollection<TimeEditorItemViewModel> times;
        private readonly ProfileStore profileStore;
        private readonly TimeService timeService;
        private readonly CommandFactory commandFactory;

        public DelegateCommand AddTimeCommand { get; private init; }
        public DelegateCommand SubmitTimeCommand { get; private init; }

        public TimeEditorViewModel(ProfileStore profileStore, TimeService timeService, CommandFactory commandFactory)
        {
            this.profileStore = profileStore;
            this.timeService = timeService;
            this.commandFactory = commandFactory;
            times = new ObservableCollection<TimeEditorItemViewModel>();

            foreach (var time in profileStore.PlayerModel.TimeRanges)
            {
                times.Add(new TimeEditorItemViewModel(profileStore, time));
            }
            times.Add(new TimeEditorItemViewModel(profileStore));

            AddTimeCommand = new DelegateCommand(AddTimeRange);
            SubmitTimeCommand = new DelegateCommand(SubmitTime, CanSubmitTime);
        }

        private bool CanSubmitTime()
        {
            var editedTime = times.Last();
            return !editedTime.HasErrors && editedTime.State == TimeItemState.Edit;
        }

        public void AddTimeRange()
        {
            var editedTime = times.Last();

            editedTime.ErrorsChanged += HandlerErrorsChanged;
            editedTime.State = TimeItemState.Edit;
        }

        private void HandlerErrorsChanged(object? sender, DataErrorsChangedEventArgs e)
        {
            SubmitTimeCommand.InvokeCanExecuteChanged();
        }

        // TODO loadings etc.
        public async void SubmitTime()
        {
            var editedTime = times.Last();
            editedTime.State = TimeItemState.Saved;
            editedTime.ErrorsChanged -= HandlerErrorsChanged;
            var resulTimeRange = new TimeRangeModel(editedTime.StartDate.Time, editedTime.EndDate.Time);
            await timeService.AddTimeRange(resulTimeRange);
            times.Add(new TimeEditorItemViewModel(profileStore));
        }

        public void RemoveTimeRange(TimeEditorItemViewModel time)
        {
            times.Remove(time);
        }

        public IEnumerable<TimeEditorItemViewModel> Times => times;

        //TODO do delegate command for consistency
        public ICommand DeleteTimeRange => commandFactory.Get<DeleteTimeRangeCommand>(this);
        public ICommand NavigateBack => commandFactory.Get<NavigateCommand<HomePageViewModel>>();
    }
}
