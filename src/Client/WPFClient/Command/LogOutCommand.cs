using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Windows.Input;
using WPFClient.Store;
using WPFClient.ViewModel;

namespace WPFClient.Command
{
    public class LogOutCommand : ICommand
    {
        private readonly HttpClientHandler httpClientHandler;
        private readonly Navigation navigationStore;
        private readonly PersistentStorage persistentStorage;

        public event EventHandler? CanExecuteChanged;

        public LogOutCommand(HttpClientHandler httpClientHandler, Navigation navigationStore, PersistentStorage persistentStorage)
        {
            this.httpClientHandler = httpClientHandler;
            this.navigationStore = navigationStore;
            this.persistentStorage = persistentStorage;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            var cookies = httpClientHandler.CookieContainer.GetAllCookies().Cast<Cookie>();
            foreach (Cookie cookie in cookies)
            {
                cookie.Discard = true;
                cookie.Expired = true;
            }
            persistentStorage.ClearCookies();
            navigationStore.SetViewModel<LoginViewModel>();
        }
    }
}
