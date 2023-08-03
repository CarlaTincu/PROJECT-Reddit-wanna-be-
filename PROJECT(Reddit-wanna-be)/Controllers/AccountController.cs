using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PROJECT_Reddit_wanna_be_.Models;
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

       // [HttpPost]
        public async Task<IActionResult> abc(string UserName,string Password)
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
                        return RedirectToAction("Index","Home");
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
            var model = new RegisterModel();
            return View(model);
        }


        [HttpPost]
        public IActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Add registration logic here
                // For now, let's assume registration is successful
                // You might want to save the user to a database
                return RedirectToAction("Login");
            }

            return View(model);
        }
        public IActionResult Dashboard()
        {
            return View();
        }

    }
}
