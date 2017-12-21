using Common.DataModels.DetailsModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Common.Services.DetailsServices
{
    public class DetailsService : IDetailsService
    {
        private const string _eventServerUrl = "http://demo.eapi.enetpulse.com/event/";
        // Your Enetpulse username
        private const string _username = "";
        // Your Enetpulse token
        private const string _token = "";

        private async Task<T> GetRequestAsync<T>(Uri uri)
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
                    return default(T);
                }

                return result;
            }
        }

        public async Task<DetailsList> GetDetailsAsync(string match_id, string includeLineup, string includeIncidents,
            string includeExtendedResults, string includeProperties, string includeLivestats)
        {
            if (string.IsNullOrWhiteSpace(match_id)) throw new Exception("error: must provide match id!");

            string finalUrl = $"{_eventServerUrl}details/?username={_username}&token={_token}&id={match_id}";

            if (!string.IsNullOrWhiteSpace(includeLineup)) finalUrl += $"&includeLineups={includeLineup}";
            if (!string.IsNullOrWhiteSpace(includeIncidents)) finalUrl += $"&includeIncidents={includeIncidents}";
            if (!string.IsNullOrWhiteSpace(includeExtendedResults)) finalUrl += $"&includeExtendedResults={includeExtendedResults}";
            if (!string.IsNullOrWhiteSpace(includeProperties)) finalUrl += $"&includeProperties={includeProperties}";
            if (!string.IsNullOrWhiteSpace(includeLivestats)) finalUrl += $"&includeLivestats={includeLivestats}";

            return await GetRequestAsync<DetailsList>(new Uri(finalUrl));
        }

        public List<IncidentViewModel> CreateIncidentViewModels(EventParticipant homeTeam, EventParticipant awayTeam)
        {
            List<Incident> homeTeamIncidents = homeTeam.incident != null ? homeTeam.incident.Values.ToList() : null;
            List<Incident> awayTeamIncidents = awayTeam.incident != null ? awayTeam.incident.Values.ToList() : null;
            if (homeTeamIncidents == null && awayTeamIncidents == null) return null;

            List<IncidentViewModel> incidents = new List<IncidentViewModel>();
            List<IncidentViewModel> homeTeamIncidentViewModel = new List<IncidentViewModel>();
            List<IncidentViewModel> awayTeamIncidentViewModel = new List<IncidentViewModel>();

            if (homeTeamIncidents != null)
            {
                for (int i = 0; i < homeTeamIncidents.Count; i++)
                {
                    IncidentViewModel incidentViewModel = new IncidentViewModel();
                    Incident actualIncident = homeTeamIncidents[i];
                    incidentViewModel.IsHome = true;
                    incidentViewModel.ElapsedTime = int.Parse(actualIncident.elapsed);
                    incidentViewModel.Text = actualIncident.participant.name;
                    if (actualIncident.incident_code.Equals("card"))
                    {
                        if (actualIncident.incident_typeFK.Equals("14"))
                            incidentViewModel.Type = "yellow_card";
                        else if (actualIncident.incident_typeFK.Equals("15"))
                            incidentViewModel.Type = "second_yellow_card";
                        else if (actualIncident.incident_typeFK.Equals("16"))
                            incidentViewModel.Type = "red_card";
                    }
                    else if (actualIncident.incident_code.Equals("goal"))
                    {
                        if (actualIncident.incident_typeFK.Equals("9"))
                        {
                            // Missed penalty
                            incidentViewModel.Type = "missed_penalty";
                            incidentViewModel.Text += " (Missed penalty)";
                        }
                        else
                        {
                            incidentViewModel.Type = "goal";
                            if (i < homeTeamIncidents.Count - 1)
                            {
                                if (homeTeamIncidents[i + 1].incident_code.Equals("assist"))
                                {
                                    incidentViewModel.Text += " (" + homeTeamIncidents[i + 1].participant.name + ")";
                                    i++;
                                }
                                else
                                {
                                    for (int j = 0; j < homeTeamIncidents.Count; j++)
                                    {
                                        if (homeTeamIncidents[j].incident_code.Equals("assist") && homeTeamIncidents[j].elapsed == actualIncident.elapsed)
                                        {
                                            incidentViewModel.Text += " (" + homeTeamIncidents[j].participant.name + ")";
                                        }
                                    }
                                }
                            }
                        }                        
                    }
                    else if (actualIncident.incident_code.Equals("subst"))
                    {
                        incidentViewModel.Type = "substitution";
                        if (i < homeTeamIncidents.Count - 1)
                        {
                            if (homeTeamIncidents[i + 1].incident_code.Equals("subst_in"))
                            {
                                incidentViewModel.Text = homeTeamIncidents[i + 1].participant.name + " (" + actualIncident.participant.name + ")";
                                i++;
                            }
                        }
                    }
                    if (!actualIncident.incident_code.Equals("assist"))
                        homeTeamIncidentViewModel.Add(incidentViewModel);
                }
                incidents.AddRange(homeTeamIncidentViewModel.ToList());
            }
            

            if (awayTeamIncidents != null)
            {
                for (int i = 0; i < awayTeamIncidents.Count; i++)
                {
                    IncidentViewModel incidentViewModel = new IncidentViewModel();
                    Incident actualIncident = awayTeamIncidents[i];
                    incidentViewModel.IsHome = false;
                    incidentViewModel.ElapsedTime = int.Parse(actualIncident.elapsed);
                    incidentViewModel.Text = actualIncident.participant.name;
                    if (actualIncident.incident_code.Equals("card"))
                    {
                        if (actualIncident.incident_typeFK.Equals("14"))
                            incidentViewModel.Type = "yellow_card";
                        else if (actualIncident.incident_typeFK.Equals("15"))
                            incidentViewModel.Type = "second_yellow_card";
                        else if (actualIncident.incident_typeFK.Equals("16"))
                            incidentViewModel.Type = "red_card";
                    }
                    else if (actualIncident.incident_code.Equals("goal"))
                    {
                        if (actualIncident.incident_typeFK.Equals("9"))
                        {
                            // Missed penalty
                            incidentViewModel.Type = "missed_penalty";
                            incidentViewModel.Text += " has missed a penalty";
                        }
                        else
                        {
                            incidentViewModel.Type = "goal";
                            if (i < awayTeamIncidents.Count - 1)
                            {
                                if (awayTeamIncidents[i + 1].incident_code.Equals("assist"))
                                {
                                    incidentViewModel.Text += " (" + awayTeamIncidents[i + 1].participant.name + ")";
                                    i++;
                                }
                                else
                                {
                                    for (int j = 0; j < awayTeamIncidents.Count; j++)
                                    {
                                        if (awayTeamIncidents[j].incident_code.Equals("assist") && awayTeamIncidents[j].elapsed == actualIncident.elapsed)
                                        {
                                            incidentViewModel.Text += " (" + awayTeamIncidents[j].participant.name + ")";
                                        }
                                    }
                                }
                            }
                        }                        
                    }
                    else if (actualIncident.incident_code.Equals("subst"))
                    {
                        incidentViewModel.Type = "substitution";
                        if (i < awayTeamIncidents.Count - 1)
                        {
                            if (awayTeamIncidents[i + 1].incident_code.Equals("subst_in"))
                            {
                                incidentViewModel.Text = awayTeamIncidents[i + 1].participant.name + " (" + actualIncident.participant.name + ")";
                                i++;
                            }
                        }
                    }
                    if (!actualIncident.incident_code.Equals("assist"))
                        awayTeamIncidentViewModel.Add(incidentViewModel);
                }
                incidents.AddRange(awayTeamIncidentViewModel.ToList());
            }

            return incidents.OrderBy(i => i.ElapsedTime).ToList();
        }
    }
}
