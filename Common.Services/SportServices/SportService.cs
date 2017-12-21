using System;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Common.DataModels;

namespace Common.Services.SportServices
{
    public class SportService : ISportService
    {
        private const string sportServerUrl = "http://demo.eapi.enetpulse.com/sport/list/";

        private async Task<T> GetRequestAsync<T>(string uri)
        {
            using (var client = new HttpClient())
            {
                var json = await client.GetStringAsync(new Uri(uri));
                T result = JsonConvert.DeserializeObject<T>(json);
                return result;
            }
        }

        public async Task<SportList> GetSportsAsync(string username, string token)
        {
            string finalUrl = $"{sportServerUrl}?username={username}&token={token}";
            return await GetRequestAsync<SportList>(finalUrl);
        }
    }
}
