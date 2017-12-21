using System;
using System.Collections.Generic;

namespace Common.DataModels.DetailsModels
{
    public class EventIncident
    {
        public string id { get; set; }
        public string eventFK { get; set; }
        public string sportFK { get; set; }
        public string event_incident_typeFK { get; set; }
        public string elapsed { get; set; }
        public string elapsed_plus { get; set; }
        public string comment { get; set; }
        public string sortorder { get; set; }
        public string n { get; set; }
        public DateTime ut { get; set; }
        public Dictionary<string, EventIncidentDetail> event_incident_detail { get; set; }
    }
}