using System;

namespace Common.DataModels.DetailsModels
{
    public class EventIncidentDetail
    {
        public string id { get; set; }
        public string type { get; set; }
        public string event_incidentFK { get; set; }
        public string participantFK { get; set; }
        public string value { get; set; }
        public string n { get; set; }
        public DateTime ut { get; set; }
    }
}