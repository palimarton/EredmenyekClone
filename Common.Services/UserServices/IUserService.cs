using Common.DataModels.AccountModels;
using System.Threading.Tasks;

namespace Common.Services.UserServices
{
    public interface IUserService
    {
        Task<User> GetUser();

        Task AddFavouriteTeam(FavouriteTeam newTeam);

        Task RemoveFavouriteTeam(string teamId);

        //Task LogInToService();
    }
}
