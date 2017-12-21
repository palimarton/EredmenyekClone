using Common.Services.UserServices;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.Data.Json;
using Windows.Security.Authentication.Web.Core;
using Windows.Security.Credentials;
using Windows.Storage;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml.Navigation;

namespace Eredmenyek.ViewModels
{
    public class UserLogInPageViewModel : ViewModelBase
    {
        private const string MicrosoftAccountProvideId = "https://login.microsoft.com";
        private readonly IUserService _userService;

        public UserLogInPageViewModel(IUserService userService)
        {
            _userService = userService;
        }

        public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            AccountsSettingsPane.GetForCurrentView().AccountCommandsRequested += BuildPaneAsync;
            return base.OnNavigatedToAsync(parameter, mode, state);
        }

        public override Task OnNavigatedFromAsync(IDictionary<string, object> pageState, bool suspending)
        {
            AccountsSettingsPane.GetForCurrentView().AccountCommandsRequested -= BuildPaneAsync;
            return base.OnNavigatedFromAsync(pageState, suspending);
        }

        public void LogIn()
        {
            AccountsSettingsPane.Show();
        }

        private async void BuildPaneAsync(AccountsSettingsPane s,
            AccountsSettingsPaneCommandsRequestedEventArgs e)
        {
            var deferral = e.GetDeferral();

            var msaProvider = await WebAuthenticationCoreManager.FindAccountProviderAsync(
                MicrosoftAccountProvideId, "consumers");

            var command = new WebAccountProviderCommand(msaProvider, GetMsaTokenAsync);
            e.WebAccountProviderCommands.Add(command);

            deferral.Complete();
        }

        private async void GetMsaTokenAsync(WebAccountProviderCommand command)
        {
            WebTokenRequest request = new WebTokenRequest(command.WebAccountProvider, "wl.basic");
            WebTokenRequestResult result = await WebAuthenticationCoreManager.RequestTokenAsync(request);

            if (result.ResponseStatus == WebTokenRequestStatus.Success)
            {
                string token = result.ResponseData[0].Token;

                var restApi = new Uri(@"https://apis.live.net/v5.0/me?access_token=" + token);

                using (var client = new HttpClient())
                {
                    var infoResult = await client.GetAsync(restApi);
                    string content = await infoResult.Content.ReadAsStringAsync();

                    var jsonObject = JsonObject.Parse(content);
                    string id = jsonObject["id"].GetString();
                    var currentUser = await _userService.GetUser();
                }
            }
        }

        private void StoreWebAccount(WebAccount account)
        {
            ApplicationData.Current.LocalSettings.Values["CurrentUserProviderId"] = account.WebAccountProvider.Id;
            ApplicationData.Current.LocalSettings.Values["CurrentUserId"] = account.Id;
        }

        private async Task<string> GetTokenSilentlyAsync()
        {
            string providerId = ApplicationData.Current.LocalSettings.Values["CurrentUserProviderId"]?.ToString();
            string accountId = ApplicationData.Current.LocalSettings.Values["CurrentUserId"]?.ToString();

            if (null == providerId || null == accountId)
            {
                return null;
            }

            WebAccountProvider provider = await WebAuthenticationCoreManager.FindAccountProviderAsync(providerId);
            WebAccount account = await WebAuthenticationCoreManager.FindAccountAsync(provider, accountId);

            WebTokenRequest request = new WebTokenRequest(provider, "wl.basic");

            WebTokenRequestResult result = await WebAuthenticationCoreManager.GetTokenSilentlyAsync(request, account);
            if (result.ResponseStatus == WebTokenRequestStatus.UserInteractionRequired)
            {
                // Unable to get a token silently - you'll need to show the UI
                return null;
            }
            else if (result.ResponseStatus == WebTokenRequestStatus.Success)
            {
                // Success
                return result.ResponseData[0].Token;
            }
            else
            {
                // Other error 
                return null;
            }
        }
    }
}
