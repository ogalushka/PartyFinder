using Autofac;
using System;
using System.Net;
using System.Net.Http;
using System.Windows;
using WPFClient.Autofac;
using WPFClient.Command;
using WPFClient.Factory;
using WPFClient.GameCatalog.Command;
using WPFClient.GameCatalog.Service;
using WPFClient.GameCatalog.ViewModel;
using WPFClient.GameCatalog.Views;
using WPFClient.Matches;
using WPFClient.Matches.Command;
using WPFClient.Matches.Service;
using WPFClient.Register.Command;
using WPFClient.Register.View;
using WPFClient.Register.ViewModel;
using WPFClient.Service;
using WPFClient.Store;
using WPFClient.TimeEditor.Command;
using WPFClient.TimeEditor.Service;
using WPFClient.TimeEditor.View;
using WPFClient.TimeEditor.ViewModel;
using WPFClient.ViewModel;
using WPFClient.Views;

namespace WPFClient
{
    public partial class App : Application
    {
        // TODO Keyboard navigations
        private IContainer? container;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var view = new MainWindow();
            container = BuildContainer(view);

            var navigation = container.Resolve<Navigation>();
            var preloaderViewModel = container.Resolve<PreloaderViewModel>();
            navigation.CurrentViewModel = preloaderViewModel;
            //navigation.SetViewModel<PreloaderViewModel>();
            //navigation.SetViewModel<HomePageViewModel>();

            var viewModel = container.Resolve<AppViewModel>();
            view.DataContext = viewModel;
            view.Show();

            preloaderViewModel.LoadApp();

        }

        private IContainer BuildContainer(MainWindow view)
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
                return httpClientHandler;
            }).SingleInstance();
            builder.Register((context) =>
            {
                var httpClientHandler = context.Resolve<HttpClientHandler>();
                return new HttpClient(httpClientHandler);
            }).SingleInstance();
            builder.RegisterType<PersistentStorage>().SingleInstance();
            builder.RegisterType<IdentityService>();
            builder.RegisterType<GamesService>();
            builder.RegisterType<PlayerService>();
            builder.RegisterType<TimeService>();
            builder.RegisterType<ProfileStore>().SingleInstance();
            //View model
            builder.RegisterViewWithBinding<HomePageViewModel, HomePage>(view);
            builder.RegisterViewWithBinding<GameCatalogViewModel, GameCatalogView>(view);
            builder.RegisterViewWithBinding<TimeEditorViewModel, TimeEditorView>(view);
            builder.RegisterViewWithBinding<LoginViewModel, LoginView>(view);
            builder.RegisterViewWithBinding<PreloaderViewModel, PreloaderView>(view);
            builder.RegisterViewWithBinding<RegisterViewModel, RegisterView>(view);

            //Command
            builder.RegisterGeneric(typeof(NavigateCommand<>));
            builder.RegisterType<LoginCommand>();
            builder.RegisterType<LogOutCommand>();
            builder.RegisterType<SearchGamesCommand>();
            builder.RegisterType<ToggleGameCommand>();
            builder.RegisterType<DeleteTimeRangeCommand>();
            builder.RegisterType<RegisterCommand>();
            builder.RegisterType<SendMatchRequestCommand>();
            builder.RegisterType<CancelMatchCommand>();
            builder.RegisterType<AcceptMatchCommand>();

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
