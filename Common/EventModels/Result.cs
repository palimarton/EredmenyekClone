using System;

namespace Common.DataModels.EventModels
{
    public class Result
    {
        public string id { get; set; }
        public string event_participantsFK { get; set; }
        public string result_typeFK { get; set; }
        public string result_code { get; set; }
        public string value { get; set; }
        public string n { get; set; }
        public DateTime ut { get; set; }
    }
}
