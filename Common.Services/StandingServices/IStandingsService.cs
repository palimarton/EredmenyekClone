using Common.DataModels.StandingModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Services.StandingServices
{
    public interface IStandingService
    {
        Task<StandingsList> GetLeagueStandingsAsync(string _object, string objectFK);

        Task<StandingsList> GetTopScorersAsync(string _object, string objectFK);

        Task<StandingsList> GetEventStatsAsync(string _object, string objectFK, string id, string includeStandingParticipant, string includeStandingData);

        List<LeagueTableView> CreateLeagueTable(StandingsList list);

        List<TopScorersView> CreateTopScorersList(StandingsList list);

        List<EventStatisticsView> CreateEventStatisticsList(StandingsList list);
    }
}
