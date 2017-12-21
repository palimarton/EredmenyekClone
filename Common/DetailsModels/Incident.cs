using System;

namespace Common.DataModels.DetailsModels
{
    public class Incident
    {
        public string id { get; set; }
        public string event_participantsFK { get; set; }
        public string incident_typeFK { get; set; }
        public string incident_code { get; set; }
        public string elapsed { get; set; }
        public string sortorder { get; set; }
        public string n { get; set; }
        public DateTime ut { get; set; }
        public string ref_participantFK { get; set; }
        public Participant participant { get; set; }
    }
}
