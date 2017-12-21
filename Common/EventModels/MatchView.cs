namespace Common.DataModels.EventModels
{
    public class MatchView
    {
        public string EventId { get; set; }
        public EventParticipant HomeTeam { get; set; }
        public EventParticipant GuestTeam { get; set; }
        public string RunningScore { get; set; }
        public string FinalScore { get; set; }
        public string StartingDate { get; set; }
        public string Elapsed { get; set; }
        public string HomeTeamFinalScore { get; set; }
        public string GuestTeamFinalScore { get; set; }
        public string HomeTeamRunningScore { get; set; }
        public string GuestTeamRunningScore { get; set; }
    }
}
