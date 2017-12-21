using System.Collections.Generic;

namespace Common.DataModels.AccountModels
{
    public class FavouriteTeamList
    {
        public List<FavouriteTeam> FavouriteTeams { get; set; }

        public FavouriteTeamList()
        {
            FavouriteTeams = new List<FavouriteTeam>();
        }
    }
}
