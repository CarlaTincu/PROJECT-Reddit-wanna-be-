using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PROJECT_Reddit_wanna_be_.Models;
using PROJECT_Reddit_wanna_be_.Project.Data.Entities;
using System.Net.Http;
using System.Reflection;
using System.Text;

namespace PROJECT_Reddit_wanna_be_.Controllers
{
    public class MainController : Controller
    {
        
        public async Task<IActionResult> MainPage()
        {
            if (true)
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
        }

        [HttpPost]
        public async Task<IActionResult> CreateTopic(Topics topic)
        {
            if (true)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7030/");
                    HttpResponseMessage response = await client.PostAsync("api/topics/Create", new StringContent(JsonConvert.SerializeObject(topic), Encoding.UTF8, "application/json"));

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        var user = JsonConvert.DeserializeObject<PROJECT_Reddit_wanna_be_.Project.Data.Entities.Topics>(responseBody);
                        return RedirectToAction("MainPage");
                    }
                    else
                    {
                        Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                    }
                }
            }
            return View(null);
        }

        [HttpPost]
        public async Task<IActionResult> Topics() 
        {
            return View();
        }
    }
}
