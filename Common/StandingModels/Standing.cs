using System;
using System.Collections.Generic;

namespace Common.DataModels.StandingModels
{
    public class Standing
    {
        public string id { get; set; }
        public string _object { get; set; }
        public string objectFK { get; set; }
        public string standing_typeFK { get; set; }
        public string name { get; set; }
        public string n { get; set; }
        public DateTime ut { get; set; }
        public Dictionary<string, StandingParticipant> standing_participants { get; set; }
    }
}