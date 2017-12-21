namespace Common.DataModels.StandingModels
{
    public class LeagueTableView
    {
        public int Rank { get; set; }
        public string TeamName { get; set; }
        public int MatchesPlayed { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Draws { get; set; }
        public int GoalsFor { get; set; }
        public int GoalsAgainst { get; set; }
        public int Points { get; set; }
        public string TeamParticipantFK { get; set; }
        public string TeamId { get; set; }
    }
}
