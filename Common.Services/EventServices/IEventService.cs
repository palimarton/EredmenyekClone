using Common.DataModels.EventModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Services.EventServices
{
    public interface IEventService
    {
        Task<EventList> GetDailyEventsAsync(string tournament_stage, string sport, string date, string live, string status_type);

        Task<EventList> GetFixtureEventsAsync(string tournament_stage, string sport, string date, string live);

        Task<EventList> GetResultEventsAsync(string tournament_stage, string sport, string date, string live);

        List<MatchView> CreateMatchViews(EventList list, bool isFixture, bool detailedDate);

        Task<EventList> GetParticipantEventFixturesAsync(string participant, string tournament_stage, int? limit);

        Task<EventList> GetParticipantEventResultssAsync(string participant, string tournament_stage, int? limit);
    }
}
