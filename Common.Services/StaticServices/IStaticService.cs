using Common.DataModels.StaticModels;
using System.Threading.Tasks;

namespace Common.Services.StaticServices
{
    public interface IStaticService
    {
        Task<ResultTypeList> GetResultTypesAsync();

        Task<StatusDescriptionList> GetStatusDescriptionsAsync();

        Task<LineupTypeList> GetLineupTypesAsync();

        Task<IncidentTypeList> GetIncidentTypesAsync();
    }
}
