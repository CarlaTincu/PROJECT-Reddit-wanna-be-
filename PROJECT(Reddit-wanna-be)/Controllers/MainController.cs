using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PROJECT_Reddit_wanna_be_.Models;
using System.Net.Http;

namespace PROJECT_Reddit_wanna_be_.Controllers
{
    public class MainController : Controller
    {
        //private readonly HttpClient _httpClient;

        //public MainController(HttpClient httpClient)
        //{
        //    _httpClient = httpClient;
        //}
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
            return View();

        }
        [HttpPut]
        public async Task<IActionResult> Topics() 
        {
            

            return View(null);
        }
    }
}
