using System;
using System.Collections.Generic;

namespace Common.DataModels.EventModels
{
    public class EventParticipant
    {
        //ID
        public string id { get; set; }
        //???
        public string number { get; set; }
        //résztvevő FK
        public string participantFK { get; set; }
        //event FK
        public string eventFK { get; set; }
        public string n { get; set; }
        public DateTime ut { get; set; }
        //
        public Dictionary<string, Result> result { get; set; }
        public StandingModels.Participant participant { get; set; }
    }
}
