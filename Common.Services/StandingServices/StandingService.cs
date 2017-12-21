using Common.DataModels.StandingModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Common.Services.StandingServices
{
    public class StandingsService : IStandingService
    {
        public const string _standingsServerUrl = "http://demo.eapi.enetpulse.com/standing/";
        // Your Enetpulse username
        private const string _username = "";
        // Your Enetpulse token
        private const string _token = "";

        private async Task<T> GetRequestAsync<T>(Uri uri)
        {
            using (var client = new HttpClient())
            {
                var json = await client.GetStringAsync(uri);
                try
                {
                    return JsonConvert.DeserializeObject<T>(json);
                }
                catch (JsonSerializationException)
                {
                    return default(T);
                }
            }
        }

        public async Task<StandingsList> GetLeagueStandingsAsync(string _object, string objectFK)
        {
            string finalUrl = $"{_standingsServerUrl}leaguetable/?username={_username}&token={_token}&object={_object}&objectFK={objectFK}";
            return await GetRequestAsync<StandingsList>(new Uri(finalUrl));
        }

        public async Task<StandingsList> GetTopScorersAsync(string _object, string objectFK)
        {
            string finalUrl = $"{_standingsServerUrl}topscorer/?username={_username}&token={_token}&object={_object}&objectFK={objectFK}";
            return await GetRequestAsync<StandingsList>(new Uri(finalUrl));
        }

        public async Task<StandingsList> GetEventStatsAsync(string _object, string objectFK, string id, string includeStandingParticipant, string includeStandingData)
        {
            string finalUrl = $"{_standingsServerUrl}event_stats/?username={_username}&token={_token}&object={_object}&objectFK={objectFK}";
            finalUrl = string.IsNullOrWhiteSpace(id) ? finalUrl : $"{finalUrl}&id={id}";
            finalUrl = string.IsNullOrWhiteSpace(includeStandingParticipant) ? finalUrl : $"{finalUrl}&includeStandingParticipant={includeStandingParticipant}";
            finalUrl = string.IsNullOrWhiteSpace(includeStandingData) ? finalUrl : $"{finalUrl}&includeStandingData={includeStandingData}";
            return await GetRequestAsync<StandingsList>(new Uri(finalUrl));
        }
        
        public List<LeagueTableView> CreateLeagueTable(StandingsList list)
        {
            List<LeagueTableView> finalList = new List<LeagueTableView>();

            foreach (var item in list.standings.Values.ElementAt(0).standing_participants)
            {
                LeagueTableView actualView = new LeagueTableView()
                {
                    Rank = int.Parse(item.Value.rank),
                    TeamName = item.Value.participant.name,
                    MatchesPlayed = item.Value.standing_data.Find(a => a.code == "played").value,
                    Wins = item.Value.standing_data.Find(a => a.code == "wins").value,
                    Draws = item.Value.standing_data.Find(a => a.code == "draws").value,
                    Losses = item.Value.standing_data.Find(a => a.code == "defeits").value,
                    GoalsFor = item.Value.standing_data.Find(a => a.code == "goalsfor").value,
                    GoalsAgainst = item.Value.standing_data.Find(a => a.code == "goalsagainst").value,
                    Points = item.Value.standing_data.Find(a => a.code == "points").value,
                    TeamId = item.Value.id,
                    TeamParticipantFK = item.Value.participantFK
                };
                finalList.Add(actualView);
            }
            finalList = finalList.OrderBy(o => o.Rank).ToList();
            return finalList;
        }

        public List<TopScorersView> CreateTopScorersList(StandingsList list)
        {
            List<TopScorersView> finalList = new List<TopScorersView>();
            foreach (var item in list.standings.Values.ElementAt(0).standing_participants)
            {
                TopScorersView actualPlayer = new TopScorersView()
                {
                    Rank = int.Parse(item.Value.rank),
                    Goals = item.Value.standing_data.Find(a => a.code == "goals").value,
                    PlayerName = item.Value.participant.name
                };
                finalList.Add(actualPlayer);
            }
            finalList = finalList.OrderBy(o => o.Rank).ToList();
            return finalList;
        }

        public List<EventStatisticsView> CreateEventStatisticsList(StandingsList list)
        {
            List<EventStatisticsView> statistics = new List<EventStatisticsView>();
            if (list.standings.Values.ElementAt(0).standing_participants.Count == 0)
            {
                return null;
            }
            var homeTeam = list.standings.Values.ElementAt(0).standing_participants.ElementAt(0).Value;
            var awayTeam = list.standings.Values.ElementAt(0).standing_participants.ElementAt(1).Value;
            if (homeTeam.standing_data.Count != awayTeam.standing_data.Count)
            {
                throw new Exception("alma");
            }
            for (int i = 0; i < homeTeam.standing_data.Count; i++)
            {
                if (homeTeam.standing_data.ElementAt(i).code != awayTeam.standing_data.ElementAt(i).code)
                {
                    throw new Exception("banan");
                }
                statistics.Add(new EventStatisticsView
                {
                    HomeTeamValue = homeTeam.standing_data.ElementAt(i).value,
                    AwayTeamValue = awayTeam.standing_data.ElementAt(i).value,
                    Name = homeTeam.standing_data.ElementAt(i).code,
                    ChartBase = new List<Data>
                    {
                        new Data { Value = homeTeam.standing_data.ElementAt(i).value },
                        new Data { Value = awayTeam.standing_data.ElementAt(i).value }
                    }
                });
            }
            return statistics;
        }
    }
}
