using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WPFClient
{
    public abstract class AsyncCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        private bool isExecuting = false;

        public bool CanExecute(object? parameter)
        {
            return !IsExecuting;
        }

        private bool IsExecuting {
            get { return isExecuting; }
            set {
                isExecuting = value;
                CanExecuteChanged?.Invoke(this, new EventArgs());
            }
        }

        public async void Execute(object? parameter)
        {
            try
            {
                IsExecuting = true;
                await ExecuteAsync(parameter);
            }
            finally
            {
                IsExecuting = false;
            }
        }

        protected abstract Task ExecuteAsync(object? parameter);
    }
}
