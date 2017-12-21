using System.Collections.Generic;

namespace Common.DataModels.StandingModels
{
    public class EventStatisticsView
    {
        public string Name { get; set; }
        public int HomeTeamValue { get; set; }
        public int AwayTeamValue { get; set; }
        public List<Data> ChartBase { get; set; }
    }

    public class Data
    {
        public double Value { get; set; }
    }
}
