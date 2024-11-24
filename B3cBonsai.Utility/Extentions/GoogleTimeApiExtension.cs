using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B3cBonsai.Utility.Extentions
{
    public class GoogleTimeApiExtension
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public static async Task<DateTimeOffset> GetCurrentTimeAsync()
        {
            var response = await _httpClient.GetAsync("https://www.googleapis.com/oauth2/v1/certs");
            response.EnsureSuccessStatusCode();

            // Lấy thời gian hiện tại từ Google API
            var currentTimestamp = DateTime.Parse(response.Headers.GetValues("Date").First());
            return currentTimestamp;
        }
    }
}
