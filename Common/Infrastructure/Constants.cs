using System.Collections.Generic;

namespace Common.DataModels.Infrasturcture
{
    public class Constants
    {
        public class TournamentNames
        {
            public static string Bundesliga     = "Bundesliga";
            public static string LaLiga         = "La Liga";
            public static string SerieA         = "Serie A";
            public static string PremierLeague  = "Premier League";
        }

        public class TournamentStageFK
        {
            public static string Bundesliga     = "850344";
            public static string LaLiga         = "850681";
            public static string SerieA         = "850756";
            public static string PremierLeague  = "850080";
        }

        public static List<string> GetTournamentStageFKs()
        {
            return new List<string>
            {
                TournamentStageFK.Bundesliga,
                TournamentStageFK.LaLiga,
                TournamentStageFK.PremierLeague,
                TournamentStageFK.SerieA
            };
        }

        public class StatisticTypes
        {
            public static string Fouls          = "foulcommit";
            public static string ThrowIn        = "throwin";
            public static string Offside        = "offside";
            public static string Cross          = "cross";
            public static string Possession     = "possession";
            public static string YellowCards    = "yellow_cards";
            public static string ShotOffTarget  = "shotoff";
            public static string BlockedShots   = "blocked_shots";
            public static string ShotOnTarget   = "shoton";
            public static string Counter        = "counter";
            public static string Corner         = "corner";
            public static string RedCards       = "red_cards";
            public static string Goal           = "goal";
            public static string Pass           = "pass";
            public static string Goalkick       = "goalkick";
            public static string Treatment      = "treatment";
            public static string Substitutes    = "subst";
            public static string Assists        = "assists";
            public static string Freekick       = "freekick";
        }
    }
}
