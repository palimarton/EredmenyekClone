using Common.DataModels;
using System.Threading.Tasks;

namespace Common.Services.SportServices
{
    public interface ISportService
    {
        Task<SportList> GetSportsAsync(string username, string token);
    }
}
