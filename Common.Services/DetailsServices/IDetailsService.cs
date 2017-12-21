using Common.DataModels.DetailsModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Services.DetailsServices
{
    public interface IDetailsService
    {
        Task<DetailsList> GetDetailsAsync(string match_id, string includeLineup, string includeIncidents,
            string includeExtendedResults, string includeProperties, string includeLivestats);

        List<IncidentViewModel> CreateIncidentViewModels(EventParticipant homeTeam,
            EventParticipant awayTeam);
    }
}
