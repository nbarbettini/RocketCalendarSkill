using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace LaunchCalendarSkill.LaunchLibraryApi
{
    public sealed class LaunchLibraryClient
    {
        private const string LaunchesBaseUrl = "https://launchlibrary.net/1.2/launch?mode=verbose";

        public async Task<Launch[]> GetLaunches(DateTimeOffset startDate, int limit = 10, int offset = 0)
        {
            var url = $"{LaunchesBaseUrl}&startdate={startDate.ToUnixTimeSeconds()}&limit={limit}&offset={offset}";

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.UserAgent.Add(new ProductInfoHeaderValue("LaunchCalendarSkill", "0.0.1"));

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var settings = new JsonSerializerSettings()
            {
                DateFormatString = "yyyyMMddTHHmmssZ",
                DateParseHandling = DateParseHandling.DateTimeOffset
            };
            var launchesResponse = JsonConvert.DeserializeObject<LaunchesResponse>(json, settings);

            return launchesResponse?.Launches ?? new Launch[0];
        }
    }
}
