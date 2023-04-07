using Autofac;
using System;
using System.ComponentModel;

namespace WPFClient.Store
{
    public class Navigation
    {
        private INotifyPropertyChanged? currentViewModel;
        private readonly ILifetimeScope container;

        public Navigation(ILifetimeScope container)
        {
            this.container = container;
        }

        public event Action ViewModelChanged = () => { };

        public INotifyPropertyChanged CurrentViewModel {
            //TODO saner way to be not nullable?
            get { return currentViewModel ?? throw new Exception(); }
            private set {
                currentViewModel = value;
                ViewModelChanged();
            }
        }

        public void SetViewModel<TViewModel>() where TViewModel : ViewModelBase
        {
            CurrentViewModel = container.Resolve<TViewModel>();
        }
    }
}
