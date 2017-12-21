using System;

namespace Common.DataModels.DetailsModels
{
    public class Participant
    {
        public string id { get; set; }
        public string name { get; set; }
        public string gender { get; set; }
        public string type { get; set; }
        public string countryFK { get; set; }
        public string n { get; set; }
        public DateTime ut { get; set; }
        public string country_name { get; set; }
    }
}
