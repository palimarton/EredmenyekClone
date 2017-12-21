using Common.DataModels.EventModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Common.Services.EventServices
{
    public class EventService : IEventService
    {
        private const string _eventServerUrl = "http://demo.eapi.enetpulse.com/event/";
        // Your Enetpulse username
        private const string _username = "";
        // Your Enetpulse token
        private const string _token = "";

        public async Task<EventList> GetDailyEventsAsync(string tournament_stage, string sport, string date, string live, string status_type)
        {
            string dailyUrl = $"{_eventServerUrl}daily/?username={_username}&token={_token}";
            string finalUrl = GenerateFinalString(dailyUrl, tournament_stage, sport, date, live, status_type);
            return await GetRequestAsync<EventList>(new Uri(finalUrl));
        }

        public async Task<EventList> GetFixtureEventsAsync(string tournament_stage, string sport, string date, string live)
        {
            string fixtureUrl = $"{_eventServerUrl}fixtures/?username={_username}&token={_token}";
            string finalUrl = GenerateFinalString(fixtureUrl, tournament_stage, sport, date, live, "");
            return await GetRequestAsync<EventList>(new Uri(finalUrl));
        }

        public async Task<EventList> GetResultEventsAsync(string tournament_stage, string sport, string date, string live)
        {
            string resultUrl = $"{_eventServerUrl}results/?username={_username}&token={_token}";
            string finalUrl = GenerateFinalString(resultUrl, tournament_stage, sport, date, live, "");
            return await GetRequestAsync<EventList>(new Uri(finalUrl));
        }

        public async Task<EventList> GetParticipantEventFixturesAsync(string participant, string tournament_stage, int? limit)
        {
            string participantFixtureUrlBase = $"{_eventServerUrl}participant_fixtures/?username={_username}&token={_token}";
            if (!string.IsNullOrWhiteSpace(participant))
            {
                participantFixtureUrlBase += $"&participantFK={participant}";
            }
            if (limit.HasValue)
            {
                participantFixtureUrlBase += $"&limit={limit.Value}";
            }
            if (!string.IsNullOrWhiteSpace(tournament_stage))
            {
                participantFixtureUrlBase += $"&tournament_stageFK={tournament_stage}";
            }
            return await GetRequestAsync<EventList>(new Uri(participantFixtureUrlBase));
        }

        public async Task<EventList> GetParticipantEventResultssAsync(string participant, string tournament_stage, int? limit)
        {
            string participantFixtureUrlBase = $"{_eventServerUrl}participant_results/?username={_username}&token={_token}";
            if (!string.IsNullOrWhiteSpace(participant))
            {
                participantFixtureUrlBase += $"&participantFK={participant}";
            }
            if (limit.HasValue)
            {
                participantFixtureUrlBase += $"&limit={limit.Value}";
            }
            if (!string.IsNullOrWhiteSpace(tournament_stage))
            {
                participantFixtureUrlBase += $"&tournament_stageFK={tournament_stage}";
            }
            return await GetRequestAsync<EventList>(new Uri(participantFixtureUrlBase));
        }
        
        public List<MatchView> CreateMatchViews(EventList list, bool isFixture, bool detailedDate)
        {
            List<MatchView> matchList = new List<MatchView>();
            if (list == null)
            {
                return matchList;
            }
            foreach (var @event in list.events)
            {
                string minute = @event.Value.startdate.Minute < 10
                    ? $"0{@event.Value.startdate.Minute.ToString()}"
                    : (@event.Value.startdate.Minute.ToString());
                var detailedDateString = string.Empty;
                if (detailedDate)
                {
                    var month = @event.Value.startdate.Month < 10
                        ? $"0{@event.Value.startdate.Month.ToString()}"
                        : @event.Value.startdate.Month.ToString();
                    var day = @event.Value.startdate.Day < 10
                        ? $"0{@event.Value.startdate.Day.ToString()}"
                        : @event.Value.startdate.Day.ToString();
                    detailedDateString = $"{@event.Value.startdate.Year}.{month}.{day} {@event.Value.startdate.Hour}:{minute}";
                }
                var homeTeam = @event.Value.event_participants.Values.ElementAt(0);
                var guestTeam = @event.Value.event_participants.Values.ElementAt(1);

                string homeTeamAS = null, guestTeamAS = null;
                if (homeTeam.result.Values.Where(a => a.result_code == "runningscore").SingleOrDefault() != null)
                {
                    // Getting Actual Score ("runningscore")
                    homeTeamAS = homeTeam.result.Values.Where(a => a.result_code == "runningscore").Select(a => a.value).ElementAt(0);
                    guestTeamAS = guestTeam.result.Values.Where(a => a.result_code == "runningscore").Select(a => a.value).ElementAt(0);
                }
                string homeTeamFS = null, guestTeamFS = null;
                if (homeTeam.result.Values.Where(a => a.result_code == "finalresult").SingleOrDefault() != null)
                {
                    // Getting Final Result ("finalresult")
                    homeTeamFS = homeTeam.result.Values.Where(a => a.result_code == "finalresult").Select(a => a.value).ElementAt(0);
                    guestTeamFS = guestTeam.result.Values.Where(a => a.result_code == "finalresult").Select(a => a.value).ElementAt(0);
                }
                string elapsedTime = null;
                if (@event.Value.elapsed != null)
                {
                    elapsedTime = @event.Value.elapsed.Values.ElementAt(0).elapsed + "'";
                }

                MatchView actualMatch = new MatchView()
                {
                    EventId = @event.Key,
                    HomeTeam = homeTeam,
                    GuestTeam = guestTeam,
                    StartingDate = (detailedDate) ? detailedDateString : $"{@event.Value.startdate.Hour}:{minute}",
                    RunningScore = (isFixture) ? " - " : $"{homeTeamAS} - {guestTeamAS}",
                    FinalScore = $"{homeTeamFS} - {guestTeamFS}",
                    Elapsed = elapsedTime,
                    HomeTeamFinalScore = homeTeamFS,
                    GuestTeamFinalScore = guestTeamFS,
                    HomeTeamRunningScore = homeTeamAS,
                    GuestTeamRunningScore = guestTeamAS
                };
                matchList.Add(actualMatch);
            }
            return matchList.OrderBy(m => m.StartingDate).ToList();
        }

        //Helper methods

        private async Task<T> GetRequestAsync<T>(Uri uri)
        where T : class
        {
            using (var client = new HttpClient())
            {
                var json = await client.GetStringAsync(uri);
                T result;
                try
                {
                    result = JsonConvert.DeserializeObject<T>(json);
                }
                catch (JsonSerializationException)
                {
                    return null;
                }

                return result;
            }
        }

        private string GenerateFinalString(string basic, string tournament_stage, string sport, string date, string live, string status_type)
        {
            string finalUrl = basic;
            if (string.IsNullOrEmpty(tournament_stage) && string.IsNullOrEmpty(sport))
            {
                throw new Exception("Required arguments missing!");
            }
            if (string.IsNullOrEmpty(tournament_stage))
            {
                finalUrl += $"&sportFK={sport}";
            }
            else if (string.IsNullOrEmpty(sport))
            {
                finalUrl += $"&tournament_stageFK={tournament_stage}";
            }
            else throw new Exception("Only tournament_stage OR sport FK required!");
            finalUrl = !string.IsNullOrEmpty(date) ? $"{finalUrl}&date={date}" : finalUrl;
            finalUrl = !string.IsNullOrEmpty(live) ? $"{finalUrl}&live={live}" : finalUrl;
            finalUrl = !string.IsNullOrEmpty(status_type) ? $"{finalUrl}&status_type={status_type}" : finalUrl;
            return finalUrl;
        }
    }
}
