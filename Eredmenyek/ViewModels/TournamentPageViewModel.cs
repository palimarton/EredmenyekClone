using Common.DataModels.Infrasturcture;
using Common.DataModels.EventModels;
using Common.DataModels.StandingModels;
using Common.Services.EventServices;
using Common.Services.StandingServices;
using System.Collections.Generic;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System;

namespace Eredmenyek.ViewModels
{
    public class TournamentPageViewModel : ViewModelBase
    {
        private readonly IEventService _eventService;
        private readonly IStandingService _standingsService;

        private string _tournamentName;
        public string TournamentName
        {
            get { return _tournamentName; }
            set { Set(ref _tournamentName, value); }
        }

        private List<LeagueTableView> _standingsList;
        public List<LeagueTableView> StandingsViewList
        {
            get { return _standingsList; }
            set { Set(ref _standingsList, value); }
        }

        private List<TopScorersView> _topScorers;
        public List<TopScorersView> TopScorers
        {
            get { return _topScorers; }
            set { _topScorers = value; }
        }

        private List<MatchView> _resultList;
        public List<MatchView> ResultList
        {
            get { return _resultList; }
            set { _resultList = value; RaisePropertyChanged(nameof(ResultList)); }
        }

        private bool _hasResult;
        public bool HasResult
        {
            get { return _hasResult; }
            set { _hasResult = value; RaisePropertyChanged(nameof(HasResult)); }
        }

        private List<MatchView> _fixtureList;
        public List<MatchView> FixtureList
        {
            get { return _fixtureList; }
            set { _fixtureList = value; RaisePropertyChanged(nameof(FixtureList)); }
        }

        private bool _hasFixture;
        public bool HasFixture
        {
            get { return _hasFixture; }
            set { _hasFixture = value; RaisePropertyChanged(nameof(HasFixture)); }
        }

        private List<MatchView> _liveMatchList;
        public List<MatchView> LiveMatchList
        {
            get { return _liveMatchList; }
            set { _liveMatchList = value; RaisePropertyChanged(nameof(LiveMatchList)); }
        }

        private bool _hasLiveMatch;
        public bool HasLiveMatch
        {
            get { return _hasLiveMatch; }
            set { _hasLiveMatch = value; RaisePropertyChanged(nameof(HasLiveMatch)); }
        }

        private string _tournamentStage;
        public string TournamentStage
        {
            get { return _tournamentStage; }
            set { _tournamentStage = value; }
        }

        public TournamentPageViewModel(IEventService eventService, IStandingService standingsService)
        {
            _eventService = eventService;
            _standingsService = standingsService;
            TournamentStage = "";
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode,
            IDictionary<string, object> state)
        {          
            int tournament = int.Parse((string)parameter);
            switch (tournament)
            {
                case 1:
                    TournamentName = Constants.TournamentNames.PremierLeague;
                    TournamentStage = Constants.TournamentStageFK.PremierLeague;
                    break;
                case 2:
                    TournamentName = Constants.TournamentNames.Bundesliga;
                    TournamentStage = Constants.TournamentStageFK.Bundesliga;
                    break;
                case 3:
                    TournamentName = Constants.TournamentNames.LaLiga;
                    TournamentStage = Constants.TournamentStageFK.LaLiga;
                    break;
                case 4:
                    TournamentName = Constants.TournamentNames.SerieA;
                    TournamentStage = Constants.TournamentStageFK.SerieA;
                    break;
            }
            Views.Busy.SetBusy(true, "Loading Results...");
            await GetMatchesForDateAsync();
            Views.Busy.SetBusy(true, "Creating League Table...");
            var leagueTableList = await _standingsService.GetLeagueStandingsAsync("tournament_stage", TournamentStage);            
            StandingsViewList = _standingsService.CreateLeagueTable(leagueTableList);
            Views.Busy.SetBusy(true, "Creating Top Scorer's List...");
            var topScorerList = await _standingsService.GetTopScorersAsync("tournament_stage", TournamentStage);
            TopScorers = _standingsService.CreateTopScorersList(topScorerList);
            
            await base.OnNavigatedToAsync(parameter, mode, state);
            Views.Busy.SetBusy(false);
        }

        private async Task GetMatchesForDateAsync(string date = "")
        {
            LiveMatchList = null; HasLiveMatch = false;
            ResultList = null; HasResult = false;
            FixtureList = null; HasFixture = false;

            if (string.IsNullOrWhiteSpace(date))
            {
                string month = DateTime.Now.Month.ToString();
                month = month.Length == 1 ? "0" + month : month;
                string day = DateTime.Now.Day.ToString();
                day = day.Length == 1 ? "0" + day : day;
                date = DateTime.Now.Year + "-" + month + "-" + day;
            }           

            if (DateTime.Today == DateTime.Parse(date))
            {
                EventList liveMatches = await _eventService.GetDailyEventsAsync(TournamentStage, "", date, "", "inprogress");
                HasLiveMatch = liveMatches != null;
                if (HasLiveMatch)
                {
                    LiveMatchList = _eventService.CreateMatchViews(liveMatches, false, false);
                }
                else
                {
                    LiveMatchList = null;
                }
            }

            if (DateTime.Today >= DateTime.Parse(date))
            {
                EventList results = await _eventService.GetResultEventsAsync(TournamentStage, "", date, "");
                HasResult = results != null;
                if (HasResult)
                {
                    ResultList = _eventService.CreateMatchViews(results, false, false);
                }
                else
                {
                    ResultList = null;
                }
            }

            if (DateTime.Today <= DateTime.Parse(date))
            {
                EventList fixtures = await _eventService.GetFixtureEventsAsync(TournamentStage, "", date, "");
                HasFixture = fixtures != null;
                if (HasFixture)
                {
                    FixtureList = _eventService.CreateMatchViews(fixtures, true, false);
                }
                else
                {
                    FixtureList = null;
                }
            }
        }

        public async void MatchDateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            Views.Busy.SetBusy(true);
            string month = e.NewDate.Month.ToString();
            month = month.Length == 1 ? "0" + month : month;
            string day = e.NewDate.Day.ToString();
            day = day.Length == 1 ? "0" + day : day;
            string date = e.NewDate.Year + "-" + month + "-" + day;
            await GetMatchesForDateAsync(date);
            Views.Busy.SetBusy(false);
        }

        public void GotoMatchPage(object sender, ItemClickEventArgs e)
        {
            var parameter = new MatchNavigationParameter();
            parameter.ClickedEvent = (MatchView)e.ClickedItem;
            parameter.TournamentFK = TournamentStage;
            NavigationService.Navigate(typeof(Views.MatchPage), parameter);
        }

        public void OnLeagueTableItemClicked(object sender, ItemClickEventArgs e)
        {
            var leagueTableItem = e.ClickedItem as LeagueTableView;
            var parameter = new TeamNavigationParameter()
            {
                Team = new EventParticipant()
                {
                    participant = new Participant()
                    {
                        name = leagueTableItem.TeamName
                    },
                    participantFK = leagueTableItem.TeamParticipantFK,
                    id = leagueTableItem.TeamId
                },
                TournamentFK = TournamentStage
            };
            NavigationService.Navigate(typeof(Views.TeamPage), parameter);
        }
    }

    public class MatchNavigationParameter
    {
        public MatchView ClickedEvent { get; set; }
        public string TournamentFK { get; set; }
    }
}
