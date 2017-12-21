using System;
using System.Collections.Generic;

namespace Common.DataModels.StandingModels
{
    public class StandingParticipant
    {
        public string id { get; set; }
        public string standingFK { get; set; }
        public string participantFK { get; set; }
        public string n { get; set; }
        public DateTime ut { get; set; }
        public string rank { get; set; }
        public List<StandingData> standing_data { get; set; }
        public Participant participant { get; set; }
    }
}