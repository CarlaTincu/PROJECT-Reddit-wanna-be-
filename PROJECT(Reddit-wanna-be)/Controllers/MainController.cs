using Azure;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PROJECT_Reddit_wanna_be_.Models;
using PROJECT_Reddit_wanna_be_.Project.Data.Entities;
using RestSharp;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using WorkingWithAPIApplication.Entities;

namespace PROJECT_Reddit_wanna_be_.Controllers
{

    public class MainController : Controller
    {

        private readonly HttpClient _client;
        private readonly MethodController method;

        public MainController(HttpClient client, MethodController method)
        {
            _client = client;
            this.method = method;
        }

        //TOPICS

        public async Task<IActionResult> MainPage()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7030/");
                HttpResponseMessage response = await client.GetAsync("api/topics");
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var topicsList = JsonConvert.DeserializeObject<List<PROJECT_Reddit_wanna_be_.Project.Data.Entities.Topics>>(responseBody);
                    var model = new MainPageModel
                    {
                        TopicsList = topicsList
                    };

                    return View(model);
                }
                else
                {
                    // Handle error
                    return View("Error");
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateTopic(Topics topic)
        {
            try
            {
                string jwtToken = await method.GetJwtTokenAsync(HttpContext);
                if (!string.IsNullOrEmpty(jwtToken))
                {
                    method.ConfigureHttpClient(_client, jwtToken);
                    string json = JsonConvert.SerializeObject(topic);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await _client.PostAsync("api/topics/Create", content);
                    var createdTopic = await method.GetApiResponseAsync<PROJECT_Reddit_wanna_be_.Project.Data.Entities.Topics>(response);
                    if (createdTopic != null)
                    {
                        return RedirectToAction("MainPage");
                    }
                    else
                    {
                        Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                    }
                }
                else
                {
                    Console.WriteLine("JWT token value not found in the cookie.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            return View(null);
        }
    }
}

