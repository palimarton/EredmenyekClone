using System;
using System.Collections.Generic;

namespace Common.DataModels.DetailsModels
{
    public class Event
    {
        public string id { get; set; }
        public string name { get; set; }
        public string tournament_stageFK { get; set; }
        public DateTime startdate { get; set; }
        public string status_type { get; set; }
        public string status_descFK { get; set; }
        public string n { get; set; }
        public DateTime ut { get; set; }
        public string tournamentFK { get; set; }
        public string tournament_templateFK { get; set; }
        public string sportFK { get; set; }
        public string tournament_stage_name { get; set; }
        public string tournament_name { get; set; }
        public string tournament_template_name { get; set; }
        public string sport_name { get; set; }
        public Dictionary<string, Property> property { get; set; }
        public Dictionary<string, EventParticipant> event_participants { get; set; }
        public Dictionary<string, EventIncident> event_incident { get; set; }
        public Elapsed elapsed { get; set; }
    }
}
