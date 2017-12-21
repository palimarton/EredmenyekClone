using System;
using System.Collections.Generic;

namespace Common.DataModels.DetailsModels
{
    public class EventParticipant
    {
        public string id { get; set; }
        public string number { get; set; }
        public string participantFK { get; set; }
        public string eventFK { get; set; }
        public string n { get; set; }
        public DateTime ut { get; set; }
        public Dictionary<string, Result> result { get; set; }
        public Dictionary<string, Lineup> lineup { get; set; }
        public Dictionary<string, Incident> incident { get; set; }
        public Participant participant { get; set; }
        public Dictionary<string, Property> property { get; set; }
    }
}
