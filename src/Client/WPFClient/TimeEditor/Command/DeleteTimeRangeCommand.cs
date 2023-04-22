using System.Threading.Tasks;
using WPFClient.Model;
using WPFClient.TimeEditor.Service;
using WPFClient.TimeEditor.ViewModel;

namespace WPFClient.TimeEditor.Command
{
    public class DeleteTimeRangeCommand : AsyncCommand
    {
        private readonly TimeEditorViewModel viewModel;
        private readonly TimeService timeService;

        public DeleteTimeRangeCommand(TimeEditorViewModel viewModel, TimeService timeService)
        {
            this.viewModel = viewModel;
            this.timeService = timeService;
        }

        protected override async Task ExecuteAsync(object? parameters)
        {
            if (parameters is TimeEditorItemViewModel itemViewModel)
            {
                var startTime = (int)itemViewModel.StartDate.Time.TotalMinutes;
                var endTime = (int)itemViewModel.EndDate.Time.TotalMinutes;
                var timeRange = new TimeRangeModel(startTime, endTime);
                await timeService.DeleteTimeRange(timeRange);
                viewModel.RemoveTimeRange(itemViewModel);
            }
        }
    }
}
