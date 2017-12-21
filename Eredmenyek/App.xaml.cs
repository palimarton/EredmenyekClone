using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Controls;
using Windows.ApplicationModel.Activation;
using System.Threading.Tasks;
using Template10.Controls;
using Template10.Common;
using Template10.Services.NavigationService;
using Autofac;
using Eredmenyek.ViewModels;
using Eredmenyek.Infrasturcture;
using Eredmenyek.Views;
using Common.Services.DetailsServices;
using Common.Services.StandingServices;
using Common.Services.UserServices;
using Common.Services.EventServices;

namespace Eredmenyek
{
    /// Documentation on APIs used in this page:
    /// https://github.com/Windows-XAML/Template10/wiki

    [Bindable]
    sealed partial class App : BootStrapper
    {
        public App()
        {
            UnhandledException += HandleExceptions;
            InitializeComponent();
            SplashFactory = (e) => new Views.Splash(e);

            #region app settings
            
            AutoSuspendAllFrames = true;
            AutoRestoreAfterTerminated = true;
            AutoExtendExecutionSession = true;

            #endregion
        }

        private void HandleExceptions(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.Exception;
            ExceptionHandler.HandleException(exception.Message);
            e.Handled = true;
            Busy.SetBusy(false);
        }

        public override UIElement CreateRootElement(IActivatedEventArgs e)
        {
            var service = NavigationServiceFactory(BackButton.Attach, ExistingContent.Exclude);
            return new ModalDialog
            {
                DisableBackButtonWhenModal = true,
                Content = new Views.Shell(service),
                ModalContent = new Views.Busy(),
            };
        }

        private IContainer _container;

        private void ConfigureDependencies()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<EventService>().As<IEventService>().InstancePerLifetimeScope();
            builder.RegisterType<StandingsService>().As<IStandingService>().InstancePerLifetimeScope();
            builder.RegisterType<DetailsService>().As<IDetailsService>().InstancePerLifetimeScope();
            builder.RegisterType<OneDriveBasedUserService>().As<IUserService>().InstancePerLifetimeScope();            

            builder.RegisterType<MainPageViewModel>().InstancePerDependency();
            builder.RegisterType<TournamentPageViewModel>().InstancePerDependency();
            builder.RegisterType<MatchPageViewModel>().InstancePerDependency();
            builder.RegisterType<TeamPageViewModel>().InstancePerDependency();            

            _container = builder.Build();
        }

        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            ConfigureDependencies();
            NotificationHelper.BackgroundTasks.NotificationBackgroundTask.RegisterBackgroundTask();
            await NavigationService.NavigateAsync(typeof(MainPage));
        }

        public override INavigable ResolveForPage(Page page, NavigationService navigationService)
        {
            if (page is MainPage)
            {
                return _container.Resolve<MainPageViewModel>();
            }
            else if (page is TournamentPage)
            {
                return _container.Resolve<TournamentPageViewModel>();
            }
            else if (page is MatchPage)
            {
                return _container.Resolve<MatchPageViewModel>();
            }
            else if (page is TeamPage)
            {
                return _container.Resolve<TeamPageViewModel>();
            }
            else
            {
                return base.ResolveForPage(page, navigationService);
            }
        }
    }
}

