using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PROJECT_Reddit_wanna_be_.Models;
using System.Text;
using System.Text.Json.Serialization;


namespace PROJECT_Reddit_wanna_be_.Controllers
{
    public class AccountController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string UserName, string Password)
        {
            if (true)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7030/");
                    HttpResponseMessage response = await client.GetAsync($"user/{UserName}/?Password={Password}");
                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        var user = JsonConvert.DeserializeObject<PROJECT_Reddit_wanna_be_.Project.Data.Entities.User>(responseBody);
                        return RedirectToAction("MainPage", "Main");
                    }
                    else
                    {
                        Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                    }
                }
            }

            return View(null);
        }
       


        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (true)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7030/");
                    HttpResponseMessage response = await client.PostAsync("api/users/create", new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        var user = JsonConvert.DeserializeObject<PROJECT_Reddit_wanna_be_.Project.Data.Entities.User>(responseBody);
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                    }
                }
            }
            return View(null);
        }

    }
}