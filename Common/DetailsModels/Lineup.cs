using System;

namespace Common.DataModels.DetailsModels
{
    public class Lineup
    {
        public string id { get; set; }
        public string event_participantsFK { get; set; }
        public string participantFK { get; set; }
        public string lineup_typeFK { get; set; }
        public string shirt_number { get; set; }
        public string pos { get; set; }
        public string enet_pos { get; set; }
        public string n { get; set; }
        public DateTime ut { get; set; }
        public Participant participant { get; set; }
    }
}