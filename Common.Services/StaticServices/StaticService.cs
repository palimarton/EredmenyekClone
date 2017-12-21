using Common.DataModels.StaticModels;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Common.Services.StaticServices
{
    public class StaticService : IStaticService
    {
        private const string _staticServerUrl = "http://demo.eapi.enetpulse.com/static/";
        // Your Enetpulse username
        private const string _username = "";
        // Your Enetpulse token
        private const string _token = "";

        private async Task<T> GetRequestAsync<T>(string uri)
        {
            using (var client = new HttpClient())
            {
                var json = await client.GetStringAsync(new Uri(uri));
                T result = JsonConvert.DeserializeObject<T>(json);
                return result;
            }
        }

        public async Task<ResultTypeList> GetResultTypesAsync()
        {
            string getResultTypesUrl = $"{_staticServerUrl}result_type/?username={_username}&token={_token}";
            return await GetRequestAsync<ResultTypeList>(getResultTypesUrl);
        }

        public async Task<StatusDescriptionList> GetStatusDescriptionsAsync()
        {
            string getStatusDescriptionsUrl = $"{_staticServerUrl}status_desc/?username={_username}&token={_token}";
            return await GetRequestAsync<StatusDescriptionList>(getStatusDescriptionsUrl);
        }

        public async Task<LineupTypeList> GetLineupTypesAsync()
        {
            string getLineupTypesUrl = $"{_staticServerUrl}lineup_type/?username={_username}&token={_token}";
            return await GetRequestAsync<LineupTypeList>(getLineupTypesUrl);
        }

        public async Task<IncidentTypeList> GetIncidentTypesAsync()
        {
            string getIncidentTypesUrl = $"{_staticServerUrl}incident_type/?username={_username}&token={_token}";
            return await GetRequestAsync<IncidentTypeList>(getIncidentTypesUrl);
        }
    }
}
