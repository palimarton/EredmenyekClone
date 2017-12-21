using Common.DataModels.DetailsModels;
using Common.DataModels.EventModels;
using Common.DataModels.StandingModels;
using Common.Services.DetailsServices;
using Common.Services.StandingServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace Eredmenyek.ViewModels
{
    public class MatchPageViewModel : ViewModelBase
    {
        #region Fields

        private MatchView _match;
        public MatchView Match
        {
            get { return _match; }
            set { _match = value; }
        }

        private DetailsList _matchDetails;
        public DetailsList MatchDetails
        {
            get { return _matchDetails; }
            set { _matchDetails = value; }
        }

        private List<Lineup> _homeTeamLineup;
        public List<Lineup> HomeTeamLineup
        {
            get { return _homeTeamLineup; }
            set { _homeTeamLineup = value; }
        }

        private List<Lineup> _guestTeamLineup;
        public List<Lineup> GuestTeamLineup
        {
            get { return _guestTeamLineup; }
            set { _guestTeamLineup = value; }
        }

        private List<IncidentViewModel> _homeTeamIncidentViewList;
        public List<IncidentViewModel> HomeTeamIncidentViewList
        {
            get { return _homeTeamIncidentViewList; }
            set { Set(ref _homeTeamIncidentViewList, value); }
        }

        private List<IncidentViewModel> _guestTeamIncidentViewList;
        public List<IncidentViewModel> GuestTeamIncidentViewList
        {
            get { return _guestTeamIncidentViewList; }
            set { Set(ref _guestTeamIncidentViewList, value); }
        }

        private List<IncidentViewModel> _matchIncidents;
        public List<IncidentViewModel> MatchIncidents
        {
            get { return _matchIncidents; }
            set { Set(ref _matchIncidents, value); }
        }

        private List<EventStatisticsView> _eventStats;
        public List<EventStatisticsView> EventStats
        {
            get { return _eventStats; }
            set { _eventStats = value; }
        }
        
        private string _tournamentFK = string.Empty;

        private IDetailsService _detailsService;
        private IStandingService _standingsService;

        #endregion

        public MatchPageViewModel(IDetailsService detailsService, IStandingService standingsService)
        {
            _detailsService = detailsService;
            _standingsService = standingsService;
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            var parameters = (MatchNavigationParameter)parameter;
            Match = parameters.ClickedEvent;
            _tournamentFK = parameters.TournamentFK;
            Views.Busy.SetBusy(true, "Loading Match Events...");
            MatchDetails = await _detailsService.GetDetailsAsync(Match.EventId, "yes", "yes", "yes", "yes", "");            
            if (!MatchDetails.@event.Values.ElementAt(0).status_type.Equals("notstarted"))
            {
                HomeTeamLineup 
                    = MatchDetails.@event.Values.ElementAt(0).event_participants.Values.ElementAt(0).lineup.Values.OrderBy(ep => int.Parse(ep.shirt_number)).ToList();
                GuestTeamLineup
                    = MatchDetails.@event.Values.ElementAt(0).event_participants.Values.ElementAt(1).lineup.Values.OrderBy(ep => int.Parse(ep.shirt_number)).ToList();

                Common.DataModels.DetailsModels.EventParticipant homeTeam 
                    = MatchDetails.@event.Values.ElementAt(0).event_participants.Values.ElementAt(0);
                Common.DataModels.DetailsModels.EventParticipant awayTeam 
                    = MatchDetails.@event.Values.ElementAt(0).event_participants.Values.ElementAt(1);

                MatchIncidents = _detailsService.CreateIncidentViewModels(homeTeam, awayTeam);

                Views.Busy.SetBusy(true, "Loading Statistics...");
                var eventStats = await _standingsService.GetEventStatsAsync("event", Match.EventId, "", "yes", "yes");
                if (eventStats != null)
                {
                    EventStats = _standingsService.CreateEventStatisticsList(eventStats);
                }
            }
            else
            {
                HomeTeamLineup = new List<Lineup>();
                GuestTeamLineup = new List<Lineup>();
            }                   

            await base.OnNavigatedToAsync(parameter, mode, state);
            Views.Busy.SetBusy(false);
        }

        public void Team_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var textBlock = sender as TextBlock;
            var parameter = new TeamNavigationParameter();
            parameter.Team = textBlock.Text.Equals(Match.HomeTeam.participant.name)
                ? Match.HomeTeam
                : Match.GuestTeam;
            parameter.TournamentFK = _tournamentFK;
            NavigationService.Navigate(typeof(Views.TeamPage), parameter);
        }
    }

    public class TeamNavigationParameter
    {
        public Common.DataModels.EventModels.EventParticipant Team { get; set; }
        public string TournamentFK { get; set; }
    }
}
