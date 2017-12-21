using Common.DataModels.AccountModels;
using Common.DataModels.EventModels;
using Common.Services.EventServices;
using Common.Services.UserServices;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Eredmenyek.ViewModels
{
    public class TeamPageViewModel : ViewModelBase
    {
        private readonly IEventService _eventService;
        private readonly IUserService _userService;

        private List<MatchView> _results;
        public List<MatchView> Results
        {
            get { return _results; }
            set { Set(ref _results, value); }
        }

        private List<MatchView> _fixtures;
        public List<MatchView> Fixtures
        {
            get { return _fixtures; }
            set { Set(ref _fixtures, value); }
        }

        private EventParticipant _team;
        public EventParticipant Team
        {
            get { return _team; }
            set { _team = value; }
        }

        private bool _isFavourite;
        public bool IsFavourite
        {
            get { return _isFavourite; }
            set { Set(ref _isFavourite, value); }
        }
        
        private string _tournamentFK = string.Empty;
        
        public TeamPageViewModel(IEventService eventService, IUserService userService)
        {
            _eventService = eventService;
            _userService = userService;
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            var parameters = parameter as TeamNavigationParameter;
            Team = parameters.Team;
            _tournamentFK = parameters.TournamentFK;
            Views.Busy.SetBusy(true, "Loading Results...");
            var resultEvents = await _eventService.GetParticipantEventResultssAsync(Team.participantFK, _tournamentFK, 5);
            Results = _eventService.CreateMatchViews(resultEvents, false, true);
            Views.Busy.SetBusy(true, "Loading Fixtures...");
            var fixtureEvents = await _eventService.GetParticipantEventFixturesAsync(Team.participantFK, _tournamentFK, 5);
            Fixtures = _eventService.CreateMatchViews(fixtureEvents, true, true);
            var user = await _userService.GetUser();
            IsFavourite = (user.FavouriteTeams != null && user.FavouriteTeams.FavouriteTeams.Where(w => w.Id.Equals(Team.id)).Any());
            
            await base.OnNavigatedToAsync(parameter, mode, state);
            Views.Busy.SetBusy(false);
        }

        public void GotoMatchPage(object sender, ItemClickEventArgs e)
        {
            var parameter = new MatchNavigationParameter();
            parameter.ClickedEvent = (MatchView)e.ClickedItem;
            parameter.TournamentFK = _tournamentFK;
            NavigationService.Navigate(typeof(Views.MatchPage), parameter);
        }

        public async Task OnFavouriteButtonTappedAsync()
        {
            Views.Busy.SetBusy(true, "Saving changes...");
            if (IsFavourite)
            {
                await _userService.RemoveFavouriteTeam(Team.id);
                IsFavourite = false;
            }
            else
            {
                await _userService.AddFavouriteTeam(new FavouriteTeam()
                {
                    Id = Team.id,
                    LeagueId = _tournamentFK,
                    Name = Team.participant.name,
                    ParticipantId = Team.participantFK
                });
                IsFavourite = true;
            }
            Views.Busy.SetBusy(false);
        }
    }
}
