using System;
using Common.DataModels.AccountModels;
using System.Threading.Tasks;
using Windows.Storage;
using Newtonsoft.Json;
using System.Linq;

namespace Common.Services.UserServices
{
    public class UserService : IUserService
    {
        private User _currentUser;
        private readonly string _fileName = "favouriteTeams.json";

        public async Task<User> GetUser()
        {
            if (_currentUser == null)
            {
                await CreateUser();
            }
            return _currentUser;
        }

        public async Task AddFavouriteTeam(FavouriteTeam newTeam)
        {
            if (_currentUser == null)
            {
                await CreateUser();
            }
            if (_currentUser.FavouriteTeams == null)
            {
                _currentUser.FavouriteTeams = new FavouriteTeamList();
            }
            _currentUser.FavouriteTeams.FavouriteTeams.Add(newTeam);
            await SaveFile();
        }

        public async Task RemoveFavouriteTeam(string teamId)
        {
            if (_currentUser == null)
            {
                await CreateUser();
            }
            if (_currentUser.FavouriteTeams == null)
            {
                throw new Exception($"Empty FavouriteTeamList -> can't remove this team! ({teamId})");
            }
            var itemToRemove = _currentUser.FavouriteTeams.FavouriteTeams.Where(w => w.Id.Equals(teamId)).FirstOrDefault();
            if (itemToRemove != null)
            {
                _currentUser.FavouriteTeams.FavouriteTeams.Remove(itemToRemove);
            }
            await SaveFile();
        }        

        // Helper methods

        private async Task CreateUser()
        {
            _currentUser = new User();
            await LoadFile();
        }

        // Loads favourite teams into _currentUser
        private async Task LoadFile()
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            var files = await localFolder.GetFilesAsync();
            StorageFile favouriteTeamsFile;

            if (files.Where(w => w.Name.Equals(_fileName)).Any())
            {
                favouriteTeamsFile = await localFolder.GetFileAsync(_fileName);
                var text = await FileIO.ReadTextAsync(favouriteTeamsFile);
                _currentUser.FavouriteTeams = JsonConvert.DeserializeObject<FavouriteTeamList>(text);
            }
            else
            {
                favouriteTeamsFile = await localFolder.CreateFileAsync(_fileName);
            }
        }

        // Saves favourite teams into a file
        private async Task SaveFile()
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            var files = await localFolder.GetFilesAsync();
            StorageFile favouriteTeamsFile;

            if (files.Where(w => w.Name.Equals(_fileName)).Any())
            {
                favouriteTeamsFile = await localFolder.GetFileAsync(_fileName);
            }
            else
            {
                favouriteTeamsFile = await localFolder.CreateFileAsync(_fileName);
            }

            var jsonText = JsonConvert.SerializeObject(_currentUser.FavouriteTeams);
            await FileIO.WriteTextAsync(favouriteTeamsFile, jsonText);
        }
    }
}
