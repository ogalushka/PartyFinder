using Autofac;
using System.Net;
using System.Net.Http;
using System.Windows;
using WPFClient.Command;
using WPFClient.Factory;
using WPFClient.GameCatalog.Command;
using WPFClient.GameCatalog.Service;
using WPFClient.GameCatalog.ViewModel;
using WPFClient.Service;
using WPFClient.Store;
using WPFClient.ViewModel;

namespace WPFClient
{
    public partial class App : Application
    {
        private IContainer? container;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            container = BuildContainer();

            var navigation = container.Resolve<Navigation>();
             navigation.SetViewModel<LoginViewModel>();
            //navigation.SetViewModel<HomePageViewModel>();

            var viewModel = container.Resolve<AppViewModel>();
            var view = new MainWindow();
            view.DataContext = viewModel;
            view.Show();
        }

        private IContainer BuildContainer()
        {
            //TODO consider adding separate scope for authenticated part of the app
            var builder = new ContainerBuilder();
            builder.RegisterType<CommandFactory>();
            builder.RegisterType<Navigation>().SingleInstance();
            builder.RegisterType<AppViewModel>();
            builder.Register((context) =>
            {
                var httpClientHandler = new HttpClientHandler();
                httpClientHandler.CookieContainer = new CookieContainer();
                return new HttpClient(httpClientHandler);
            }).SingleInstance();
            builder.RegisterType<IdentityService>();
            builder.RegisterType<GamesService>();
            //View model
            builder.RegisterType<LoginViewModel>();
            builder.RegisterType<HomePageViewModel>();
            builder.RegisterType<GameCatalogViewModel>();
            //Command
            builder.RegisterType<LoginCommand>();
            builder.RegisterType<LogOutCommand>();
            builder.RegisterType<SearchGamesCommand>();

            builder.RegisterType<SessionStore>();
            return builder.Build();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            container?.Dispose();
            base.OnExit(e);
        }
    }
}
