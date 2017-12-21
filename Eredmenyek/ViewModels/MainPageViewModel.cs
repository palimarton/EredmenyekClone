using Template10.Mvvm;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using Common.Services.EventServices;
using Common.Services.UserServices;
using Common.DataModels.AccountModels;
using Windows.UI.Xaml.Controls;
using static Eredmenyek.Views.Shell;
using Template10.Services.NavigationService;
using Eredmenyek.Views;

namespace Eredmenyek.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private readonly IEventService _eventService;
        private readonly IUserService _userService;

        private List<FavouriteTeam> _teamNames;
        public List<FavouriteTeam> TeamNames
        {
            get { return _teamNames; }
            set { Set( ref _teamNames, value); }
        }

        private bool _hasFavouriteTeam = false;
        public bool HasFavouriteTeam
        {
            get { return _hasFavouriteTeam; }
            set { Set(ref _hasFavouriteTeam, value); }
        }

        private bool _loggedIn = false;
        public bool LoggedIn
        {
            get { return _loggedIn; }
            set
            {
                Set(ref _loggedIn, value);
                RaisePropertyChanged(nameof(LoggedIn));
            }
        }

        public MainPageViewModel(IEventService eventService, IUserService userService)
        {
            _eventService = eventService;
            _userService = userService;
        }

        public void GotoSettings() => NavigationService.Navigate(typeof(SettingsPage), 0);

        public void GotoAbout() => NavigationService.Navigate(typeof(SettingsPage), 2);

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            Busy.SetBusy(true, "Loading favourite teams");
            User currentUser = null;
            try
            {
                currentUser = await _userService.GetUser();
                LoggedIn = true;
            }
            catch (Microsoft.Graph.ServiceException)
            {
                LoggedIn = false;
            }
            if (currentUser != null)
            {
                TeamNames = new List<FavouriteTeam>();
                if (currentUser.FavouriteTeams != null)
                {
                    foreach (var team in currentUser.FavouriteTeams.FavouriteTeams)
                    {
                        TeamNames.Add(team);
                    }
                }
                HasFavouriteTeam = TeamNames.Count > 0;
            }
            OnLogInFinished += RefreshPageAsync;
            await base.OnNavigatedToAsync(parameter, mode, state);
            Busy.SetBusy(false);
        }

        public override Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            OnLogInFinished -= RefreshPageAsync;
            return base.OnNavigatingFromAsync(args);
        }

        public void FavouriteTeamTapped(object sender, ItemClickEventArgs e)
        {
            var team = e.ClickedItem as FavouriteTeam;
            var parameter = new TeamNavigationParameter()
            {
                Team = new Common.DataModels.EventModels.EventParticipant()
                {
                    participant = new Common.DataModels.StandingModels.Participant()
                    {
                        name = team.Name
                    },
                    participantFK = team.ParticipantId,
                    id = team.Id
                },
                TournamentFK = team.LeagueId
            };
            NavigationService.Navigate(typeof(Views.TeamPage), parameter);
        }

        public async void RefreshPageAsync()
        {
            LoggedIn = true;
            var currentUser = await _userService.GetUser();
            TeamNames = currentUser.FavouriteTeams.FavouriteTeams;
            HasFavouriteTeam = TeamNames.Count > 0;
            NavigationService.Refresh();
        }
    }
}

