using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.DataModels.AccountModels;
using Microsoft.Toolkit.Uwp.Services.OneDrive;
using Windows.Storage;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;
using Newtonsoft.Json;

namespace Common.Services.UserServices
{
    public class OneDriveBasedUserService : IUserService
    {
        private User _currentUser;
        // Your app's client ID
        private static readonly string _appClientId = "";
        private readonly string _fileName = "favouriteTeams.json";

        public async Task AddFavouriteTeam(FavouriteTeam newTeam)
        {
            if (_currentUser.FavouriteTeams == null)
            {
                _currentUser.FavouriteTeams = new FavouriteTeamList();
            }
            if (_currentUser.FavouriteTeams.FavouriteTeams == null)
            {
                _currentUser.FavouriteTeams.FavouriteTeams = new List<FavouriteTeam>();
            }
            _currentUser.FavouriteTeams.FavouriteTeams.Add(newTeam);
            await SaveFavouriteTeams();
        }

        public async Task<User> GetUser()
        {
            if (_currentUser == null || _currentUser.Name.Equals("failed"))
            {
                await CreateUser();
            }
            return _currentUser;
        }

        public async Task RemoveFavouriteTeam(string teamId)
        {
            if (_currentUser.FavouriteTeams == null)
            {
                _currentUser.FavouriteTeams = new FavouriteTeamList();
            }
            if (_currentUser.FavouriteTeams.FavouriteTeams == null)
            {
                _currentUser.FavouriteTeams.FavouriteTeams = new List<FavouriteTeam>();
            }
            var teamToRemove = _currentUser.FavouriteTeams.FavouriteTeams.Where(w => w.Id.Equals(teamId)).FirstOrDefault();
            if (teamToRemove != null)
            {
                _currentUser.FavouriteTeams.FavouriteTeams.Remove(teamToRemove);
            }
            await SaveFavouriteTeams();
        }

        // Helper methods
        private async Task CreateUser()
        {
            _currentUser = new User
            {
                FavouriteTeams = new FavouriteTeamList()
                {
                    FavouriteTeams = new List<FavouriteTeam>()
                },
                Name = "failed"
            };
            try
            {
                await SetUpOneDriveAccess();
                _currentUser.Name = "success";
            }
            catch (Microsoft.Graph.ServiceException)
            {
                _currentUser.Name = "failed";
                throw;
            }
            
            await LoadFavouriteTeams();
        }

        private async Task LoadFavouriteTeams()
        {
            var favouriteTeamsFile = await GetFavouriteTeamsFile();
            using (var stream = await favouriteTeamsFile.OpenAsync())
            {
                byte[] buffer = new byte[stream.Size];
                var localBuffer = await stream.ReadAsync(buffer.AsBuffer(), (uint)stream.Size, InputStreamOptions.ReadAhead);
                var localFolder = ApplicationData.Current.LocalFolder;
                var myLocalFile = await localFolder.CreateFileAsync($"{favouriteTeamsFile.Name}", CreationCollisionOption.ReplaceExisting);
                
                using (var localStream = await myLocalFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    await localStream.WriteAsync(localBuffer);
                    await localStream.FlushAsync();
                }

                var jsonText = await FileIO.ReadTextAsync(myLocalFile);
                _currentUser.FavouriteTeams = JsonConvert.DeserializeObject<FavouriteTeamList>(jsonText);
            }
        }

        private async Task SaveFavouriteTeams()
        {
            var oldFavouriteTeamsFile = await GetFavouriteTeamsFile();
            var appRoot = await OneDriveService.Instance.AppRootFolderAsync();
            await oldFavouriteTeamsFile.RenameAsync("oldFavouriteTeams.json");
            try
            {
                var localFolder = ApplicationData.Current.LocalFolder;
                var localFile = await localFolder.GetFileAsync($"{_fileName}");
                if (localFile != null)
                {
                    var jsonText = JsonConvert.SerializeObject(_currentUser.FavouriteTeams);
                    await FileIO.WriteTextAsync(localFile, jsonText);
                    using (var localStream = await localFile.OpenReadAsync())
                    {
                        var newFavouriteTeamsFile = await appRoot.CreateFileAsync($"{_fileName}", CreationCollisionOption.ReplaceExisting, localStream);
                    }
                }
                await oldFavouriteTeamsFile.DeleteAsync();
            }
            catch (Exception)
            {
                await oldFavouriteTeamsFile.RenameAsync($"{_fileName}");
                throw;
            }
        }

        public static async Task<bool> SetUpOneDriveAccess()
        {
            try
            {
                OneDriveService.Instance.Initialize(_appClientId, OneDriveEnums.AccountProviderType.Msa, OneDriveScopes.AppFolder);
                var success = await OneDriveService.Instance.LoginAsync();
                if (!success)
                {
                    throw new Exception("Could not log into OneDrive");
                }
                return success;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<OneDriveStorageFile> GetFavouriteTeamsFile()
        {
            var appRoot = await OneDriveService.Instance.AppRootFolderAsync();
            var files = await appRoot.GetFilesAsync();
            OneDriveStorageFile favouriteTeamsFile;
            if (!files.Any(a => a.Name.Equals($"{_fileName}")))
            {
                favouriteTeamsFile = await appRoot.CreateFileAsync($"{_fileName}");
            }
            else
            {
                favouriteTeamsFile = files.Where(w => w.Name.Equals($"{_fileName}")).First();
            }
            return favouriteTeamsFile;
        }        
    }
}
