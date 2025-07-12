

using Newtonsoft.Json.Linq;

namespace BlazorApp1.Data
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;

    public class TikTokAuthService
    {
        private readonly HttpClient _httpClient;

        public TikTokAuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Exchanges an authorization code for an access token.
        /// </summary>
        public async Task<string> GetAccessTokenAsync(string authCode)
        {
            string tokenUrl = "https://open.tiktokapis.com/v2/oauth/token/";

            var formData = new Dictionary<string, string>
        {
            { "client_key", "aw7713vhsjt8i3og" },       // Replace with actual Client ID
            { "client_secret", "rrBkHTaivCtchgxFzSPB5irCJokDdzT7" }, // Replace with actual Client Secret
            { "grant_type", "authorization_code" },
            { "code", authCode },
            { "redirect_uri", "YOUR_REDIRECT_URI" }  // Must match the registered redirect URI
        };

            var content = new FormUrlEncodedContent(formData);
            HttpResponseMessage response = await _httpClient.PostAsync(tokenUrl, content);

            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                JObject data = JObject.Parse(jsonResponse);
                return data["access_token"]?.ToString() ?? "No Access Token";
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
                return "Error Fetching Token";
            }
        }
    }

}
