using System;

namespace Common.DataModels.StandingModels
{
    public class StandingData
    {
        public string id { get; set; }
        public string standing_type_paramFK { get; set; }
        public int value { get; set; }
        public string code { get; set; }
        public string n { get; set; }
        public DateTime ut { get; set; }
        public string sub_param { get; set; }
    }
}