using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;

namespace PROJECT_Reddit_wanna_be_.Controllers
{
    public class MethodController : Controller
    {
        private readonly HttpClient _client;

        public MethodController()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri("https://localhost:7030/");
        }

        public void ConfigureHttpClient(HttpClient client, string jwtToken)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        }

        public async Task<string> GetJwtTokenAsync(HttpContext httpContext)
        {
            string authToken = httpContext.Request.Cookies["jwt"];

            if (!string.IsNullOrEmpty(authToken))
            {
                try
                {
                    JObject authTokenObject = JObject.Parse(authToken);
                    return authTokenObject["token"]?.ToString();
                }
                catch (JsonReaderException)
                {
                    Console.WriteLine("Invalid JSON format in the cookie.");
                }
            }

            return null;
        }

        public string GetUserIdFromJwtToken(string jwtToken)
        {
            JObject authTokenObject = JObject.Parse(jwtToken);
            string Token = authTokenObject["token"]?.ToString();
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var token = tokenHandler.ReadJwtToken(Token);
                var userIdClaim = token.Claims.FirstOrDefault(c => c.Type == "UserID");

                if (userIdClaim != null)
                {
                    return userIdClaim.Value;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }
        public void ConfigureHttpClient(string jwtToken)
        {
            if (!string.IsNullOrEmpty(jwtToken))
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
            }
        }
        public async Task<T> GetApiResponseAsync<T>(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(responseBody);
            }
            else
            {
                Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                return default(T);
            }
        }
    }
}
